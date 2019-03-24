using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AtlasSeed
{
    class NoosphericOrb : ComplexRecipe
    {
        public NoosphericOrb()
        {
            Name = "Noospheric Orb";
            Type = RecipeFilterLabels.AtlasSeed;
            Ingredients.Add(IngredientType.IONISED_COBALT, 100);
            ChildRecipes.Add(new EnglobedShade(), 1);
        }
    }
}
