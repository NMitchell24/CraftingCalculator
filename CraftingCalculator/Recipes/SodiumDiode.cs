using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class SodiumDiode : Recipe
    {
        public SodiumDiode()
        {
            Name = "Sodium Diode";
            Type = "Component";
            Ingredients.Add(IngredientType.SODIUM_NITRATE, 40);
            Ingredients.Add(IngredientType.FERRITE_DUST, 40);
        }
    }
}
