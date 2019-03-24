using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Model.Recipes.BaseAquaticConstruction
{
    class LShapedGlassTunnel : ComplexRecipe
    {
        public LShapedGlassTunnel()
        {
            Name = "L-Shaped Glass Tunnel";
            Type = RecipeFilterLabels.BaseAquaticConstruction;
            Ingredients.Add(IngredientType.CRYSTAL_SULPHIDE, 2);
            Ingredients.Add(IngredientType.FERRITE_DUST, 25);
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
