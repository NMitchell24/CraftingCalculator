using CraftingCalculator.Model.Recipes.ConcentratedDeposit;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair
{
    class GeologyCannonRepair : ComplexRecipe
    {
        public GeologyCannonRepair()
        {
            Name = "Geology Cannon (Repair)";
            Type = RecipeFilterLabels.MultitoolTechRepair;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new RareMetalElement(), 1);
        }
    }
}
