using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Recipes.BaseBasicComponentsMetal
{
    class MetalWindowPanel : ComplexRecipe
    {
        public MetalWindowPanel()
        {
            Name = "Metal Window Panel";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 40);
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
