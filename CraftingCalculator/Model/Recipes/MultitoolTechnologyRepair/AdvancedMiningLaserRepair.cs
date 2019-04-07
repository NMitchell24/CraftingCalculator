using CraftingCalculator.Model.Recipes.CraftingComponent;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair
{
    class AdvancedMiningLaserRepair : ComplexRecipe
    {
        public AdvancedMiningLaserRepair()
        {
            Name = "Advanced Mining Laser (Repair)";
            Type = RecipeFilterLabels.MultitoolTechRepair;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new CarbonNanotubes(), 1);
            ChildRecipes.Add(new HermeticSeal(), 1);
        }
    }
}
