using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AtlasSeed
{
    class ModifiedQuanta : ComplexRecipe
    {
        public ModifiedQuanta()
        {
            Name = "Modified Quanta";
            Type = "Atlas Seed";
            Ingredients.Add(IngredientType.INDIUM, 100);
            ChildRecipes.Add(new NovaeReclaiment(), 1);
        }
    }
}
