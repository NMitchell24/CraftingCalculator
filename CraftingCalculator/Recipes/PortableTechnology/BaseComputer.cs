using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.PortableTechnology
{
    class BaseComputer : Recipe
    {
        public BaseComputer()
        {
            Name = "Base Computer";
            Type = "Portable Base Technology";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 40);
        }
    }
}
