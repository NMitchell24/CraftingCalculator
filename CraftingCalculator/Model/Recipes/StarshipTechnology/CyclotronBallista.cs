using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnology
{
    class CyclotronBallista : Recipe
    {
        public CyclotronBallista()
        {
            Name = "Cyclotron Ballista";
            Type = "Starship Weapon Module";
            Ingredients.Add(IngredientType.DIOXITE, 200);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 2);
        }
    }
}
