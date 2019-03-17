using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class TShapedCorridor : Recipe
    {
        public TShapedCorridor()
        {
            Name = "T-Shaped Corridor";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 100);
        }
    }
}
