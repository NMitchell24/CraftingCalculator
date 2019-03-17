using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class CurvedCooridor : Recipe
    {
        public CurvedCooridor()
        {
            Name = "Curved Cooridor";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 100);
        }
    }
}
