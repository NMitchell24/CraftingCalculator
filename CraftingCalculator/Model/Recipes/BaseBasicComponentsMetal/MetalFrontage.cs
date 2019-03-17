using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsMetal
{
    class MetalFrontage : Recipe
    {
        public MetalFrontage()
        {
            Name = "Metal Frontage";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 40);
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
