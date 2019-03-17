using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
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
