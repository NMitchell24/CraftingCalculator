using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnology
{
    class BlazeJavelin : Recipe
    {
        public BlazeJavelin()
        {
            Name = "Blaze Javelin";
            Type = RecipeFilterLabels.MultitoolTech;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 200);
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 150);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
