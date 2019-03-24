using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Farming
{
    class GravatinoHost : Recipe
    {
        public GravatinoHost()
        {
            Name = "Gravatino Host";
            Type = RecipeFilterLabels.Farming;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 25);
            Ingredients.Add(IngredientType.IONISED_COBALT, 120);
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 25);
        }
    }
}
