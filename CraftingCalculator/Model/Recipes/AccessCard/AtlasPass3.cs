using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.AccessCard
{
    class AtlasPass3 : ComplexRecipe
    {
        public AtlasPass3()
        {
            Name = "Atlas Pass v1";
            Type = "Access Card";
            Ingredients.Add(IngredientType.EMERIL, 200);
            ChildRecipes.Add(new Microprocessor(), 1);
        }
    }
}
