using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStorage
{
    class SmallCrate : Recipe
    {
        public SmallCrate()
        {
            Name = "Small Crate";
            Type = RecipeFilterLabels.BaseStorage;
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
