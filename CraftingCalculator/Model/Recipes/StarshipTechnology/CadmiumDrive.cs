using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnology
{
    class CadmiumDrive : Recipe
    {
        public CadmiumDrive()
        {
            Name = "Cadmium Drive";
            Type = RecipeFilterLabels.StarshipTech;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 250);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
