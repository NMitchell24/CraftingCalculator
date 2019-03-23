using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.Consumable
{
    class MindControlDevice : ComplexRecipe
    {
        public MindControlDevice()
        {
            Name = "Mind Control Device";
            Type = RecipeFilterLabels.Consumables;
            Ingredients.Add(IngredientType.GOLD, 50);
            ChildRecipes.Add(new SodiumDiode(), 1);
        }
    }
}
