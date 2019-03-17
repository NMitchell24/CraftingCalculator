using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class BaseWindowSmallRectangle : ComplexRecipe
    {
        public BaseWindowSmallRectangle()
        {
            Name = "Small Rectangular Window";
            Type = "Base Building";
            Ingredients.Add(IngredientType.IONISED_COBALT, 5);
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
