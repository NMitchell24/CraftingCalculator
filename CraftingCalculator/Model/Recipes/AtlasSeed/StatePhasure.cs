using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AtlasSeed
{
    class StatePhasure : ComplexRecipe
    {
        public StatePhasure()
        {
            Name = "State Phasure";
            Type = RecipeFilterLabels.AtlasSeed;
            Ingredients.Add(IngredientType.CADMIUM, 100);
            ChildRecipes.Add(new PhoticJade(), 1);
        }
    }
}
