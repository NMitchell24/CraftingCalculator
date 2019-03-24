using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class BaseWindowSectionedRect : ComplexRecipe
    {
        public BaseWindowSectionedRect()
        {
            Name = "Rectangular tri-section Window";
            Type = RecipeFilterLabels.BaseStructures;
            Ingredients.Add(IngredientType.IONISED_COBALT, 5);
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
