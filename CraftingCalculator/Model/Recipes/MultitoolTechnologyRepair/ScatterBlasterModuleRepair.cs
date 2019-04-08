using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair
{
    class ScatterBlasterModuleRepair : Recipe
    {
        public ScatterBlasterModuleRepair()
        {
            Name = "Scatter Blaster Module (Repair)";
            Type = RecipeFilterLabels.MultitoolTechRepair;
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 125);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
