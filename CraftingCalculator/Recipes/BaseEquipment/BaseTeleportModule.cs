using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.Consumable;

namespace CraftingCalculator.Recipes.BaseEquipment
{
    class BaseTeleportModule : ComplexRecipe
    {
        public BaseTeleportModule()
        {
            Name = "Base Teleport Module";
            Type = "Base Equipment";
            Ingredients.Add(IngredientType.IONISED_COBALT, 50);
            ChildRecipes.Add(new IonBattery(), 1);
        }
    }
}
