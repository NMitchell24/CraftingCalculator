using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.Farming
{
    class Frostwort : Recipe
    {
        public Frostwort()
        {
            Name = "Frostwort";
            Type = "Farming (Plantable Seed)";
            Ingredients.Add(IngredientType.FROST_CRYSTAL, 50);
            Ingredients.Add(IngredientType.DIOXITE, 25);
        }
    }
}
