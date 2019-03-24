using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class CurvedCorridor : Recipe
    {
        public CurvedCorridor()
        {
            Name = "Curved Cooridor";
            Type = RecipeFilterLabels.BaseStructures;
            Ingredients.Add(IngredientType.PURE_FERRITE, 100);
        }
    }
}
