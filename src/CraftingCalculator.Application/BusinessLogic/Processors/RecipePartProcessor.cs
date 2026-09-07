using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Edits the parts of a single recipe - its own components plus the child recipes nested inside it -
/// as one collection, so callers do not have to know which of <see cref="Recipe.Components"/> or
/// <see cref="Recipe.ChildRecipes"/> a given part lives in.
/// </summary>
public static class RecipePartProcessor
{
    /// <summary>
    /// The recipe's components and child recipes as one list, components first and each group
    /// ordered by name.
    /// </summary>
    public static List<IBaseQuantityRecord> GetParts(Recipe recipe)
    {
        List<IBaseQuantityRecord> parts = [.. recipe.Components.ComponentList];
        parts.AddRange(recipe.ChildRecipes.RecipeList);

        return [.. parts.OrderBy(p => p.Type).ThenBy(p => p.Name)];
    }

    /// <summary>
    /// Adds the part to the recipe, or raises the quantity of the matching part already on it by
    /// <paramref name="quantity"/>.
    /// </summary>
    public static void Add(Recipe recipe, IBaseDataRecord? part, long quantity)
    {
        switch (part)
        {
            case Component component:
                recipe.Components.Add(component, quantity);
                break;
            case Recipe child:
                recipe.ChildRecipes.Add(child, quantity);
                break;
        }
    }

    /// <summary>
    /// Sets the part's quantity on the recipe. A quantity of 0 or less removes the part entirely.
    /// </summary>
    public static void SetQuantity(Recipe recipe, IBaseQuantityRecord part, long quantity)
    {
        if (quantity <= 0)
        {
            Remove(recipe, part);
            return;
        }

        part.Quantity = quantity;
    }

    /// <summary>Removes the part from the recipe.</summary>
    public static void Remove(Recipe recipe, IBaseQuantityRecord part)
    {
        // RemoveAll rather than a quantity of 0: it is what pushes the part onto the map's
        // RemovedComponents/RemovedRecipes list, which is the only signal RecipeDAO.SaveAsync has
        // that the underlying join row should be deleted.
        switch (part)
        {
            case ComponentQuantity componentQuantity:
                recipe.Components.RemoveAll(componentQuantity.Component);
                break;
            case RecipeQuantity recipeQuantity:
                recipe.ChildRecipes.RemoveAll(recipeQuantity.Recipe);
                break;
        }
    }
}
