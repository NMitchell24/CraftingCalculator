using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnologyRepair
{
    class HyperdriveModuleRepair : Recipe
    {
        public HyperdriveModuleRepair()
        {
            Name = "Hyperdrive Module (Repair)";
            Type = RecipeFilterLabels.StarshipTechRepair;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 75);
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 13);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
