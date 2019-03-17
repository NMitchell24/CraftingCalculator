using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnology
{
    class PositronEjector : Recipe
    {
        public PositronEjector()
        {
            Name = "Positron Ejector";
            Type = "Starship Weapon Module";
            Ingredients.Add(IngredientType.URANIUM, 200);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 2);
        }
    }
}
