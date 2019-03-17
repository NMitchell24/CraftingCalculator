using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.Consumable
{
    class WarpCell : ComplexRecipe
    {
        public WarpCell()
        {
            Name = "Warp Drive";
            Type = "Consumable (Hyperdrive Fuel)";
            ChildRecipes.Add(new AntimatterHousing(), 1);
            ChildRecipes.Add(new Antimatter(), 1);
        }
    }
}
