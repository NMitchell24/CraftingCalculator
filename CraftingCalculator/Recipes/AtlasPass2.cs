using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class AtlasPass2 : ComplexRecipe
    {
        public AtlasPass2()
        {
            Name = "Atlas Pass v2";
            Type = "Access Card";
            Ingredients.Add(IngredientType.CADMIUM, 200);
            ChildRecipes.Add(new Microprocessor(), 1);
        }
    }
}
