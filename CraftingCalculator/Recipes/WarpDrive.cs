using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class WarpDrive : ComplexRecipe
    {
        public WarpDrive()
        {
            Name = "Warp Drive";
            Type = "Consumable (Hyperdrive Fuel)";
            ChildRecipes.Add(new AntimatterHousing(), 1);
            ChildRecipes.Add(new Antimatter(), 1);
        }
    }
}
