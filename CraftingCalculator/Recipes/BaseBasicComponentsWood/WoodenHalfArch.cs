using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsWood
{
    class WoodenHalfArch : Recipe
    {
        public WoodenHalfArch()
        {
            Name = "Wooden Half Arch";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
