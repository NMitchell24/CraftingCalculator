using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class StatePhasure : ComplexRecipe
    {
        public StatePhasure()
        {
            Name = "State Phasure";
            Type = "Atlas Seed";
            Ingredients.Add(IngredientType.CADMIUM, 100);
            ChildRecipes.Add(new PhoticJade(), 1);
        }
    }
}
