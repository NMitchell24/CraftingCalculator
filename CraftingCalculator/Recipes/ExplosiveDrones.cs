using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class ExplosiveDrones : Recipe
    {
        public ExplosiveDrones()
        {
            Name = "Explosive Drones";
            Type = "Consumable (Frigate)";
            Ingredients.Add(IngredientType.WALKER_BRAIN, 1);
            Ingredients.Add(IngredientType.GOLD, 50);
        }
    }
}
