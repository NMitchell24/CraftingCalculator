using CraftingCalculator.Model.Recipes.CraftingComponent;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair
{
    class GeologyCannonModuleRepair : ComplexRecipe
    {
        public GeologyCannonModuleRepair()
        {
            Name = "Geology Cannon Module (Repair)";
            Type = RecipeFilterLabels.MultitoolTechRepair;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new DihydrogenJelly(), 2);
        }
    }
}
