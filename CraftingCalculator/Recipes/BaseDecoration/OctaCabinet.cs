using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class OctaCabinet : Recipe
    {
        public OctaCabinet()
        {
            Name = "Octa-Cabinet";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
