using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class InfrastructureLadder : Recipe
    {
        public InfrastructureLadder()
        {
            Name = "Infrastructure Ladder";
            Type = RecipeFilterLabels.BaseStructures;
            Ingredients.Add(IngredientType.PURE_FERRITE, 20);
        }
    }
}
