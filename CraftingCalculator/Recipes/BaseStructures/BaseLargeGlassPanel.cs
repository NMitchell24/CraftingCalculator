using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class BaseLargeGlassPanel : ComplexRecipe
    {
        public BaseLargeGlassPanel()
        {
            Name = "Large Glass Panel";
            Type = "Base Building";
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
