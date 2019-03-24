using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AtlasSeed
{
    class DarkMatter : ComplexRecipe
    {
        public DarkMatter()
        {
            Name = "Dark Matter";
            Type = RecipeFilterLabels.AtlasSeed;
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 5);
            ChildRecipes.Add(new NoosphericOrb(), 1);
        }
    }
}
