using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsMetal
{
    class MetalHalfRamp : Recipe
    {
        public MetalHalfRamp()
        {
            Name = "Metal Half Ramp";
            Type = RecipeFilterLabels.BaseComponentsMetal;
            Ingredients.Add(IngredientType.FERRITE_DUST, 25);
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
        }
    }
}
