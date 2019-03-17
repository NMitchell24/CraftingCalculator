using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExocraftTechnology
{
    class ExocraftMiningLaserSigma : Recipe
    {
        public ExocraftMiningLaserSigma()
        {
            Name = "Exocraft Mining Laser Upgrade Sigma";
            Type = "Exocraft Mining Laser Module";
            Ingredients.Add(IngredientType.PUGNEUM, 50);
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 50);
        }
    }
}
