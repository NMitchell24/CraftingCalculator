using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Model.Recipes.BaseStructures
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
