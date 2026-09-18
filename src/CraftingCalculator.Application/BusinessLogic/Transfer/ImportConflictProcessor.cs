using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.Application.BusinessLogic.Transfer;

/// <summary>
/// Works out which incoming records share a name with records already in a dataset, and whether merging them would
/// nest a blueprint inside itself.
/// </summary>
public static class ImportConflictProcessor
{
    /// <summary>
    /// The records of <paramref name="incoming"/> named the same as a record of the same kind in
    /// <paramref name="current"/>, ignoring case and surrounding spaces, grouped by kind in <see cref="RecordKind"/>
    /// order and then in the order <paramref name="incoming"/> lists them. Each record of <paramref name="current"/>
    /// is matched at most once, by the first incoming record with its name. When <paramref name="current"/> holds
    /// more than one record with a name, the one with the lowest id is matched.
    /// </summary>
    public static IReadOnlyList<ImportConflict> Find(DatasetSnapshot incoming, DatasetSnapshot current) =>
    [
        .. Match(RecordKind.Category,
            incoming.Categories.Select(record => (record.Id, record.Name)),
            current.Categories.Select(record => (record.Id, record.Name))),
        .. Match(RecordKind.Component,
            incoming.Components.Select(record => (record.Id, record.Name)),
            current.Components.Select(record => (record.Id, record.Name))),
        .. Match(RecordKind.Blueprint,
            incoming.Blueprints.Select(record => (record.Id, record.Name)),
            current.Blueprints.Select(record => (record.Id, record.Name))),
        .. Match(RecordKind.Favorite,
            incoming.Favorites.Select(record => (record.Id, record.Name)),
            current.Favorites.Select(record => (record.Id, record.Name)))
    ];

    /// <summary>
    /// The names of the blueprints that merging <paramref name="plan"/> into <paramref name="current"/> would leave
    /// nested inside themselves, sorted. Empty when the merge nests none. A blueprint that is already nested inside
    /// itself in <paramref name="current"/> is not named.
    /// </summary>
    public static IReadOnlyList<string> FindNewCycles(MergePlan plan, DatasetSnapshot current)
    {
        IReadOnlySet<RecordKey> before = DependencyGraphProcessor.FindCycles(DependencyGraphProcessor.Build(current));
        DatasetSnapshot merged = Merge(plan, current);
        IReadOnlySet<RecordKey> after = DependencyGraphProcessor.FindCycles(DependencyGraphProcessor.Build(merged));

        return
        [
            .. merged.Blueprints
                .Where(blueprint =>
                {
                    RecordKey key = new(RecordKind.Blueprint, blueprint.Id);
                    return after.Contains(key) && !before.Contains(key);
                })
                .Select(blueprint => blueprint.Name)
                .Order(StringComparer.OrdinalIgnoreCase)
        ];
    }

    private static IEnumerable<ImportConflict> Match(
        RecordKind kind, IEnumerable<(int Id, string Name)> incoming, IEnumerable<(int Id, string Name)> current)
    {
        // A dataset is allowed two records with one name. Matching the lowest id lands the same file on the same
        // record every time.
        Dictionary<string, int> existing = current
            .GroupBy(record => record.Name.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Min(record => record.Id), StringComparer.OrdinalIgnoreCase);

        foreach ((int id, string name) in incoming)
        {
            // Removed once matched, so a second incoming record with the name is added as a record of its own: two
            // incoming records landing on one existing record would leave the merge depending on which wrote last.
            if (existing.Remove(name.Trim(), out int existingId))
            {
                yield return new ImportConflict(kind, id, existingId, name);
            }
        }
    }

