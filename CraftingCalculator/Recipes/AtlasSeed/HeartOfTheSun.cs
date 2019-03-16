using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.AtlasSeed
{
    class HeartOfTheSun : ComplexRecipe
    {
        public HeartOfTheSun()
        {
            Name = "Heart of the Sun";
            Type = "Atlas Seed";
            Ingredients.Add(IngredientType.GOLD, 100);
            ChildRecipes.Add(new ModifiedQuanta(), 1);
        }
    }
}
