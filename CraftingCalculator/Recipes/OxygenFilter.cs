using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class OxygenFilter : Recipe
    {
        public OxygenFilter()
        {
            Name = "Oxygen Filter";
            Type = "Component";
            Ingredients.Add(IngredientType.OXYGEN, 90);
            Ingredients.Add(IngredientType.PURE_FERRITE, 30);
        }
    }
}
