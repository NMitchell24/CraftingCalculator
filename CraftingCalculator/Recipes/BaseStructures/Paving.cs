using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class Paving : Recipe
    {
        public Paving()
        {
            Name = "Paving";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
        }
    }
}
