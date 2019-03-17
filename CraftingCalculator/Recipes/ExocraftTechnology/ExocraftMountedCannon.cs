using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.ExocraftTechnology
{
    class ExocraftMountedCannon : Recipe
    {
        public ExocraftMountedCannon()
        {
            Name = "Exocraft Mounted Cannon";
            Type = "Exocraft Weapon Module";
            Ingredients.Add(IngredientType.PUGNEUM, 50);
            Ingredients.Add(IngredientType.COPPER, 100);
        }
    }
}
