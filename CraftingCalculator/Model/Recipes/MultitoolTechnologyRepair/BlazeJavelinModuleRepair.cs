using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair
{
    class BlazeJavelinModuleRepair : Recipe
    {
        public BlazeJavelinModuleRepair()
        {
            Name = "Blaze Javelin Module (Repair)";
            Type = RecipeFilterLabels.MultitoolTechRepair;
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 75);
            Ingredients.Add(IngredientType.GRAVATINO_BALL, 1);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
