using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ConcentratedDeposit
{
    class TetraCobalt : Recipe
    {
        public TetraCobalt()
        {
            Name = "Tetra Cobalt";
            Type = "Concentrated Deposit";
            Ingredients.Add(IngredientType.IONISED_COBALT, 150);
        }
    }
}
