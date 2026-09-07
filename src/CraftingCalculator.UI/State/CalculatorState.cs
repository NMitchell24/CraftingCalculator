using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.UI.State;

/// <summary>
/// The batch of recipes currently being priced out on the Calculate screen. Scoped and shared across
/// pages (Favorites/Library can load into or read from it) - the one exception to "state rides in the
/// route", since this is genuine cross-page session state. Components subscribe to <see cref="Changed"/>
/// in <c>OnInitialized</c> and unsubscribe in <c>Dispose</c>.
/// </summary>
public sealed class CalculatorState(IRecipeService recipeService, IFavoriteService favoriteService)
{
    private readonly RecipeMap _recipeMap = new();

    // Keyed by each node's full path from the tree root (see RecipeTreeNode.Path), not by RecipeNode.Id
    // alone - the same recipe/ingredient can appear more than once in one tree (e.g. a recipe used both
    // standalone in the batch and nested inside another batch recipe), and keying by Id alone made every
    // occurrence share one expansion state instead of each position remembering its own.
    private readonly HashSet<string> _expandedPaths = [];

    public event Action? Changed;

    public IReadOnlyList<RecipeQuantity> RecipeQuantities => _recipeMap.RecipeList;
    public IReadOnlyList<IngredientQuantity> TotalIngredients { get; private set; } = [];
    public IReadOnlyList<RecipeNode> TreeRoots { get; private set; } = [];
    public double TotalCost { get; private set; }
    public double TotalValue { get; private set; }
    public double Profit => TotalValue - TotalCost;

    /// <summary>Total units of raw material the batch needs, summed across every distinct ingredient.</summary>
    public long TotalComponentCount { get; private set; }

    /// <summary>
    /// How many craft operations the breakdown implies - every recipe in the tree, at every depth.
    /// Ingredient leaves are not steps: they are gathered, not crafted.
    /// </summary>
    public int CraftingStepCount { get; private set; }

    /// <summary>
    /// The favorite the current batch came from, or null when it was built by hand or cleared. Drives
    /// the "update or create new" branch of the save flow (see Components/Dialogs/FavoritePrompts).
    /// </summary>
    public string? LoadedFavoriteName { get; private set; }

    public void AddRecipes(IEnumerable<Recipe> recipes)
    {
        foreach (Recipe recipe in recipes)
        {
            _recipeMap.Add(recipe, 1);
        }

        Recalculate();
    }

    /// <summary>Setting a quantity of 0 or less removes the recipe from the batch entirely.</summary>
    public void SetQuantity(RecipeQuantity target, long quantity)
    {
        if (quantity <= 0)
        {
            _recipeMap.RemoveAll(target.Recipe);
        }
        else
        {
            target.Quantity = quantity;
        }

        Recalculate();
    }

    public void Remove(RecipeQuantity target)
    {
        _recipeMap.RemoveAll(target.Recipe);
        Recalculate();
    }

    public void Clear()
    {
        _recipeMap.Reset();
        LoadedFavoriteName = null;
        Recalculate();
    }

    public async Task LoadFavoriteAsync(RecipeFavorite favorite)
    {
        _recipeMap.Reset();

        foreach (RecipeQuantity quantity in await favoriteService.GetRecipeQuantitiesForFavoriteAsync(favorite))
        {
            _recipeMap.Add(quantity.Recipe, quantity.Quantity);
        }

        LoadedFavoriteName = favorite.Name;
        Recalculate();
    }

    public Task<bool> FavoriteExistsAsync(string? name) => favoriteService.DoesFavoriteExistAsync(name);

    public async Task SaveAsFavoriteAsync(string name)
    {
        await favoriteService.SaveFavoriteAsync(new RecipeFavorite { Name = name }, [.. _recipeMap.RecipeList]);
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

    private static int CountRecipes(IReadOnlyList<RecipeNode> nodes) =>
        nodes.Sum(node => (node.IsIngredient ? 0 : 1) + CountRecipes(node.Children));

    private static void CollectPaths(IReadOnlyList<RecipeNode> nodes, string parentPath, HashSet<string> into)
    {
        foreach (RecipeNode node in nodes)
        {
            string path = $"{parentPath}/{node.Id ?? node.Name}";
            into.Add(path);
            CollectPaths(node.Children, path, into);
        }
    }

    private void Recalculate()
    {
        (double totalCost, double totalValue, IngredientMap materials) = BatchProcessor.CalculateTotals(_recipeMap.RecipeList);

        TotalCost = totalCost;
        TotalValue = totalValue;
        TotalIngredients = [.. materials.IngredientList.OrderBy(i => i.Name)];
        TreeRoots = [.. _recipeMap.RecipeList.Select(rq => recipeService.GetRecipeNode(rq.Recipe, rq.Quantity))];

        // Computed here rather than as expression-bodied properties: both walk the whole batch, and the
        // summary card reads them on every render.
        TotalComponentCount = TotalIngredients.Sum(i => i.Quantity);
        CraftingStepCount = CountRecipes(TreeRoots);

        Changed?.Invoke();
    }
}
