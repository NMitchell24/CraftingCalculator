using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsWood
{
    class WoodenFrontage : Recipe
    {
        public WoodenFrontage()
        {
            Name = "Wooden Frontage";
            Type = RecipeFilterLabels.BaseComponentsWood;
            Ingredients.Add(IngredientType.CARBON, 40);
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
