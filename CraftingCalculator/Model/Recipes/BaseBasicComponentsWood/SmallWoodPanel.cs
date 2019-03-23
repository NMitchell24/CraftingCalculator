using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsWood
{
    class SmallWoodPanel : Recipe
    {
        public SmallWoodPanel()
        {
            Name = "Wood Panel (Small)";
            Type = RecipeFilterLabels.BaseComponentsWood;
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
