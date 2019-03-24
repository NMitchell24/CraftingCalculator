using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.AccessCard
{
    class AtlasPass2 : ComplexRecipe
    {
        public AtlasPass2()
        {
            Name = "Atlas Pass v2";
            Type = RecipeFilterLabels.AccessCard;
            Ingredients.Add(IngredientType.CADMIUM, 200);
            ChildRecipes.Add(new Microprocessor(), 1);
        }
    }
}
