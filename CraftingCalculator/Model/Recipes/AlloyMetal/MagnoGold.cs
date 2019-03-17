using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AlloyMetal
{
    class MagnoGold : Recipe
    {
        public MagnoGold()
        {
            Name = "Magno-Gold";
            Type = "Alloy Metal";
            Ingredients.Add(IngredientType.PHOSPHORUS, 50);
            Ingredients.Add(IngredientType.IONISED_COBALT, 50);
        }
    }
}
