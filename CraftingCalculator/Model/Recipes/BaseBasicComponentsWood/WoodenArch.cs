using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsWood
{
    class WoodenArch : Recipe
    {
        public WoodenArch()
        {
            Name = "Wooden Arch";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CARBON, 40);
        }
    }
}
