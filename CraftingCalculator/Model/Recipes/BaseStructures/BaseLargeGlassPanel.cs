using CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class BaseLargeGlassPanel : ComplexRecipe
    {
        public BaseLargeGlassPanel()
        {
            Name = "Large Glass Panel";
            Type = RecipeFilterLabels.BaseStructures;
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
