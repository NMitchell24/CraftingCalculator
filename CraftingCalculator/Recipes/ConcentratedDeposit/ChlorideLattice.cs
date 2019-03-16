using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.ConcentratedDeposit
{
    class ChlorideLattice : Recipe
    {
        public ChlorideLattice()
        {
            Name = "Chloride Lattice";
            Type = "Concentrated Deposit";
            Ingredients.Add(IngredientType.CHLORINE, 150);
        }
    }
}
