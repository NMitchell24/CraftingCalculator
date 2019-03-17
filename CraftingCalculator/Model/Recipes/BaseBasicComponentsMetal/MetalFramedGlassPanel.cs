using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsMetal
{
    class MetalFramedGlassPanel : Recipe
    {
        public MetalFramedGlassPanel()
        {
            Name = "Metal-Framed Glass Panel";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
