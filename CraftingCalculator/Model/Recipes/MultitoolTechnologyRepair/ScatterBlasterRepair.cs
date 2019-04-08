using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair
{
    class ScatterBlasterRepair : Recipe
    {
        public ScatterBlasterRepair()
        {
            Name = "Scatter Blaster (Repair)";
            Type = RecipeFilterLabels.MultitoolTechRepair;
            Ingredients.Add(IngredientType.PUGNEUM, 100);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
