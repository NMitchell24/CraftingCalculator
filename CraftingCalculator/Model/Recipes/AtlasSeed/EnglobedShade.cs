using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AtlasSeed
{
    class EnglobedShade : ComplexRecipe
    {
        public EnglobedShade()
        {
            Name = "Englobed Shade";
            Type = "Atlas Seed";
            Ingredients.Add(IngredientType.PURE_FERRITE, 100);
            ChildRecipes.Add(new CapturedNanode(), 1);
        }
    }
}
