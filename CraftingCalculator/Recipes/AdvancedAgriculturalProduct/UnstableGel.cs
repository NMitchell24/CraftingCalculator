using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.AdvancedAgriculturalProduct
{
    class UnstableGel : Recipe
    {
        public UnstableGel()
        {
            Name = "Unstable Gel";
            Type = "Advanced Agricultural Product";
            Ingredients.Add(IngredientType.CACTUS_FLESH, 200);
        }
    }
}
