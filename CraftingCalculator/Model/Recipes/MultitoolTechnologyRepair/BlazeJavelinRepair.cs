using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair
{
    class BlazeJavelinRepair : Recipe
    {
        public BlazeJavelinRepair()
        {
            Name = "Blaze Javelin (Repair)";
            Type = RecipeFilterLabels.MultitoolTechRepair;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 100);
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 75);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
