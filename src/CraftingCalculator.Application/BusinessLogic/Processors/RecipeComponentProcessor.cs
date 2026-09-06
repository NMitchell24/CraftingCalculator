using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Edits the components of a single recipe - its own ingredients plus the child recipes nested
/// inside it - as one collection, so callers do not have to know which of
/// <see cref="Recipe.Ingredients"/> or <see cref="Recipe.ChildRecipes"/> a given component lives in.
/// </summary>
public static class RecipeComponentProcessor
{
    /// <summary>
    /// The recipe's ingredients and child recipes as one list, ingredients first and each group
    /// ordered by name.
    /// </summary>
    public static List<IBaseQuantityRecord> GetComponents(Recipe recipe)
    {
        List<IBaseQuantityRecord> components = [.. recipe.Ingredients.IngredientList];
        components.AddRange(recipe.ChildRecipes.RecipeList);

        return [.. components.OrderBy(c => c.Type).ThenBy(c => c.Name)];
    }

    /// <summary>
    /// Adds the component to the recipe, or raises the quantity of the matching component already
    /// on it by <paramref name="quantity"/>.
    /// </summary>
    public static void Add(Recipe recipe, IBaseDataRecord? component, long quantity)
    {
        switch (component)
        {
            case Ingredient ingredient:
                recipe.Ingredients.Add(ingredient, quantity);
                break;
            case Recipe child:
                recipe.ChildRecipes.Add(child, quantity);
                break;
        }
    }

    /// <summary>
    /// Sets the component's quantity on the recipe. A quantity of 0 or less removes the component
    /// entirely.
    /// </summary>
    public static void SetQuantity(Recipe recipe, IBaseQuantityRecord component, long quantity)
    {
        if (quantity <= 0)
        {
            Remove(recipe, component);
            return;
        }

        component.Quantity = quantity;
    }

    /// <summary>Removes the component from the recipe.</summary>
    public static void Remove(Recipe recipe, IBaseQuantityRecord component)
    {
        // RemoveAll rather than a quantity of 0: it is what pushes the component onto the map's
        // RemovedIngredients/RemovedRecipes list, which is the only signal RecipeDAO.SaveAsync has
        // that the underlying join row should be deleted.
        switch (component)
        {
            case IngredientQuantity ingredientQuantity:
                recipe.Ingredients.RemoveAll(ingredientQuantity.Ingredient);
                break;
            case RecipeQuantity recipeQuantity:
                recipe.ChildRecipes.RemoveAll(recipeQuantity.Recipe);
                break;
        }
    }
}
