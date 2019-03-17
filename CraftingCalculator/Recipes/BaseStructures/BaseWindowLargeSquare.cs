using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class BaseWindowLargeSquare : ComplexRecipe
    {
        public BaseWindowLargeSquare()
        {
            Name = "Large Square Window";
            Type = "Base Building";
            Ingredients.Add(IngredientType.IONISED_COBALT, 10);
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
