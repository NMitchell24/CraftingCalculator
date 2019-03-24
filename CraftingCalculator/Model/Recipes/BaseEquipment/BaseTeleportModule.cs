using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.Consumable;

namespace CraftingCalculator.Model.Recipes.BaseEquipment
{
    class BaseTeleportModule : ComplexRecipe
    {
        public BaseTeleportModule()
        {
            Name = "Base Teleport Module";
            Type = RecipeFilterLabels.BaseEquipment;
            Ingredients.Add(IngredientType.IONISED_COBALT, 50);
            ChildRecipes.Add(new IonBattery(), 1);
        }
    }
}
