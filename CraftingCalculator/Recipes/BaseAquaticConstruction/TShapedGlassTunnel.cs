using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Recipes.BaseAquaticConstruction
{
    class TShapedGlassTunnel : ComplexRecipe
    {
        public TShapedGlassTunnel()
        {
            Name = "T-Shaped Glass Tunnel";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CRYSTAL_SULPHIDE, 2);
            Ingredients.Add(IngredientType.FERRITE_DUST, 25);
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
