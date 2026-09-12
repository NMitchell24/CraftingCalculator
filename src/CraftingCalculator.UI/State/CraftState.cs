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

    // Keyed by each node's full path from the tree root (see BlueprintTreeNode.Path), not by name
    // alone - the same blueprint/component can appear more than once in one tree (e.g. a blueprint used both
    // standalone in the batch and nested inside another batch blueprint), and keying by name alone made every
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

    public void AddBlueprints(IEnumerable<BlueprintModel> blueprints)
    {
        foreach (BlueprintModel blueprint in blueprints)
        {
            _blueprintMap.Add(blueprint, 1);
        }

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
    /// batch.
    /// </summary>
    public void Step(BlueprintQuantity target, long step)
    {
        // Zero is the landing every step down passes through, so the step that starts there is a
        // deliberate second tap rather than an overshoot. That is what lets a step of any size clamp
        // without losing the remove gesture: -10 against a quantity of 4 settles on zero instead of
        // dropping the blueprint out of the batch on one tap.
        if (step < 0 && target.Quantity == 0)
        {
            Remove(target);
            return;
        }

        SetQuantity(target, Math.Max(target.Quantity + step, 0));
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

    /// <summary>
    /// Whether a step currently visible in the tree counts crafts rather than items - what the
    /// asterisk legend under the Crafting Steps tree explains.
    /// </summary>
    // Collapsed subtrees are excluded deliberately: the legend would otherwise resolve an asterisk that
    // is not on screen.
    public bool HasVisibleCraftCountedStep => AnyCountsByCraft(TreeRoots, "");

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

    private bool AnyCountsByCraft(IReadOnlyList<BlueprintNode> nodes, string parentPath)
    {
        foreach (BlueprintNode node in nodes)
        {
            if (BlueprintProcessor.CountsByCraft(node))
            {
                return true;
            }

            string path = $"{parentPath}/{node.Name}";
            if (IsExpanded(path) && AnyCountsByCraft(node.Children, path))
            {
                return true;
            }
        }

        return false;
    }

    private static long CountCrafts(IReadOnlyList<BlueprintNode> nodes) =>
        nodes.Sum(node => node.Crafts + CountCrafts(node.Children));

    private static void CollectPaths(IReadOnlyList<BlueprintNode> nodes, string parentPath, HashSet<string> into)
    {
        foreach (BlueprintNode node in nodes)
        {
            string path = $"{parentPath}/{node.Name}";
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
