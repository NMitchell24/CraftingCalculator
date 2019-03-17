using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class Chair : Recipe
    {
        public Chair()
        {
            Name = "Chair";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
            Ingredients.Add(IngredientType.STAR_SILK, 1);
        }
    }
}
