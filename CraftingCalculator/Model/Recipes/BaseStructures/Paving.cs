using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class Paving : Recipe
    {
        public Paving()
        {
            Name = "Paving";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
        }
    }
}
