using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ConcentratedDeposit
{
    class ChlorideLattice : Recipe
    {
        public ChlorideLattice()
        {
            Name = "Chloride Lattice";
            Type = RecipeFilterLabels.ConcentratedDeposits;
            Ingredients.Add(IngredientType.CHLORINE, 150);
        }
    }
}
