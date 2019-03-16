using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class UnstablePlasma : ComplexRecipe
    {
        public UnstablePlasma()
        {
            Name = "Unstable Plasma";
            Type = "Consumable (Plasma Launcher)";
            Ingredients.Add(IngredientType.OXYGEN, 50);
            ChildRecipes.Add(new MetalPlating(), 1);
        }
    }
}
