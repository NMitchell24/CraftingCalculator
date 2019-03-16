using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.Consumable;

namespace CraftingCalculator.Recipes.MultitoolTechnology
{
    class CombatScope : ComplexRecipe
    {
        public CombatScope()
        {
            Name = "Combat Scope";
            Type = "Multitool Module";
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new IonBattery(), 2);
        }
    }
}
