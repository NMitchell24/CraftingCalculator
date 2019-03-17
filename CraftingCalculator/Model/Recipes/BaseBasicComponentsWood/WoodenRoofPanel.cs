using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsWood
{
    class WoodenRoofPanel : Recipe
    {
        public WoodenRoofPanel()
        {
            Name = "Wooden Roof Panel";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CARBON, 20);
            Ingredients.Add(IngredientType.FERRITE_DUST, 10);
        }
    }
}
