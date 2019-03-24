using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStorage
{
    class StorageContainer : Recipe
    {
        public StorageContainer()
        {
            Name = "Storage Container (0 - 9)";
            Type = RecipeFilterLabels.BaseStorage;
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 50);
            Ingredients.Add(IngredientType.SODIUM, 20);
        }
    }
}
