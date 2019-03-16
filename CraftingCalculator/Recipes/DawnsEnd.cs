using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class DawnsEnd : ComplexRecipe
    {
        public DawnsEnd()
        {
            Name = "Dawn's End";
            Type = "Atlas Seed";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 25);
            ChildRecipes.Add(new DarkMatter(), 1);
        }
    }
}
