using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ConcentratedDeposit
{
    class SuperoxideCrystal : Recipe
    {
        public SuperoxideCrystal()
        {
            Name = "Superoxide Crystal";
            Type = RecipeFilterLabels.ConcentratedDeposits;
            Ingredients.Add(IngredientType.OXYGEN, 150);
        }
    }
}
