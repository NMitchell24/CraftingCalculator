using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class BaseWindowLargeSquare : ComplexRecipe
    {
        public BaseWindowLargeSquare()
        {
            Name = "Large Square Window";
            Type = RecipeFilterLabels.BaseStructures;
            Ingredients.Add(IngredientType.IONISED_COBALT, 10);
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