    /// <summary>
    /// What <paramref name="current"/> holds once <paramref name="plan"/> is merged into it: the records the plan
    /// writes carry the id of the record they land on, and an added record is numbered after the highest id of its
    /// kind.
    /// </summary>
    private static DatasetSnapshot Merge(MergePlan plan, DatasetSnapshot current)
    {
        DatasetSnapshot incoming = plan.Incoming;

        Dictionary<RecordKey, int> landsOn = plan.Conflicts.ToDictionary(
            conflict => new RecordKey(conflict.Kind, conflict.IncomingId), conflict => conflict.ExistingId);
        HashSet<RecordKey> matched = [.. landsOn.Keys];
        HashSet<RecordKey> overwritten =
        [
            .. plan.Conflicts
                .Where(conflict => plan.Replace.Contains(new RecordKey(conflict.Kind, conflict.IncomingId)))
                .Select(conflict => new RecordKey(conflict.Kind, conflict.ExistingId))
        ];

        NumberAdded(RecordKind.Category, incoming.Categories.Select(record => record.Id), current.Categories.Select(record => record.Id));
        NumberAdded(RecordKind.Component, incoming.Components.Select(record => record.Id), current.Components.Select(record => record.Id));
        NumberAdded(RecordKind.Blueprint, incoming.Blueprints.Select(record => record.Id), current.Blueprints.Select(record => record.Id));
        NumberAdded(RecordKind.Favorite, incoming.Favorites.Select(record => record.Id), current.Favorites.Select(record => record.Id));

        return new DatasetSnapshot(
            current.DatasetName,
            [
                .. current.Categories.Where(record => Kept(RecordKind.Category, record.Id)),
                .. incoming.Categories.Where(record => Written(RecordKind.Category, record.Id))
                    .Select(record => record with { Id = Target(RecordKind.Category, record.Id) })
            ],
            [
                .. current.Components.Where(record => Kept(RecordKind.Component, record.Id)),
                .. incoming.Components.Where(record => Written(RecordKind.Component, record.Id))
                    .Select(record => record with
                    {
                        Id = Target(RecordKind.Component, record.Id),
                        CategoryId = TargetCategory(record.CategoryId)
                    })
            ],
            [
                .. current.Blueprints.Where(record => Kept(RecordKind.Blueprint, record.Id)),
                .. incoming.Blueprints.Where(record => Written(RecordKind.Blueprint, record.Id))
                    .Select(record => record with
                    {
                        Id = Target(RecordKind.Blueprint, record.Id),
                        CategoryId = TargetCategory(record.CategoryId),
                        Components = TargetLinks(RecordKind.Component, record.Components),
                        Blueprints = TargetLinks(RecordKind.Blueprint, record.Blueprints)
                    })
            ],
            [
                .. current.Favorites.Where(record => Kept(RecordKind.Favorite, record.Id)),
                .. incoming.Favorites.Where(record => Written(RecordKind.Favorite, record.Id))
                    .Select(record => record with
                    {
                        Id = Target(RecordKind.Favorite, record.Id),
                        Blueprints = TargetLinks(RecordKind.Blueprint, record.Blueprints)
                    })
            ],
            current.Settings);

        void NumberAdded(RecordKind kind, IEnumerable<int> incomingIds, IEnumerable<int> currentIds)
        {
            int next = currentIds.DefaultIfEmpty(0).Max();

            foreach (RecordKey key in incomingIds.Select(id => new RecordKey(kind, id)).Where(key => !matched.Contains(key)))
            {
                landsOn[key] = ++next;
            }
        }

        bool Kept(RecordKind kind, int currentId) => !overwritten.Contains(new RecordKey(kind, currentId));

        bool Written(RecordKind kind, int incomingId)
        {
            RecordKey key = new(kind, incomingId);
            return !matched.Contains(key) || plan.Replace.Contains(key);
        }

        int Target(RecordKind kind, int incomingId) => landsOn[new RecordKey(kind, incomingId)];

        int? TargetCategory(int? incomingId) => incomingId is { } id ? Target(RecordKind.Category, id) : null;

        List<QuantityLink> TargetLinks(RecordKind kind, IEnumerable<QuantityLink> links) =>
            [.. links.Select(link => link with { TargetId = Target(kind, link.TargetId) })];
    }
}
