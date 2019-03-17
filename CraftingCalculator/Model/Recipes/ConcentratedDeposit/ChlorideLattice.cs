using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ConcentratedDeposit
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
