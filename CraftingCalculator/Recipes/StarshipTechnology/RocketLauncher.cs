using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.StarshipTechnology
{
    class RocketLauncher : Recipe
    {
        public RocketLauncher()
        {
            Name = "Rocket Launcher";
            Type = "Starship Weapon Module";
            Ingredients.Add(IngredientType.COPPER, 200);
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 100);
        }
    }
}
