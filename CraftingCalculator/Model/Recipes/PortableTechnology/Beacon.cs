using CraftingCalculator.Model.Recipes.CraftingComponent;
using CraftingCalculator.Model.Recipes.Consumable;

namespace CraftingCalculator.Model.Recipes.PortableTechnology
{
    class Beacon : ComplexRecipe
    {
        public Beacon()
        {
            Name = "Beacon";
            Type = RecipeFilterLabels.PortableTech;
            ChildRecipes.Add(new MetalPlating(), 1);
            ChildRecipes.Add(new Microprocessor(), 1);
            ChildRecipes.Add(new IonBattery(), 2);
        }
    }
}
