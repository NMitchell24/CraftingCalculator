using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsWood
{
    class WoodenHalfRamp : Recipe
    {
        public WoodenHalfRamp()
        {
            Name = "Wooden Half Ramp";
            Type = RecipeFilterLabels.BaseComponentsWood;
            Ingredients.Add(IngredientType.CARBON, 25);
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
        }
    }
}
