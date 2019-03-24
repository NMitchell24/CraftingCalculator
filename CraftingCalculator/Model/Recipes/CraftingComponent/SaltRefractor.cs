using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.CraftingComponent
{
    class SaltRefractor : Recipe
    {
        public SaltRefractor()
        {
            Name = "Salt Refractor";
            Type = RecipeFilterLabels.CraftingComponents;
            Ingredients.Add(IngredientType.SALT, 100);
        }
    }
}
