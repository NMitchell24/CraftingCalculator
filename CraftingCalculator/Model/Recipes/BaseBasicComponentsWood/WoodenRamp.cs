using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsWood
{
    class WoodenRamp : Recipe
    {
        public WoodenRamp()
        {
            Name = "Wooden Ramp";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CARBON, 50);
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
