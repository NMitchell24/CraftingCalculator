using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExosuitTechnology
{
    class NeuralStimulator : Recipe
    {
        public NeuralStimulator()
        {
            Name = "Neural Stimulator";
            Type = RecipeFilterLabels.ExosuitTech;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 100);
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 100);
        }
    }
}
