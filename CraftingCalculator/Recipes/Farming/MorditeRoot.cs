using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.Farming
{
    class MorditeRoot : Recipe
    {
        public MorditeRoot()
        {
            Name = "Mordite Root";
            Type = "Farming (Plantable Seed)";
            Ingredients.Add(IngredientType.MORDITE, 40);
        }
    }
}
