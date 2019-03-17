using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.ExosuitTechnology
{
    class ShieldLattice : ComplexRecipe
    {
        public ShieldLattice()
        {
            Name = "Shield Lattice";
            Type = "Exosuit Shield Module";
            ChildRecipes.Add(new SodiumDiode(), 1);
            ChildRecipes.Add(new Antimatter(), 1);
        }
    }
}
