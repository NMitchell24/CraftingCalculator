using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class CurvedCorridor : Recipe
    {
        public CurvedCorridor()
        {
            Name = "Curved Cooridor";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 100);
        }
    }
}
