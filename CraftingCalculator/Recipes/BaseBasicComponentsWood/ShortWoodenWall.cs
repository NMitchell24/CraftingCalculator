using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsWood
{
    class ShortWoodenWall : Recipe
    {
        public ShortWoodenWall()
        {
            Name = "Short Wooden Wall";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
