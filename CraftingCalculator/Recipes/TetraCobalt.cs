using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class TetraCobalt : Recipe
    {
        public TetraCobalt()
        {
            Name = "Tetra Cobalt";
            Type = "Curiosity";
            Ingredients.Add(IngredientType.IONISED_COBALT, 150);
        }
    }
}
