using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.CraftingComponent
{
    class SodiumDiode : Recipe
    {
        public SodiumDiode()
        {
            Name = "Sodium Diode";
            Type = RecipeFilterLabels.CraftingComponents;
            Ingredients.Add(IngredientType.SODIUM_NITRATE, 40);
            Ingredients.Add(IngredientType.FERRITE_DUST, 40);
        }
    }
}
