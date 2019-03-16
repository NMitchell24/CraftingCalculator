using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.Farming
{
    class GravatinoHost : Recipe
    {
        public GravatinoHost()
        {
            Name = "Gravatino Host";
            Type = "Farming (Plantable Seed)";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 25);
            Ingredients.Add(IngredientType.IONISED_COBALT, 120);
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 25);
        }
    }
}
