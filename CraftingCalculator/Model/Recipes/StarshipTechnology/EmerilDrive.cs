using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnology
{
    class EmerilDrive : Recipe
    {
        public EmerilDrive()
        {
            Name = "Emeril Drive";
            Type = RecipeFilterLabels.StarshipTech;
            Ingredients.Add(IngredientType.CADMIUM, 250);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 2);
        }
    }
}
