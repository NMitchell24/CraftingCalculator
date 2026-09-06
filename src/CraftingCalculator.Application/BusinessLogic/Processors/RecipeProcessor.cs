using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Flattens a recipe's (already-loaded) component graph into its combined raw ingredients, or into
/// a breakdown tree. Ports the recursive walk that used to live on the Recipe model itself
/// (Recipe.GetIngredients / Recipe.GetRecipeNodes) now that models stay dumb.
/// </summary>
public static class RecipeProcessor
{
    /// <summary>
    /// Recursion guard. The app does not otherwise detect a cycle in the recipe graph (only
    /// self-reference is prevented at edit time - see the "no cycle guard" parity issue), so an
    /// A -> B -> A pair would otherwise recurse until the stack overflows. Bounding the depth turns
    /// that into a catchable exception instead of a process-killing StackOverflowException.
    /// </summary>
    public const int MaxRecipeDepth = 64;

    public static IngredientMap Flatten(Recipe recipe) => Flatten(recipe, 0);

    private static IngredientMap Flatten(Recipe recipe, int depth)
    {
        ThrowIfTooDeep(recipe, depth);

        IngredientMap combined = new IngredientMap(recipe.Ingredients, false);

        foreach (RecipeQuantity child in recipe.ChildRecipes.RecipeList)
        {
            IngredientMap childIngredients = Flatten(child.Recipe, depth + 1);
            combined = IngredientProcessor.CombineIngredients(childIngredients, combined, child.Quantity);
        }

        return combined;
    }

    public static RecipeTree BuildTree(Recipe recipe, long quantity) => BuildTree(recipe, quantity, 0);

    private static RecipeTree BuildTree(Recipe recipe, long quantity, int depth)
    {
        ThrowIfTooDeep(recipe, depth);

        RecipeTree tree = new RecipeTree
        {
            Name = recipe.Name + " x" + quantity,
            Id = recipe.Name,
            Tooltip = recipe.Tooltip
        };

        foreach (IngredientQuantity i in recipe.Ingredients.IngredientList)
        {
            tree.AddRecipeNode(new RecipeTree(i.Name + " x" + (i.Quantity * quantity), i.Tooltip));
        }

        foreach (RecipeQuantity r in recipe.ChildRecipes.RecipeList)
        {
            tree.AddRecipeNode(BuildTree(r.Recipe, r.Quantity * quantity, depth + 1));
        }

        return tree;
    }

    private static void ThrowIfTooDeep(Recipe recipe, int depth)
    {
        if (depth > MaxRecipeDepth)
        {
            throw new InvalidOperationException(
                $"Recipe graph exceeded the maximum depth of {MaxRecipeDepth}; check for a cycle involving '{recipe.Name}'.");
        }
    }
}
