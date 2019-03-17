using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Farming
{
    class CopriteFlower : Recipe
    {
        public CopriteFlower()
        {
            Name = "Coprite Flower";
            Type = "Farming (Plantable Seed)";
            Ingredients.Add(IngredientType.COPRITE, 40);
        }
    }
}
