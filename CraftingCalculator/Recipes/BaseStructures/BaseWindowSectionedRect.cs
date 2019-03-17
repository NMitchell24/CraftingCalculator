using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class BaseWindowSectionedRect : ComplexRecipe
    {
        public BaseWindowSectionedRect()
        {
            Name = "Rectangular tri-section Window";
            Type = "Base Building";
            Ingredients.Add(IngredientType.IONISED_COBALT, 5);
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
