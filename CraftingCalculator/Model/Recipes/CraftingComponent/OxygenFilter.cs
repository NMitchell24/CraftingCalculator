using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.CraftingComponent
{
    class OxygenFilter : Recipe
    {
        public OxygenFilter()
        {
            Name = "Oxygen Filter";
            Type = RecipeFilterLabels.CraftingComponents;
            Ingredients.Add(IngredientType.OXYGEN, 90);
            Ingredients.Add(IngredientType.PURE_FERRITE, 30);
        }
    }
}
