using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsWood
{
    class WoodenDoorFrame : Recipe
    {
        public WoodenDoorFrame()
        {
            Name = "Wooden Door Frame";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CARBON, 40);
        }
    }
}
