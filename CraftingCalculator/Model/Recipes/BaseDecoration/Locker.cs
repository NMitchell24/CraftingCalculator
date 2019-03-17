using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class Locker : Recipe
    {
        public Locker()
        {
            Name = "Locker";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
