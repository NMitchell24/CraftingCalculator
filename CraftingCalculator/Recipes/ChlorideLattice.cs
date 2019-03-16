using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class ChlorideLattice : Recipe
    {
        public ChlorideLattice()
        {
            Name = "Chloride Lattice";
            Type = "Curiosity";
            Ingredients.Add(IngredientType.CHLORINE, 150);
        }
    }
}
