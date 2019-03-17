using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class BaseWindowSmallPortal : ComplexRecipe
    {
        public BaseWindowSmallPortal()
        {
            Name = "Small Round Portal Window";
            Type = "Base Building";
            Ingredients.Add(IngredientType.IONISED_COBALT, 5);
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
