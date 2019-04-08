using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnologyRepair
{
    class PhaseBeamModuleRepair : Recipe
    {
        public PhaseBeamModuleRepair()
        {
            Name = "Phase Beam Module (Repair)";
            Type = RecipeFilterLabels.StarshipTechRepair;
            Ingredients.Add(IngredientType.SILVER, 75);
            Ingredients.Add(IngredientType.CADMIUM, 125);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
