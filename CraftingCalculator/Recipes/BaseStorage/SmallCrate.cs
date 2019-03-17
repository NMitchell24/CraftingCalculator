using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseStorage
{
    class SmallCrate : Recipe
    {
        public SmallCrate()
        {
            Name = "Small Crate";
            Type = "Base Storage";
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
