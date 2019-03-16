using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.StarshipTechnology
{
    class CadmiumDrive : Recipe
    {
        public CadmiumDrive()
        {
            Name = "Cadmium Drive";
            Type = "Hyperdrive Module";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 250);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
