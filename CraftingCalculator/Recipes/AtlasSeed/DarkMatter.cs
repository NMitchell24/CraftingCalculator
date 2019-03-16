using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.AtlasSeed
{
    class DarkMatter : ComplexRecipe
    {
        public DarkMatter()
        {
            Name = "Dark Matter";
            Type = "Atlas Seed";
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 5);
            ChildRecipes.Add(new NoosphericOrb(), 1);
        }
    }
}
