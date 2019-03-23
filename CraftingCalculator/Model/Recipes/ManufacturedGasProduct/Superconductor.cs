using CraftingCalculator.Model.Recipes.EnhancedGasProduct;

namespace CraftingCalculator.Model.Recipes.ManufacturedGasProduct
{
    class Superconductor : ComplexRecipe
    {
        public Superconductor()
        {
            Name = "Superconductor";
            Type = RecipeFilterLabels.ManufacturedGasProducts;
            ChildRecipes.Add(new Semiconductor(), 1);
            ChildRecipes.Add(new EnrichedCarbon(), 1);
        }
    }
}
