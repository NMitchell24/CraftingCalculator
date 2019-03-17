using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class StoragePanel : Recipe
    {
        public StoragePanel()
        {
            Name = "Storage Panel";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
