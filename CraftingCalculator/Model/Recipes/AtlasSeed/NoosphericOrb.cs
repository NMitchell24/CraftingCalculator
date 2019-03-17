using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AtlasSeed
{
    class NoosphericOrb : ComplexRecipe
    {
        public NoosphericOrb()
        {
            Name = "Noospheric Orb";
            Type = "Atlas Seed";
            Ingredients.Add(IngredientType.IONISED_COBALT, 200);
            ChildRecipes.Add(new EnglobedShade(), 1);
        }
    }
}
