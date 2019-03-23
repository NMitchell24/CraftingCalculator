using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AtlasSeed
{
    class HeartOfTheSun : ComplexRecipe
    {
        public HeartOfTheSun()
        {
            Name = "Heart of the Sun";
            Type = RecipeFilterLabels.AtlasSeed;
            Ingredients.Add(IngredientType.GOLD, 100);
            ChildRecipes.Add(new ModifiedQuanta(), 1);
        }
    }
}
