using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnologyRepair
{
    class DeflectorShieldModuleRepair : Recipe
    {
        public DeflectorShieldModuleRepair()
        {
            Name = "Deflector Shield Module (Repair)";
            Type = RecipeFilterLabels.StarshipTechRepair;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 50);
            Ingredients.Add(IngredientType.SODIUM, 50);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
