using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class GlassRoofedCorridor : ComplexRecipe
    {
        public GlassRoofedCorridor()
        {
            Name = "Glass Roofed Cooridor";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 100);
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
