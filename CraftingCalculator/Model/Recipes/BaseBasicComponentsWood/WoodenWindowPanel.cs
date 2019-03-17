using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsWood
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
