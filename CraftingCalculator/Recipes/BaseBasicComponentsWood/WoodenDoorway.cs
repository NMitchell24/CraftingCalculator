using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsWood
{
    class WoodenDoorway : Recipe
    {
        public WoodenDoorway()
        {
            Name = "Wooden Doorway";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
