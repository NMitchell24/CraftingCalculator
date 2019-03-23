using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExocraftTechnology
{
    class ExocraftMiningLaserSigma : Recipe
    {
        public ExocraftMiningLaserSigma()
        {
            Name = "Exocraft Mining Laser Upgrade Sigma";
            Type = RecipeFilterLabels.ExocraftTech;
            Ingredients.Add(IngredientType.PUGNEUM, 50);
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 50);
        }
    }
}
