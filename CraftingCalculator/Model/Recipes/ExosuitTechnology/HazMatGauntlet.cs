using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExosuitTechnology
{
    class HazMatGauntlet : Recipe
    {
        public HazMatGauntlet()
        {
            Name = "Haz-Mat Gauntlet";
            Type = "Exosuit Module";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 50);
            Ingredients.Add(IngredientType.SODIUM_NITRATE, 20);
        }
    }
}
