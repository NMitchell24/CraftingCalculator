using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class InfrastructureLadder : Recipe
    {
        public InfrastructureLadder()
        {
            Name = "Infrastructure Ladder";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 20);
        }
    }
}
