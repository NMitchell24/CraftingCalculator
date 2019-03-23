using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsWood
{
    class WoodenRoofCorner : Recipe
    {
        public WoodenRoofCorner()
        {
            Name = "Wooden Roof Corner";
            Type = RecipeFilterLabels.BaseComponentsWood;
            Ingredients.Add(IngredientType.CARBON, 20);
            Ingredients.Add(IngredientType.FERRITE_DUST, 10);
        }
    }
}
