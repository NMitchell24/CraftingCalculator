using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class HoloDoor : Recipe
    {
        public HoloDoor()
        {
            Name = "Holo Door";
            Type = "Base Building";
            Ingredients.Add(IngredientType.IONISED_COBALT, 40);
            Ingredients.Add(IngredientType.PURE_FERRITE, 40);
        }
    }
}
