using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;
using CraftingCalculator.Model.Recipes.Consumable;

namespace CraftingCalculator.Model.Recipes.PortableTechnology
{
    class AutonomousMiningUnit : ComplexRecipe
    {
        public AutonomousMiningUnit()
        {
            Name = "Autonomous Mining Unit";
            Type = RecipeFilterLabels.PortableTech;
            Ingredients.Add(IngredientType.URANIUM, 100);
            ChildRecipes.Add(new MetalPlating(), 2);
            ChildRecipes.Add(new AdvancedIonBattery(), 1);
        }
    }
}
