using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.Farming
{
    class Echinocactus : Recipe
    {
        public Echinocactus()
        {
            Name = "Echinocactus";
            Type = "Farming (Plantable Seed)";
            Ingredients.Add(IngredientType.CACTUS_FLESH, 50);
            Ingredients.Add(IngredientType.PYRITE, 25);
        }
    }
}
