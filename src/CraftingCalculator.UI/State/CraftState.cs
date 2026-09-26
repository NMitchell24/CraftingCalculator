using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
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
    private readonly BlueprintMap _blueprintMap = new();

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

    /// <summary>Raised when <see cref="HasVisibleCraftCountedStep"/> changes, whatever changed it.</summary>
    public event Action? HasVisibleCraftCountedStepChanged;

    public IReadOnlyList<BlueprintQuantity> BlueprintQuantities => _blueprintMap.BlueprintList;
    public IReadOnlyList<ComponentQuantity> TotalComponents { get; private set; } = [];
    public IReadOnlyList<BlueprintNode> TreeRoots { get; private set; } = [];
    public double TotalCost { get; private set; }
    public double TotalValue { get; private set; }
    public double Profit => TotalValue - TotalCost;

    /// <summary>Total units of raw material the batch needs, summed across every distinct component.</summary>
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
    public IReadOnlyList<BlueprintQuantity> SurplusStock { get; private set; } = [];

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
        List<BlueprintQuantity> quantities =
            await Task.Run(() => favoriteService.GetBlueprintQuantitiesForFavoriteAsync(favorite));

        if (clearCount != _clearCount)
        {
            return false;
        }

        _blueprintMap.Reset();

        foreach (BlueprintQuantity quantity in quantities)
        {
            _blueprintMap.Add(quantity.Blueprint, quantity.Quantity);
        }

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
        List<BlueprintQuantity> batch = [.. _blueprintMap.BlueprintList];
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

        UpdateHasVisibleCraftCountedStep();
    }

    public void ExpandAll()
    {
        CollectPaths(TreeRoots, "", _expandedPaths);
        UpdateHasVisibleCraftCountedStep();
        ExpansionChanged?.Invoke();
    }

    public void CollapseAll()
    {
        _expandedPaths.Clear();
        UpdateHasVisibleCraftCountedStep();
        ExpansionChanged?.Invoke();
    }

    private void UpdateHasVisibleCraftCountedStep()
    {
        // Collapsed subtrees are excluded deliberately: the legend would otherwise resolve an asterisk that
        // is not on screen.
        bool visible = AnyCountsByCraft(TreeRoots, "");

        if (visible == HasVisibleCraftCountedStep)
        {
            return;
        }

        HasVisibleCraftCountedStep = visible;
        HasVisibleCraftCountedStepChanged?.Invoke();
    }

    private bool AnyCountsByCraft(IReadOnlyList<BlueprintNode> nodes, string parentPath)
    {
        foreach (BlueprintNode node in nodes)
        {
            if (BlueprintProcessor.CountsByCraft(node))
            {
                return true;
            }

            string path = PathOf(parentPath, node);
            if (IsExpanded(path) && AnyCountsByCraft(node.Children, path))
            {
                return true;
            }
        }

        return false;
    }

    private static void CollectPaths(IReadOnlyList<BlueprintNode> nodes, string parentPath, HashSet<string> into)
    {
        foreach (BlueprintNode node in nodes)
        {
            string path = PathOf(parentPath, node);
            into.Add(path);
            CollectPaths(node.Children, path, into);
        }
    }

    private void Recalculate()
    {
        BatchTotals totals = BatchProcessor.CalculateTotals(_blueprintMap.BlueprintList, selectedDataset.Settings);

        TotalCost = totals.TotalCost;
        TotalValue = totals.TotalValue;
        SurplusValue = totals.SurplusValue;
        TotalProductionTime = totals.TotalProductionTime;
        TotalComponents = [.. totals.Materials.ComponentList.OrderBy(componentQuantity => componentQuantity.Name)];
        SurplusStock = [.. totals.Surplus.BlueprintList.OrderBy(blueprintQuantity => blueprintQuantity.Name)];
        TreeRoots = totals.Roots;
        CraftingStepCount = totals.Crafts;

        // Computed here rather than as expression-bodied properties: each walks the whole batch, and the
        // summary card reads them on every render.
        TotalComponentCount = TotalComponents.Sum(componentQuantity => componentQuantity.Quantity);
        SurplusCount = SurplusStock.Sum(blueprintQuantity => blueprintQuantity.Quantity);

        UpdateHasVisibleCraftCountedStep();
        Changed?.Invoke();
    }

    [LoggerMessage(Level = LogLevel.Error,
        Message = "The batch blueprints could not be reloaded; the batch is left as it was")]
    private static partial void LogBatchReloadFailed(ILogger logger, Exception exception);
}
