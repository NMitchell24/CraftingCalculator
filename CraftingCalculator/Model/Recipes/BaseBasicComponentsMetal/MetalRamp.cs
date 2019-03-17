using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsMetal
{
    class MetalRamp : Recipe
    {
        public MetalRamp()
        {
            Name = "Metal Ramp";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 50);
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
