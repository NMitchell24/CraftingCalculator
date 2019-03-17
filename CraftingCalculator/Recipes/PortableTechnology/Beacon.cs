using CraftingCalculator.Recipes.CraftingComponent;
using CraftingCalculator.Recipes.Consumable;

namespace CraftingCalculator.Recipes.PortableTechnology
{
    class Beacon : ComplexRecipe
    {
        public Beacon()
        {
            Name = "Beacon";
            Type = "Portable Base Technology";
            ChildRecipes.Add(new MetalPlating(), 1);
            ChildRecipes.Add(new Microprocessor(), 1);
            ChildRecipes.Add(new IonBattery(), 2);
        }
    }
}
