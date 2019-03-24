using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsMetal
{
    class SmallMetalPanel : Recipe
    {
        public SmallMetalPanel()
        {
            Name = "Small Metal Panel";
            Type = RecipeFilterLabels.BaseComponentsMetal;
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
