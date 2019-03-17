using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ConcentratedDeposit
{
    class SuperoxideCrystal : Recipe
    {
        public SuperoxideCrystal()
        {
            Name = "Superoxide Crystal";
            Type = "Concentrated Deposit";
            Ingredients.Add(IngredientType.OXYGEN, 150);
        }
    }
}
