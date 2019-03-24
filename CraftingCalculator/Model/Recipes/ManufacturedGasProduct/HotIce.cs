using CraftingCalculator.Model.Recipes.EnhancedGasProduct;

namespace CraftingCalculator.Model.Recipes.ManufacturedGasProduct
{
    class HotIce : ComplexRecipe
    {
        public HotIce()
        {
            Name = "Hot Ice";
            Type = RecipeFilterLabels.ManufacturedGasProducts;
            ChildRecipes.Add(new EnrichedCarbon(), 1);
            ChildRecipes.Add(new NitrogenSalt(), 1);
        }
    }
}
