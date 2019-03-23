using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.AccessCard
{
    class AtlasPass1 : ComplexRecipe
    {
        public AtlasPass1()
        {
            Name = "Atlas Pass v1";
            Type = RecipeFilterLabels.AccessCard;
            Ingredients.Add(IngredientType.COPPER, 200);
            ChildRecipes.Add(new Microprocessor(), 1);
        }
    }
}
