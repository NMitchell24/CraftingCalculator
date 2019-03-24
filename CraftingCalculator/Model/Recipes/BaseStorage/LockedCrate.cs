using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStorage
{
    class LockedCrate : Recipe
    {
        public LockedCrate()
        {
            Name = "Locked Crate";
            Type = RecipeFilterLabels.BaseStorage;
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
