using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
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
