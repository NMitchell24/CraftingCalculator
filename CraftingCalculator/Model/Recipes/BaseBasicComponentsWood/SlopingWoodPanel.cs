using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsWood
{
    class SlopingWoodPanel : Recipe
    {
        public SlopingWoodPanel()
        {
            Name = "Sloping Wood Panel";
            Type = RecipeFilterLabels.BaseComponentsWood;
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
