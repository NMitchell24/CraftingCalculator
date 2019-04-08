using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair
{
    class BoltcasterRepair : ComplexRecipe
    {
        public BoltcasterRepair()
        {
            Name = "Boltcaster (Repair)";
            Type = RecipeFilterLabels.MultitoolTechRepair;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 50);
            ChildRecipes.Add(new CarbonNanotubes(), 2);
        }
    }
}
