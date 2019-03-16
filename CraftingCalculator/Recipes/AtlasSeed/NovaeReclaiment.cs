using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.AtlasSeed
{
    class NovaeReclaiment : ComplexRecipe
    {
        public NovaeReclaiment()
        {
            Name = "Novae Reclaiment";
            Type = "Atlas Seed";
            Ingredients.Add(IngredientType.EMERIL, 100);
            ChildRecipes.Add(new StatePhasure(), 1);
        }
    }
}
