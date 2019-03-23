using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class GlassRoofedCorridor : ComplexRecipe
    {
        public GlassRoofedCorridor()
        {
            Name = "Glass Roofed Cooridor";
            Type = RecipeFilterLabels.BaseStructures;
            Ingredients.Add(IngredientType.PURE_FERRITE, 100);
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
