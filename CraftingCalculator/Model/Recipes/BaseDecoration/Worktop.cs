using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class Worktop : Recipe
    {
        public Worktop()
        {
            Name = "Worktop";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
