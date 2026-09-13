using System.Collections.Frozen;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// The selection rules shared by export and import, which keep every selected record's dependencies
/// selected with it. Each method returns the <see cref="SelectionChange"/> an action would make without
/// making it, so the caller can ask the user first and then apply <see cref="SelectionChange.Added"/> and
/// <see cref="SelectionChange.Removed"/> itself.
/// </summary>
public static class TransferSelectionProcessor
{
    private static readonly IReadOnlySet<RecordKey> NoKeys = FrozenSet<RecordKey>.Empty;

    /// <summary>Selects <paramref name="key"/> and everything it depends on, at any depth.</summary>
    public static SelectionChange Select(DependencyGraph graph, IReadOnlySet<RecordKey> selected, RecordKey key)
    {
        HashSet<RecordKey> acted = [key];
        return SelectClosure(graph, selected, acted, acted);
    }

    /// <summary>Deselects <paramref name="key"/> and everything that depends on it, at any depth.</summary>
    public static SelectionChange Deselect(DependencyGraph graph, IReadOnlySet<RecordKey> selected, RecordKey key) =>
        DeselectClosure(graph, selected, [key]);

    /// <summary>Selects every record of <paramref name="kind"/> and everything those depend on.</summary>
    public static SelectionChange SelectAll(DependencyGraph graph, IReadOnlySet<RecordKey> selected, RecordKind kind)
    {
        HashSet<RecordKey> acted = OfKind(graph, kind);
        return SelectClosure(graph, selected, acted, acted);
    }

    /// <summary>Deselects every record of <paramref name="kind"/> and everything that depends on those.</summary>
    public static SelectionChange DeselectAll(DependencyGraph graph, IReadOnlySet<RecordKey> selected, RecordKind kind) =>
        DeselectClosure(graph, selected, OfKind(graph, kind));

    /// <summary>
    /// Selects the components and blueprints filed directly under any of <paramref name="categories"/>, and
    /// everything those depend on. <see cref="SelectionChange.CascadedByKind"/> counts all of it: only the
    /// categories themselves are the records acted on.
    /// </summary>
    public static SelectionChange SelectUsersOf(
        DependencyGraph graph, IReadOnlySet<RecordKey> selected, IReadOnlyCollection<RecordKey> categories) =>
        SelectClosure(graph, selected, categories.SelectMany(category => graph.UsedBy[category]), new HashSet<RecordKey>(categories));

    /// <summary>How many of the records of <paramref name="kind"/> are in <paramref name="selected"/>.</summary>
    public static SelectionState StateOf(DependencyGraph graph, IReadOnlySet<RecordKey> selected, RecordKind kind)
    {
        int chosen = CountSelected(graph, selected, kind);

        return chosen == 0 ? SelectionState.None : StateOf(chosen, graph.All.Count(key => key.Kind == kind));
    }

    /// <summary>The state of a group of <paramref name="count"/> records, <paramref name="selected"/> of them selected.</summary>
    public static SelectionState StateOf(int selected, int count)
    {
        if (selected == 0)
        {
            return SelectionState.None;
        }

        return selected == count ? SelectionState.All : SelectionState.Some;
    }

    /// <summary>The number of records of <paramref name="kind"/> in <paramref name="selected"/>.</summary>
    public static int CountSelected(DependencyGraph graph, IReadOnlySet<RecordKey> selected, RecordKind kind) =>
        graph.All.Count(key => key.Kind == kind && selected.Contains(key));

    /// <summary>Whether every record <paramref name="selected"/> depends on is selected too.</summary>
    public static bool IsClosed(DependencyGraph graph, IReadOnlySet<RecordKey> selected) =>
        selected.All(key => graph.DependsOn[key].All(selected.Contains));

    /// <summary>
    /// The <paramref name="selected"/> records of <paramref name="snapshot"/> as a snapshot of their own, each list
    /// in the order <paramref name="snapshot"/> has it and every id unchanged.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="selected"/> leaves out a record that a selected record depends on.
    /// </exception>
    public static DatasetSnapshot Extract(DatasetSnapshot snapshot, IReadOnlySet<RecordKey> selected)
    {
        // A link to a record left out would point at nothing once the extract is written.
        if (!IsClosed(DependencyGraphProcessor.Build(snapshot), selected))
        {
            throw new InvalidOperationException("The selection leaves out a record that a selected record depends on.");
        }

        return new DatasetSnapshot(
            snapshot.DatasetName,
            [.. snapshot.Categories.Where(category => selected.Contains(new RecordKey(RecordKind.Category, category.Id)))],
            [.. snapshot.Components.Where(component => selected.Contains(new RecordKey(RecordKind.Component, component.Id)))],
            [.. snapshot.Blueprints.Where(blueprint => selected.Contains(new RecordKey(RecordKind.Blueprint, blueprint.Id)))],
            [.. snapshot.Favorites.Where(favorite => selected.Contains(new RecordKey(RecordKind.Favorite, favorite.Id)))]);
    }

    private static SelectionChange SelectClosure(
        DependencyGraph graph, IReadOnlySet<RecordKey> selected, IEnumerable<RecordKey> roots, IReadOnlySet<RecordKey> acted)
    {
        HashSet<RecordKey> added = Walk(roots, graph.DependsOn, key => !selected.Contains(key));
        return new SelectionChange(added, NoKeys, CountCascaded(added, acted));
    }

    private static SelectionChange DeselectClosure(
        DependencyGraph graph, IReadOnlySet<RecordKey> selected, HashSet<RecordKey> roots)
    {
        HashSet<RecordKey> removed = Walk(roots, graph.UsedBy, selected.Contains);
        return new SelectionChange(NoKeys, removed, CountCascaded(removed, roots));
    }

    /// <summary>
    /// Every key reachable from <paramref name="roots"/> along <paramref name="edges"/>, the roots included,
    /// that <paramref name="changes"/> accepts.
    /// </summary>
    private static HashSet<RecordKey> Walk(
        IEnumerable<RecordKey> roots,
        IReadOnlyDictionary<RecordKey, IReadOnlyList<RecordKey>> edges,
        Func<RecordKey, bool> changes)
    {
        // The visited set is what keeps a shared sub-recipe - Bronze, reached through both the Bronze Axe and
        // the Bronze Nails - to one visit. The walk carries on through a key that does not change, so the
        // result does not depend on the selection already being closed.
        HashSet<RecordKey> visited = [.. roots];
        Queue<RecordKey> pending = new(visited);
        HashSet<RecordKey> changed = [];

        while (pending.TryDequeue(out RecordKey key))
        {
            if (changes(key))
            {
                changed.Add(key);
            }

            // Where is evaluated lazily, one edge at a time, so an edge is checked against visited only after
            // every earlier edge of this key has been added to it.
            foreach (RecordKey next in edges[key].Where(neighbor => !visited.Contains(neighbor)))
            {
                visited.Add(next);
                pending.Enqueue(next);
            }
        }

        return changed;
    }

    private static IReadOnlyDictionary<RecordKind, int> CountCascaded(
        IEnumerable<RecordKey> changed, IReadOnlySet<RecordKey> acted) =>
        changed
            .Where(key => !acted.Contains(key))
            .GroupBy(key => key.Kind)
            .ToDictionary(group => group.Key, group => group.Count());

    private static HashSet<RecordKey> OfKind(DependencyGraph graph, RecordKind kind) =>
        [.. graph.All.Where(key => key.Kind == kind)];
}
