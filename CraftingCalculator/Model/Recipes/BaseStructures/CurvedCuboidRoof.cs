using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Model.Recipes.BaseStructures
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
