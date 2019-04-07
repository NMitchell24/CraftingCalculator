using CraftingCalculator.Model.Recipes.ConcentratedDeposit;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair
{
    class AmplifiedCartridgesRepair : ComplexRecipe
    {
        public AmplifiedCartridgesRepair()
        {
            Name = "Amplified Cartridges (Repair)";
            Type = RecipeFilterLabels.MultitoolTechRepair;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new TetraCobalt(), 1);
        }
    }
}
