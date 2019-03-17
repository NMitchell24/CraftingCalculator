using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsWood
{
    class ThinWoodenWall : Recipe
    {
        public ThinWoodenWall()
        {
            Name = "Thin Wooden Wall";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
