using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AtlasSeed
{
    class NovaeReclaiment : ComplexRecipe
    {
        public NovaeReclaiment()
        {
            Name = "Novae Reclaiment";
            Type = RecipeFilterLabels.AtlasSeed;
            Ingredients.Add(IngredientType.EMERIL, 100);
            ChildRecipes.Add(new StatePhasure(), 1);
        }
    }
}
