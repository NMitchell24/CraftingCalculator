using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnology
{
    class PulseSplitter : Recipe
    {
        public PulseSplitter()
        {
            Name = "Pulse Splitter";
            Type = RecipeFilterLabels.MultitoolTech;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            Ingredients.Add(IngredientType.DEUTERIUM, 200);
        }
    }
}
