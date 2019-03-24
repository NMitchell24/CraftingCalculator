using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AlloyMetal
{
    class DirtyBronze : Recipe
    {
        public DirtyBronze()
        {
            Name = "Dirty Bronze";
            Type = RecipeFilterLabels.AlloyMetal;
            Ingredients.Add(IngredientType.PYRITE, 50);
            Ingredients.Add(IngredientType.PURE_FERRITE, 100);
        }
    }
}
