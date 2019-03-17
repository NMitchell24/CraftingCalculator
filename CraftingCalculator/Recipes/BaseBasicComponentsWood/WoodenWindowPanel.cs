using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Recipes.BaseBasicComponentsWood
{
    class WoodenWindowPanel : ComplexRecipe
    {
        public WoodenWindowPanel()
        {
            Name = "Wooden Window Panel";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CARBON, 40);
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
