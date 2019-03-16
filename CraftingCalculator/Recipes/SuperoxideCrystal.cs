using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class SuperoxideCrystal : Recipe
    {
        public SuperoxideCrystal()
        {
            Name = "Superoxide Crystal";
            Type = "Curiosity";
            Ingredients.Add(IngredientType.OXYGEN, 150);
        }
    }
}
