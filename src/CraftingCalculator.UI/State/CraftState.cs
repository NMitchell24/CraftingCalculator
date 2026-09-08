using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.UI.State;

/// <summary>
/// The batch of blueprints currently being priced out on the Craft screen. Scoped and shared across
/// pages (Favorites/Dataset can load into or read from it) - the one exception to "state rides in the
/// route", since this is genuine cross-page session state. Components subscribe to <see cref="Changed"/>
/// in <c>OnInitialized</c> and unsubscribe in <c>Dispose</c>.
/// </summary>
public sealed class CraftState(IBlueprintService blueprintService, IFavoriteService favoriteService)
{
    private readonly BlueprintMap _blueprintMap = new();

    // Keyed by each node's full path from the tree root (see BlueprintTreeNode.Path), not by BlueprintNode.Id
    // alone - the same blueprint/component can appear more than once in one tree (e.g. a blueprint used both
    // standalone in the batch and nested inside another batch blueprint), and keying by Id alone made every
    // occurrence share one expansion state instead of each position remembering its own.
    private readonly HashSet<string> _expandedPaths = [];

    public event Action? Changed;

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
    public string? LoadedFavoriteName { get; private set; }

    public void AddBlueprints(IEnumerable<Blueprint> blueprints)
    {
        foreach (Blueprint blueprint in blueprints)
        {
            _blueprintMap.Add(blueprint, 1);
        }

        Recalculate();
    }

    /// <summary>
    /// Sets the blueprint's quantity in the batch. Zero is a valid quantity that keeps the blueprint
    /// selected; only a negative quantity removes it, which the stepper reaches by decrementing past
    /// zero.
    /// </summary>
    public void SetQuantity(BlueprintQuantity target, long quantity)
    {
        if (quantity < 0)
        {
            _blueprintMap.RemoveAll(target.Blueprint);
        }
        else
        {
            target.Quantity = quantity;
        }

        Recalculate();
    }

    public void Remove(BlueprintQuantity target)
    {
        _blueprintMap.RemoveAll(target.Blueprint);
        Recalculate();
    }

    public void Clear()
    {
        _blueprintMap.Reset();
        LoadedFavoriteName = null;
        Recalculate();
    }

    public async Task LoadFavoriteAsync(BlueprintFavorite favorite)
    {
        _blueprintMap.Reset();

        foreach (BlueprintQuantity quantity in await favoriteService.GetBlueprintQuantitiesForFavoriteAsync(favorite))
        {
            _blueprintMap.Add(quantity.Blueprint, quantity.Quantity);
        }

        LoadedFavoriteName = favorite.Name;
        Recalculate();
    }

    public Task<bool> FavoriteExistsAsync(string? name) => favoriteService.DoesFavoriteExistAsync(name);

    public async Task SaveAsFavoriteAsync(string name)
    {
        await favoriteService.SaveFavoriteAsync(new BlueprintFavorite { Name = name }, [.. _blueprintMap.BlueprintList]);
        LoadedFavoriteName = name;
    }

    /// <summary>Keeps <see cref="LoadedFavoriteName"/> in step when the loaded favorite is renamed.</summary>
    public void OnFavoriteRenamed(string previousName, string newName)
    {
        if (LoadedFavoriteName != previousName)
        {
            return;
        }

        LoadedFavoriteName = newName;
        Changed?.Invoke();
    }

    /// <summary>Keeps <see cref="LoadedFavoriteName"/> in step when the loaded favorite is deleted.</summary>
    public void OnFavoriteDeleted(string name)
    {
        if (LoadedFavoriteName != name)
        {
            return;
        }

        LoadedFavoriteName = null;
        Changed?.Invoke();
    }

    public bool IsExpanded(string path) => _expandedPaths.Contains(path);

    public void ToggleExpanded(string path)
    {
        if (!_expandedPaths.Add(path))
        {
            _expandedPaths.Remove(path);
        }

        Changed?.Invoke();
    }

    public void ExpandAll()
    {
        CollectPaths(TreeRoots, "", _expandedPaths);
        Changed?.Invoke();
    }

    public void CollapseAll()
    {
        _expandedPaths.Clear();
        Changed?.Invoke();
    }

    private static long CountCrafts(IReadOnlyList<BlueprintNode> nodes) =>
        nodes.Sum(node => node.Crafts + CountCrafts(node.Children));

    private static void CollectPaths(IReadOnlyList<BlueprintNode> nodes, string parentPath, HashSet<string> into)
    {
        foreach (BlueprintNode node in nodes)
        {
            string path = $"{parentPath}/{node.Id ?? node.Name}";
            into.Add(path);
            CollectPaths(node.Children, path, into);
        }
    }

    private void Recalculate()
    {
        BatchTotals totals = BatchProcessor.CalculateTotals(_blueprintMap.BlueprintList);

        TotalCost = totals.TotalCost;
        TotalValue = totals.TotalValue;
        TotalProductionTime = totals.TotalProductionTime;
        TotalComponents = [.. totals.Materials.ComponentList.OrderBy(componentQuantity => componentQuantity.Name)];
        SurplusStock = [.. totals.Surplus.BlueprintList.OrderBy(blueprintQuantity => blueprintQuantity.Name)];
        TreeRoots = [.. _blueprintMap.BlueprintList.Select(blueprintQuantity => blueprintService.GetBlueprintNode(blueprintQuantity.Blueprint, blueprintQuantity.Quantity))];

        // Computed here rather than as expression-bodied properties: each walks the whole batch, and the
        // summary card reads them on every render.
        TotalComponentCount = TotalComponents.Sum(componentQuantity => componentQuantity.Quantity);
        CraftingStepCount = CountCrafts(TreeRoots);
        SurplusCount = SurplusStock.Sum(blueprintQuantity => blueprintQuantity.Quantity);
        SurplusValue = SurplusStock.Sum(blueprintQuantity => blueprintQuantity.TotalValue);

        Changed?.Invoke();
    }
}
