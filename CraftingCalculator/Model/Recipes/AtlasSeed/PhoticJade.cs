using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AtlasSeed
{
    class PhoticJade : ComplexRecipe
    {
        public PhoticJade()
        {
            Name = "Photic Jade";
            Type = RecipeFilterLabels.AtlasSeed;
            Ingredients.Add(IngredientType.COPPER, 100);
            ChildRecipes.Add(new DawnsEnd(), 1);
        }
    }
}
