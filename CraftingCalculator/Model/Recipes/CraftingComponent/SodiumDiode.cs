using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.CraftingComponent
{
    class SodiumDiode : Recipe
    {
        public SodiumDiode()
        {
            Name = "Sodium Diode";
            Type = "Crafting Component";
            Ingredients.Add(IngredientType.SODIUM_NITRATE, 40);
            Ingredients.Add(IngredientType.FERRITE_DUST, 40);
        }
    }
}
