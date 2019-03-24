using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsMetal
{
    class MetalWindowPanel : ComplexRecipe
    {
        public MetalWindowPanel()
        {
            Name = "Metal Window Panel";
            Type = RecipeFilterLabels.BaseComponentsMetal;
            Ingredients.Add(IngredientType.FERRITE_DUST, 40);
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
