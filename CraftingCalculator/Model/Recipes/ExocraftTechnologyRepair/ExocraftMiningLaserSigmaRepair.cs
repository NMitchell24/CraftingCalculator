using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExocraftTechnologyRepair
{
    class ExocraftMiningLaserSigmaRepair : Recipe
    {
        public ExocraftMiningLaserSigmaRepair()
        {
            Name = "Exocraft Mining Laser Sigma (Repair)";
            Type = RecipeFilterLabels.ExocraftTechRepair;
            Ingredients.Add(IngredientType.PUGNEUM, 50);
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 50);
        }
    }
}
