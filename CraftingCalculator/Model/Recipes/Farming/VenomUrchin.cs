using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Farming
{
    class VenomUrchin : Recipe
    {
        public VenomUrchin()
        {
            Name = "Venom Urchin";
            Type = "Farming (Plantable Seed)";
            Ingredients.Add(IngredientType.EMERIL, 100);
            Ingredients.Add(IngredientType.IONISED_COBALT, 100);
        }
    }
}
