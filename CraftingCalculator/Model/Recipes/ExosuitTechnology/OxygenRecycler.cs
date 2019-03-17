using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.ExosuitTechnology
{
    class OxygenRecycler : ComplexRecipe
    {
        public OxygenRecycler()
        {
            Name = "Oxygen Recycler";
            Type = "Life Support Module";
            ChildRecipes.Add(new OxygenFilter(), 1);
        }
    }
}
