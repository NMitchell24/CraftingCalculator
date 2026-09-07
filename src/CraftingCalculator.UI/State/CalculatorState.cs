using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.UI.State;

/// <summary>
/// The batch of blueprints currently being priced out on the Calculate screen. Scoped and shared across
/// pages (Favorites/Library can load into or read from it) - the one exception to "state rides in the
/// route", since this is genuine cross-page session state. Components subscribe to <see cref="Changed"/>
/// in <c>OnInitialized</c> and unsubscribe in <c>Dispose</c>.
/// </summary>
public sealed class CalculatorState(IBlueprintService blueprintService, IFavoriteService favoriteService)
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
    /// How many craft operations the breakdown implies - every blueprint in the tree, at every depth.
    /// Component leaves are not steps: they are gathered, not crafted.
    /// </summary>
    public int CraftingStepCount { get; private set; }

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

    /// <summary>Setting a quantity of 0 or less removes the blueprint from the batch entirely.</summary>
    public void SetQuantity(BlueprintQuantity target, long quantity)
    {
        if (quantity <= 0)
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

    private static int CountBlueprints(IReadOnlyList<BlueprintNode> nodes) =>
        nodes.Sum(node => (node.IsComponent ? 0 : 1) + CountBlueprints(node.Children));

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
        (double totalCost, double totalValue, ComponentMap materials) = BatchProcessor.CalculateTotals(_blueprintMap.BlueprintList);

        TotalCost = totalCost;
        TotalValue = totalValue;
        TotalComponents = [.. materials.ComponentList.OrderBy(i => i.Name)];
        TreeRoots = [.. _blueprintMap.BlueprintList.Select(rq => blueprintService.GetBlueprintNode(rq.Blueprint, rq.Quantity))];

        // Computed here rather than as expression-bodied properties: both walk the whole batch, and the
        // summary card reads them on every render.
        TotalComponentCount = TotalComponents.Sum(i => i.Quantity);
        CraftingStepCount = CountBlueprints(TreeRoots);

        Changed?.Invoke();
    }
}
