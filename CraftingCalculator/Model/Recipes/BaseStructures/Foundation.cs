using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
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
