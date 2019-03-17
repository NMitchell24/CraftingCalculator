using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsMetal
{
    class MetalHalfRamp : Recipe
    {
        public MetalHalfRamp()
        {
            Name = "Metal Half Ramp";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 25);
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
        }
    }
}
