using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using System.Collections.ObjectModel;
using CraftingCalculator.Domain.Models;
using Microsoft.Extensions.Logging;

namespace CraftingCalculator.UI.State;

/// <summary>
/// The batch of blueprints currently being priced out on the Craft screen. Scoped and shared across
/// pages (Favorites/Dataset can load into or read from it) - the one exception to "state rides in the
/// route", since this is genuine cross-page session state. Components subscribe to <see cref="Changed"/>
/// in <c>OnInitialized</c> and unsubscribe in <c>Dispose</c>.
/// </summary>
public sealed partial class CraftState(
    IBlueprintService blueprintService,
    IFavoriteService favoriteService,
    ISelectedDatasetState selectedDataset,
    ILogger<CraftState> logger)
{
    private static readonly IReadOnlySet<int> NoStops = new HashSet<int>();

    private readonly BlueprintMap _blueprintMap = new();

    // The batch's already crafted blueprints, by id. Recalculate keeps it to blueprints nested below the batch's roots.
    private readonly HashSet<int> _alreadyCraftedIds = [];

    // Nested below the roots, but only inside an already crafted blueprint, so marking them would change nothing.
    private HashSet<int> _insideAlreadyCrafted = [];

    // Keyed by each node's full path from the tree root (see PathOf), not by the record alone - the same
    // blueprint/component can appear more than once in one tree (e.g. a blueprint used both standalone in the
    // batch and nested inside another batch blueprint), and each position remembers its own expansion state.
    private readonly HashSet<string> _expandedPaths = [];

    // Bumped by Clear: the user clearing the batch, and every dataset switch, dataset delete and delete-all. A background
    // read captures it first and drops its result if it changed, so a read begun for the old batch never lands in the new one.
    private int _clearCount;

    public event Action? Changed;

    /// <summary>
    /// Raised when <see cref="ExpandAll"/> or <see cref="CollapseAll"/> changes every row of the Crafting Steps tree at
    /// once. <see cref="ToggleExpanded"/> doesn't raise it: the row that was toggled already shows its new state.
    /// </summary>
    public event Action? ExpansionChanged;

    /// <summary>
    /// Raised when <see cref="HasVisibleCraftCountedStep"/> or <see cref="HasVisibleAlreadyCraftedStep"/> changes,
    /// whatever changed it.
    /// </summary>
    public event Action? LegendChanged;

    public IReadOnlyList<BlueprintQuantity> BlueprintQuantities => _blueprintMap.BlueprintList;

    /// <summary>
    /// Everything the batch gathers rather than crafts, by name: its components, and the blueprints it takes as
    /// already crafted.
    /// </summary>
    public IReadOnlyList<IBaseQuantityRecord> TotalComponents { get; private set; } = [];
    public IReadOnlyList<BlueprintNode> TreeRoots { get; private set; } = [];
    public double TotalCost { get; private set; }
    public double TotalValue { get; private set; }
    public double Profit => TotalValue - TotalCost;

    /// <summary>
    /// Total units of raw material the batch needs, summed across every distinct component and already crafted
    /// blueprint.
    /// </summary>
    public long TotalComponentCount { get; private set; }

    /// <summary>
    /// How many craft operations the breakdown implies: every blueprint in the tree, at every depth,
    /// counted once per craft rather than once per row - four Frames that each need two Brackets are
    /// eight Bracket crafts plus four Frame crafts, or half that many Bracket crafts if the Bracket
    /// blueprint yields two. Component leaves are not steps: they are gathered, not crafted.
    /// </summary>
    public long CraftingStepCount { get; private set; }

    /// <summary>
    /// The items the batch produces beyond what it asked for, because a craft is indivisible - needing
    /// three of something that yields two runs two crafts and leaves one spare. Merged across the whole
    /// batch and every depth of the tree.
    /// </summary>
    public ReadOnlyCollection<BlueprintQuantity> SurplusStock { get; private set; } =
        ReadOnlyCollection<BlueprintQuantity>.Empty;

    /// <summary>Total surplus items, summed across every distinct blueprint.</summary>
    public long SurplusCount { get; private set; }

    /// <summary>
    /// How long the whole batch takes: every craft at every depth plus the components those crafts
    /// consume, summed as though the batch were made one step at a time.
    /// </summary>
    public TimeSpan TotalProductionTime { get; private set; }

    /// <summary>What the surplus is worth. Deliberately not part of <see cref="Profit"/>.</summary>
    public double SurplusValue { get; private set; }

    /// <summary>
    /// The favorite the current batch came from, or null when it was built by hand or cleared. Drives
    /// the "update or create new" branch of the save flow (see Components/Dialogs/FavoritePrompts).
    /// </summary>
    public BlueprintFavorite? LoadedFavorite { get; private set; }

    /// <summary>
    /// The ids of the blueprints the batch takes as already crafted rather than crafting. Each one is nested below the
    /// batch, and none is one of its entries.
    /// </summary>
    public IReadOnlySet<int> AlreadyCraftedIds => _alreadyCraftedIds;

    /// <summary>Every blueprint nested below the batch's entries, and not itself an entry, by name.</summary>
    public IReadOnlyList<BlueprintModel> AlreadyCraftedCandidates { get; private set; } = [];

    public bool IsAlreadyCrafted(int blueprintId) => _alreadyCraftedIds.Contains(blueprintId);

    /// <summary>
    /// Whether the blueprint is only reached through blueprints the batch takes as already crafted, so marking it
    /// makes no difference while they stay marked.
    /// </summary>
    public bool IsInsideAlreadyCrafted(int blueprintId) => _insideAlreadyCrafted.Contains(blueprintId);

    /// <summary>
    /// Marks the blueprint as already crafted for this batch, or unmarks it. A blueprint that isn't one of
    /// <see cref="AlreadyCraftedCandidates"/> doesn't stay marked.
    /// </summary>
    public void SetAlreadyCrafted(int blueprintId, bool alreadyCrafted)
    {
        if (alreadyCrafted ? _alreadyCraftedIds.Add(blueprintId) : _alreadyCraftedIds.Remove(blueprintId))
        {
            Recalculate();
        }
    }

    /// <summary>Adds a blueprint to the batch, or raises its quantity by one when it is already there.</summary>
    public void Add(BlueprintModel blueprint)
    {
        _blueprintMap.Add(blueprint, 1);
        Recalculate();
    }

    /// <summary>
    /// Sets the blueprint's quantity in the batch. Zero is a valid quantity that keeps the blueprint
    /// selected and contributing nothing; a negative quantity is ignored.
    /// </summary>
    public void SetQuantity(BlueprintQuantity target, long quantity)
    {
        // The floor for the whole batch: a negative quantity reaches Recalculate and puts negative
        // components, cost and value on the Crafting Summary. Both callers already clamp at zero
        // (MudNumericField's Min, and Step), so this holds the invariant for whatever calls it next.
        if (quantity < 0)
        {
            return;
        }

        target.Quantity = quantity;
        Recalculate();
    }

    /// <summary>
    /// Moves the blueprint's quantity by <paramref name="step"/>, which is negative to step down.
    /// Stepping down settles at zero, and stepping down again from zero removes the blueprint from the
    /// batch. Stepping up saturates at <see cref="long.MaxValue"/>.
    /// </summary>
    public void Step(BlueprintQuantity target, long step)
    {
        if (QuantityStepProcessor.Step(target.Quantity, step) is { } quantity)
        {
            SetQuantity(target, quantity);
        }
        else
        {
            Remove(target);
        }
    }

    public void Remove(BlueprintQuantity target)
    {
        _blueprintMap.Remove(target);
        Recalculate();
    }

    public void Clear()
    {
        _clearCount++;
        _blueprintMap.Reset();
        _alreadyCraftedIds.Clear();
        LoadedFavorite = null;
        Recalculate();
    }

    /// <summary>
    /// Replaces the batch with <paramref name="favorite"/>'s blueprints and makes it <see cref="LoadedFavorite"/>. Returns
    /// false, with nothing loaded, when the batch was cleared while the favorite was being read.
    /// </summary>
    public async Task<bool> LoadFavoriteAsync(BlueprintFavorite favorite)
    {
        // Read before the batch is touched. Resetting first empties the user's batch and then leaves it empty
        // if the read throws, and the Craft screen's failure dialog promises the opposite - it tells them the
        // batch is as they left it.
        int clearCount = _clearCount;
        Batch batch = await Task.Run(() => favoriteService.GetBatchForFavoriteAsync(favorite));

        if (clearCount != _clearCount)
        {
            return false;
        }

        _blueprintMap.Reset();

        foreach (BlueprintQuantity quantity in batch.Blueprints)
        {
            _blueprintMap.Add(quantity.Blueprint, quantity.Quantity);
        }

        _alreadyCraftedIds.Clear();
        _alreadyCraftedIds.UnionWith(batch.AlreadyCraftedIds);

        LoadedFavorite = favorite;
        Recalculate();

        return true;
    }

    /// <summary>
    /// Reads every blueprint in the batch again, keeping its quantity, so the batch prices what is saved now rather
    /// than what was loaded. A blueprint that no longer exists is removed from the batch. Call it after any write to
    /// the dataset's records; a batch that fails to reload keeps what it had, and the failure is not rethrown.
    /// </summary>
    public async Task ReloadBlueprintsAsync()
    {
        if (_blueprintMap.BlueprintList.Count == 0)
        {
            return;
        }

        HashSet<int> ids = [.. _blueprintMap.BlueprintList.Select(entry => entry.Blueprint.Id)];
        Dictionary<int, BlueprintModel> reloaded;

        try
        {
            // SQLite blocks the thread it runs on, so the read goes to the background and only the batch changes here.
            reloaded = await Task.Run(() => blueprintService.GetBlueprintsByIdsAsync(ids));
        }
        catch (Exception exception)
        {
            // Every caller has already written its change, so a batch that couldn't refresh is no reason to report
            // that write as failed.
            LogBatchReloadFailed(logger, exception);
            return;
        }

        // The user can change the batch while the read runs, so the blueprints are swapped into the batch as it is
        // now. Rebuilding it from the ids read above would undo those changes, and an entry added during the read
        // was not asked for and is left alone.
        foreach (BlueprintQuantity entry in _blueprintMap.BlueprintList.ToList())
        {
            if (reloaded.TryGetValue(entry.Blueprint.Id, out BlueprintModel? blueprint))
            {
                entry.Blueprint = blueprint;
            }
            else if (ids.Contains(entry.Blueprint.Id))
            {
                _blueprintMap.Remove(entry);
            }
        }

        Recalculate();
    }

    public Task<bool> FavoriteExistsAsync(string? name) => Task.Run(() => favoriteService.DoesFavoriteExistAsync(name));

    /// <summary>
    /// Saves the batch as the favorite named <paramref name="name"/>, replacing a favorite that already has the name,
    /// and makes it <see cref="LoadedFavorite"/>.
    /// </summary>
    public Task SaveAsFavoriteAsync(string name) => SaveBatchAsAsync(new BlueprintFavorite { Name = name });

    /// <summary>Saves the batch over <see cref="LoadedFavorite"/>. Does nothing when no favorite is loaded.</summary>
    public async Task UpdateLoadedFavoriteAsync()
    {
        if (LoadedFavorite is { } loaded)
        {
            await SaveBatchAsAsync(loaded);
        }
    }

    private async Task SaveBatchAsAsync(BlueprintFavorite favorite)
    {
        // Copied here, on the UI thread, so the save never enumerates the batch while a tap is changing it.
        Batch batch = new([.. _blueprintMap.BlueprintList], new HashSet<int>(_alreadyCraftedIds));
        int clearCount = _clearCount;
        BlueprintFavorite saved = await Task.Run(() => favoriteService.SaveFavoriteAsync(favorite, batch));

        // The favorite is saved either way; only a batch that is still the one saved gets to call it loaded.
        if (clearCount == _clearCount)
        {
            LoadedFavorite = saved;
        }
    }

    /// <summary>Works the batch out again under the selected dataset's settings, after they have changed.</summary>
    public void OnDatasettingsChanged() => Recalculate();

    /// <summary>Keeps <see cref="LoadedFavorite"/> in step when the favorite with <paramref name="id"/> is renamed.</summary>
    public void OnFavoriteRenamed(int id, string newName)
    {
        if (LoadedFavorite?.Id != id)
        {
            return;
        }

        LoadedFavorite = new BlueprintFavorite { Id = id, Name = newName };
        Changed?.Invoke();
    }

    /// <summary>Keeps <see cref="LoadedFavorite"/> in step when the favorite with <paramref name="id"/> is deleted.</summary>
    public void OnFavoriteDeleted(int id)
    {
        if (LoadedFavorite?.Id != id)
        {
            return;
        }

        LoadedFavorite = null;
        Changed?.Invoke();
    }

    /// <summary>
    /// Whether a step currently visible in the tree counts crafts rather than items - what the
    /// asterisk legend under the Crafting Steps tree explains.
    /// </summary>
    public bool HasVisibleCraftCountedStep { get; private set; }

    /// <summary>
    /// Whether a step currently visible in the tree is a blueprint the batch takes as already crafted, which the
    /// legend under the Crafting Steps tree also explains.
    /// </summary>
    public bool HasVisibleAlreadyCraftedStep { get; private set; }

    /// <summary>
    /// The position of <paramref name="node"/> in the Crafting Steps tree, below the node at <paramref name="parentPath"/>
    /// (empty for a tree root). Two records that share a name have different paths.
    /// </summary>
    // Type and id, because components and blueprints are numbered separately.
    public static string PathOf(string parentPath, BlueprintNode node) => $"{parentPath}/{node.Source.Type}:{node.Source.Id}";

    public bool IsExpanded(string path) => _expandedPaths.Contains(path);

    public void ToggleExpanded(string path)
    {
        if (!_expandedPaths.Add(path))
        {
            _expandedPaths.Remove(path);
        }

        UpdateLegend();
    }

    public void ExpandAll()
    {
        if (!CollectPaths(TreeRoots, "", _expandedPaths))
        {
            return;
        }

        UpdateLegend();
        ExpansionChanged?.Invoke();
    }

    public void CollapseAll()
    {
        if (_expandedPaths.Count == 0)
        {
            return;
        }

        _expandedPaths.Clear();
        UpdateLegend();
        ExpansionChanged?.Invoke();
    }

    private void UpdateLegend()
    {
        // Collapsed subtrees are excluded deliberately: the legend would otherwise explain a row that is not on screen.
        // With nothing marked there's no already crafted row to find, so the walk can stop at the first asterisk.
        LegendScan scan = new() { SkipAlreadyCrafted = _alreadyCraftedIds.Count == 0 };
        ScanVisible(TreeRoots, "", ref scan);

        if (scan.CraftCounted == HasVisibleCraftCountedStep && scan.AlreadyCrafted == HasVisibleAlreadyCraftedStep)
        {
            return;
        }

        HasVisibleCraftCountedStep = scan.CraftCounted;
        HasVisibleAlreadyCraftedStep = scan.AlreadyCrafted;
        LegendChanged?.Invoke();
    }

    private void ScanVisible(IReadOnlyList<BlueprintNode> nodes, string parentPath, ref LegendScan scan)
    {
        foreach (BlueprintNode node in nodes)
        {
            scan.CraftCounted |= BlueprintProcessor.CountsByCraft(node);
            scan.AlreadyCrafted |= node.AlreadyCrafted;
            if (scan.Done)
            {
                return;
            }

            string path = PathOf(parentPath, node);
            if (!IsExpanded(path))
            {
                continue;
            }

            ScanVisible(node.Children, path, ref scan);
            if (scan.Done)
            {
                return;
            }
        }
    }

    private struct LegendScan
    {
        public bool CraftCounted;
        public bool AlreadyCrafted;
        public bool SkipAlreadyCrafted;

        public readonly bool Done => CraftCounted && (AlreadyCrafted || SkipAlreadyCrafted);
    }

    // Returns whether any path was added, which is false when the tree was already fully expanded.
    private static bool CollectPaths(IReadOnlyList<BlueprintNode> nodes, string parentPath, HashSet<string> into)
    {
        bool added = false;

        foreach (BlueprintNode node in nodes)
        {
            string path = PathOf(parentPath, node);
            added |= into.Add(path);
            added |= CollectPaths(node.Children, path, into);
        }

        return added;
    }

    // Drops the paths of rows that are no longer in the tree - a removed or cleared batch entry, a replaced batch, a part
    // taken out of a blueprint - so the set only ever describes the tree on screen. A path under a collapsed row stays:
    // that row opens with its children as they were.
    private void PruneExpandedPaths()
    {
        if (_expandedPaths.Count == 0)
        {
            return;
        }

        HashSet<string> present = [];
        CollectPaths(TreeRoots, "", present);
        _expandedPaths.IntersectWith(present);
    }

    // Keeps the marks to blueprints nested below the batch's entries, and works out which of those are only reached
    // through a marked one. An entry is never marked, and nothing outside the batch stays marked, which is what clears
    // the marks once the batch is empty.
    private void UpdateAlreadyCrafted()
    {
        List<BlueprintModel> roots = [.. _blueprintMap.BlueprintList.Select(entry => entry.Blueprint)];

        Dictionary<int, BlueprintModel> nested = BlueprintProcessor.NestedBlueprints(roots, NoStops);
        foreach (BlueprintModel root in roots)
        {
            nested.Remove(root.Id);
        }

        _alreadyCraftedIds.IntersectWith(nested.Keys);

        // Without a mark nothing is cut off, so there's no second walk to make.
        _insideAlreadyCrafted = _alreadyCraftedIds.Count == 0
            ? []
            : [.. nested.Keys.Except(BlueprintProcessor.NestedBlueprints(roots, _alreadyCraftedIds).Keys)];

        AlreadyCraftedCandidates = [.. nested.Values.OrderBy(blueprint => blueprint.Name)];
    }

    private void Recalculate()
    {
        UpdateAlreadyCrafted();
        BatchTotals totals = BatchProcessor.CalculateTotals(
            new Batch(_blueprintMap.BlueprintList, _alreadyCraftedIds), selectedDataset.Settings);

        TotalCost = totals.TotalCost;
        TotalValue = totals.TotalValue;
        SurplusValue = totals.SurplusValue;
        TotalProductionTime = totals.TotalProductionTime;
        TotalComponents =
        [
            .. totals.Materials.ComponentList.Concat<IBaseQuantityRecord>(totals.AlreadyCrafted.BlueprintList)
                .OrderBy(quantity => quantity.Name)
        ];
        SurplusStock =
            totals.Surplus.BlueprintList.OrderBy(blueprintQuantity => blueprintQuantity.Name).ToList().AsReadOnly();
        TreeRoots = totals.Roots;
        CraftingStepCount = totals.Crafts;
        PruneExpandedPaths();

        // Computed here rather than as expression-bodied properties: each walks the whole batch, and the
        // summary card reads them on every render.
        TotalComponentCount = TotalComponents.Sum(quantity => quantity.Quantity);
        SurplusCount = SurplusStock.Sum(blueprintQuantity => blueprintQuantity.Quantity);

        UpdateLegend();
        Changed?.Invoke();
    }

    [LoggerMessage(Level = LogLevel.Error,
        Message = "The batch blueprints could not be reloaded; the batch is left as it was")]
    private static partial void LogBatchReloadFailed(ILogger logger, Exception exception);
}
