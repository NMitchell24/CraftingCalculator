using CraftingCalculator.Recipes.EnhancedGasProduct;

namespace CraftingCalculator.Recipes.ManufacturedGasProduct
{
    class OrganicCatalyst : ComplexRecipe
    {
        public OrganicCatalyst()
        {
            Name = "Organic Catalyst";
            Type = "Manufactured Gas Product";
            ChildRecipes.Add(new ThermicCondensate(), 1);
            ChildRecipes.Add(new EnrichedCarbon(), 1);

        }
    }
}
