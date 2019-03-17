using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseStorage
{
    class StorageContainer : Recipe
    {
        public StorageContainer()
        {
            Name = "Storage Container (0 - 9)";
            Type = "Base Storage";
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 50);
            Ingredients.Add(IngredientType.SODIUM, 20);
        }
    }
}
