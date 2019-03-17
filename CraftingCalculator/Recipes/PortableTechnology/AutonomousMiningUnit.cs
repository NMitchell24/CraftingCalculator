using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;
using CraftingCalculator.Recipes.Consumable;

namespace CraftingCalculator.Recipes.PortableTechnology
{
    class AutonomousMiningUnit : ComplexRecipe
    {
        public AutonomousMiningUnit()
        {
            Name = "Autonomous Mining Unit";
            Type = "Portable Base Technology";
            Ingredients.Add(IngredientType.URANIUM, 100);
            ChildRecipes.Add(new MetalPlating(), 2);
            ChildRecipes.Add(new AdvancedIonBattery(), 1);
        }
    }
}
