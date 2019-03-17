using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsWood
{
    class WoodFloorPanel : Recipe
    {
        public WoodFloorPanel()
        {
            Name = "Wood Floor Panel";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
