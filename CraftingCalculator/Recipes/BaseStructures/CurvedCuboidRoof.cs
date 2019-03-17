using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class CurvedCuboidRoof : ComplexRecipe
    {
        public CurvedCuboidRoof()
        {
            Name = "Curved Cuboid Roof";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 100);
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
