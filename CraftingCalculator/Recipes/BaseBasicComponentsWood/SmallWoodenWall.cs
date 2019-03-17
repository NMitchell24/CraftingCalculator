using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsWood
{
    class SmallWoodenWall : Recipe
    {
        public SmallWoodenWall()
        {
            Name = "Small Wooden Wall";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
