using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsWood
{
    class WoodFramedGlassPanel : Recipe
    {
        public WoodFramedGlassPanel()
        {
            Name = "Wood-Framed Glass Panel";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
