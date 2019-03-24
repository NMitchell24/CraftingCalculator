using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnology
{
    class RocketLauncher : Recipe
    {
        public RocketLauncher()
        {
            Name = "Rocket Launcher";
            Type = RecipeFilterLabels.StarshipTech;
            Ingredients.Add(IngredientType.COPPER, 200);
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 100);
        }
    }
}
