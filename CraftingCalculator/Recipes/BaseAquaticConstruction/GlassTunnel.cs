using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Recipes.BaseAquaticConstruction
{
    class GlassTunnel : ComplexRecipe
    {
        public GlassTunnel()
        {
            Name = "Glass Tunnel";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CRYSTAL_SULPHIDE, 2);
            Ingredients.Add(IngredientType.FERRITE_DUST, 25);
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
