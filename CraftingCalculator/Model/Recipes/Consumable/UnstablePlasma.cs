using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.Consumable
{
    class UnstablePlasma : ComplexRecipe
    {
        public UnstablePlasma()
        {
            Name = "Unstable Plasma";
            Type = RecipeFilterLabels.Consumables;
            Ingredients.Add(IngredientType.OXYGEN, 50);
            ChildRecipes.Add(new MetalPlating(), 1);
        }
    }
}
