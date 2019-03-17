using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.Consumable
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
