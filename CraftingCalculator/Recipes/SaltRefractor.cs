using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class SaltRefractor : Recipe
    {
        public SaltRefractor()
        {
            Name = "Salt Refractor";
            Type = "Component";
            Ingredients.Add(IngredientType.SALT, 100);
        }
    }
}
