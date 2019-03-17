using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
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
