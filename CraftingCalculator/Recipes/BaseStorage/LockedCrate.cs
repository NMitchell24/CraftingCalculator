using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseStorage
{
    class LockedCrate : Recipe
    {
        public LockedCrate()
        {
            Name = "Locked Crate";
            Type = "Base Storage";
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
