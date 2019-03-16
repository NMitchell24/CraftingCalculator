using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.AlloyMetal
{
    class DirtyBronze : Recipe
    {
        public DirtyBronze()
        {
            Name = "Dirty Bronze";
            Type = "Alloy Metal";
            Ingredients.Add(IngredientType.PYRITE, 50);
            Ingredients.Add(IngredientType.PURE_FERRITE, 100);
        }
    }
}
