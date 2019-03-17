using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class XShapedCorridor : Recipe
    {
        public XShapedCorridor()
        {
            Name = "X-Shaped Corridor";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 100);
        }
    }
}
