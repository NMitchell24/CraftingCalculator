using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.CraftingComponent
{
    class SaltRefractor : Recipe
    {
        public SaltRefractor()
        {
            Name = "Salt Refractor";
            Type = "Crafting Component";
            Ingredients.Add(IngredientType.SALT, 100);
        }
    }
}
