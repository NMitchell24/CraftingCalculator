using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsMetal
{
    class MetalRoofPanel : Recipe
    {
        public MetalRoofPanel()
        {
            Name = "Metal Roof Panel";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CARBON, 20);
            Ingredients.Add(IngredientType.FERRITE_DUST, 10);
        }
    }
}
