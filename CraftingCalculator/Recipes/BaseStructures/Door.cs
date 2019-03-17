using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class Door : Recipe
    {
        public Door()
        {
            Name = "Door";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 50);
        }
    }
}
