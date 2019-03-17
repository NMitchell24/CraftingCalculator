using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class Foundation : Recipe
    {
        public Foundation()
        {
            Name = "Foundation";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 150);
        }
    }
}
