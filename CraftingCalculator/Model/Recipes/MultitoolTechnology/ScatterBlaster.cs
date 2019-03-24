using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnology
{
    class ScatterBlaster : Recipe
    {
        public ScatterBlaster()
        {
            Name = "Scatter Blaster";
            Type = RecipeFilterLabels.MultitoolTech;
            Ingredients.Add(IngredientType.PUGNEUM, 200);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
