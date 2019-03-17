
namespace CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct
{
    class CircuitBoard : ComplexRecipe
    {
        public CircuitBoard()
        {
            Name = "Circuit Board";
            Type = "Advanced Agricultural Product";
            ChildRecipes.Add(new HeatCapacitor(), 1);
            ChildRecipes.Add(new PolyFibre(), 1);
        }
    }
}
