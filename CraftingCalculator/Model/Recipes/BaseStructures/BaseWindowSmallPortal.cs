using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class BaseWindowSmallPortal : ComplexRecipe
    {
        public BaseWindowSmallPortal()
        {
            Name = "Small Round Portal Window";
            Type = RecipeFilterLabels.BaseStructures;
            Ingredients.Add(IngredientType.IONISED_COBALT, 5);
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
