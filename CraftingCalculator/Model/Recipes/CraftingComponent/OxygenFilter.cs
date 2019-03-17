using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.CraftingComponent
{
    class OxygenFilter : Recipe
    {
        public OxygenFilter()
        {
            Name = "Oxygen Filter";
            Type = "Crafting Component";
            Ingredients.Add(IngredientType.OXYGEN, 90);
            Ingredients.Add(IngredientType.PURE_FERRITE, 30);
        }
    }
}
