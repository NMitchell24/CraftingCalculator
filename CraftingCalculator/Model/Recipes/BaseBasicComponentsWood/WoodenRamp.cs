using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsWood
{
    class WoodenRamp : Recipe
    {
        public WoodenRamp()
        {
            Name = "Wooden Ramp";
            Type = RecipeFilterLabels.BaseComponentsWood;
            Ingredients.Add(IngredientType.CARBON, 50);
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
