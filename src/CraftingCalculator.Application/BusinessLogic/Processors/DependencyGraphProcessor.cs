using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Works out which records of a <see cref="DatasetSnapshot"/> need which others, and which blueprints end up
/// nested inside themselves. The selection rules in <see cref="TransferSelectionProcessor"/> only ever walk
/// the graph built here, so a new kind of link between records is a change to <see cref="Build"/> alone.
/// </summary>
public static class DependencyGraphProcessor
{
    /// <summary>
    /// Builds the graph of <paramref name="snapshot"/>: a component depends on its category; a blueprint on
    /// its category, its components and the blueprints nested inside it; a favorite on its blueprints.
    /// </summary>
    public static DependencyGraph Build(DatasetSnapshot snapshot)
    {
        List<RecordKey> all = [];
        Dictionary<RecordKey, IReadOnlyList<RecordKey>> dependsOn = [];

        foreach (SnapshotCategory category in snapshot.Categories)
        {
            Add(new RecordKey(RecordKind.Category, category.Id), []);
        }

        foreach (SnapshotComponent component in snapshot.Components)
        {
            Add(new RecordKey(RecordKind.Component, component.Id), CategoryOf(component.CategoryId));
        }

        foreach (SnapshotBlueprint blueprint in snapshot.Blueprints)
        {
            Add(new RecordKey(RecordKind.Blueprint, blueprint.Id),
            [
                .. CategoryOf(blueprint.CategoryId),
                .. Links(RecordKind.Component, blueprint.Components),
                .. Links(RecordKind.Blueprint, blueprint.Blueprints)
            ]);
        }

        foreach (SnapshotFavorite favorite in snapshot.Favorites)
        {
            Add(new RecordKey(RecordKind.Favorite, favorite.Id), Links(RecordKind.Blueprint, favorite.Blueprints));
        }

        Dictionary<RecordKey, List<RecordKey>> usedBy = all.ToDictionary(key => key, _ => new List<RecordKey>());

        foreach (RecordKey user in all)
        {
            foreach (RecordKey dependency in dependsOn[user])
            {
                usedBy[dependency].Add(user);
            }
        }

        return new DependencyGraph(
            all,
            dependsOn,
            usedBy.ToDictionary(pair => pair.Key, IReadOnlyList<RecordKey> (pair) => pair.Value));

        void Add(RecordKey key, IEnumerable<RecordKey> dependencies)
        {
            all.Add(key);

            // Distinct because nothing stops a blueprint listing the same component twice, and a repeated
            // edge would list its user twice under UsedBy.
            dependsOn[key] = [.. dependencies.Distinct()];
        }
    }

    /// <summary>
    /// The blueprints that end up nested inside themselves: every blueprint that nests itself directly, or
    /// sits on a loop of blueprints that nest each other. A blueprint that only nests one of those, without
    /// being nested back, is not included. Empty when there is no such loop.
    /// </summary>
    public static IReadOnlySet<RecordKey> FindCycles(DependencyGraph graph) => new CycleFinder(graph).Run();

    private static IEnumerable<RecordKey> CategoryOf(int? categoryId) =>
        categoryId is { } id ? [new RecordKey(RecordKind.Category, id)] : [];

    private static IEnumerable<RecordKey> Links(RecordKind kind, IEnumerable<QuantityLink> links) =>
        links.Select(link => new RecordKey(kind, link.TargetId));

    // Tarjan's strongly connected components, run over blueprint-to-blueprint edges only: a loop of blueprints is
    // exactly a component with more than one member, or one member that links to itself. Written with an explicit
    // stack rather than recursion because an import file can nest blueprints deeper than the call stack allows.
    // One instance serves one FindCycles call.
    private sealed class CycleFinder(DependencyGraph graph)
    {
        private readonly HashSet<RecordKey> _inCycle = [];
        private readonly Dictionary<RecordKey, int> _discoveredAt = [];
        private readonly Dictionary<RecordKey, int> _lowestReachable = [];
        private readonly Stack<RecordKey> _open = new();
        private readonly HashSet<RecordKey> _isOpen = [];
        private readonly Stack<(RecordKey Blueprint, int NextChild)> _walk = new();

        public IReadOnlySet<RecordKey> Run()
        {
            // Where is evaluated lazily, so the ContainsKey check sees the blueprints discovered by earlier walks.
            foreach (RecordKey start in graph.All.Where(key =>
                         key.Kind == RecordKind.Blueprint && !_discoveredAt.ContainsKey(key)))
            {
                Walk(start);
            }

            return _inCycle;
        }

        private void Walk(RecordKey start)
        {
            Discover(start);

            while (_walk.TryPop(out (RecordKey Blueprint, int NextChild) frame))
            {
                IReadOnlyList<RecordKey> children = graph.DependsOn[frame.Blueprint];
                int next = SkipDiscoveredChildren(frame.Blueprint, children, frame.NextChild);

                if (next < children.Count)
                {
                    // Resumed at the following child once this one's walk is finished.
                    _walk.Push((frame.Blueprint, next + 1));
                    Discover(children[next]);
                }
                else
                {
                    Finish(frame.Blueprint);
                }
            }
        }

        /// <summary>
        /// The index of the first child of <paramref name="blueprint"/>, from <paramref name="from"/> on, that is an
        /// undiscovered blueprint; <c>children.Count</c> when there is none. Every discovered child passed over that
        /// is still open lowers what <paramref name="blueprint"/> can reach.
        /// </summary>
        private int SkipDiscoveredChildren(RecordKey blueprint, IReadOnlyList<RecordKey> children, int from)
        {
            for (int next = from; next < children.Count; next++)
            {
                RecordKey child = children[next];

                if (child.Kind != RecordKind.Blueprint)
                {
                    continue;
                }

                if (!_discoveredAt.TryGetValue(child, out int order))
                {
                    return next;
                }

                if (_isOpen.Contains(child))
                {
                    _lowestReachable[blueprint] = Math.Min(_lowestReachable[blueprint], order);
                }
            }

            return children.Count;
        }

        private void Discover(RecordKey blueprint)
        {
            int order = _discoveredAt.Count;
            _discoveredAt[blueprint] = order;
            _lowestReachable[blueprint] = order;
            _open.Push(blueprint);
            _isOpen.Add(blueprint);
            _walk.Push((blueprint, 0));
        }

        /// <summary>
        /// Closes the component rooted at <paramref name="blueprint"/> once all its children are walked, and passes
        /// what it can reach up to the blueprint that nests it.
        /// </summary>
        private void Finish(RecordKey blueprint)
        {
            if (_lowestReachable[blueprint] == _discoveredAt[blueprint])
            {
                CloseComponent(blueprint);
            }

            if (_walk.TryPeek(out (RecordKey Blueprint, int NextChild) parent))
            {
                _lowestReachable[parent.Blueprint] =
                    Math.Min(_lowestReachable[parent.Blueprint], _lowestReachable[blueprint]);
            }
        }

        private void CloseComponent(RecordKey root)
        {
            List<RecordKey> members = [];
            RecordKey member;

            do
            {
                member = _open.Pop();
                _isOpen.Remove(member);
                members.Add(member);
            }
            while (member != root);

            if (members.Count > 1 || graph.DependsOn[root].Contains(root))
            {
                _inCycle.UnionWith(members);
            }
        }
    }
}
