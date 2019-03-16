using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.AtlasSeed
{
    class PhoticJade : ComplexRecipe
    {
        public PhoticJade()
        {
            Name = "Photic Jade";
            Type = "Atlas Seed";
            Ingredients.Add(IngredientType.COPPER, 100);
            ChildRecipes.Add(new DawnsEnd(), 1);
        }
    }
}
