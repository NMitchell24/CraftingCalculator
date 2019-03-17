using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class Ladder : Recipe
    {
        public Ladder()
        {
            Name = "Ladder";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 30);
        }
    }
}
