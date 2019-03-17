using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class CeilingLight : Recipe
    {
        public CeilingLight()
        {
            Name = "Ceiling Light";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
            Ingredients.Add(IngredientType.COBALT, 5);
        }
    }
}
