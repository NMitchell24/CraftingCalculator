using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
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
