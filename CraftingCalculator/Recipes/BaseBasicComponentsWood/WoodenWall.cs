using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsWood
{
    class WoodenWall : Recipe
    {
        public WoodenWall()
        {
            Name = "Wooden Wall";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CARBON, 40);
        }
    }
}
