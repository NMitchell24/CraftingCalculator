using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.Consumable
{
    class MindControlDevice : ComplexRecipe
    {
        public MindControlDevice()
        {
            Name = "Mind Control Device";
            Type = "Consumable (Frigate)";
            Ingredients.Add(IngredientType.GOLD, 50);
            ChildRecipes.Add(new SodiumDiode(), 1);
        }
    }
}
