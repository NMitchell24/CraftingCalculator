using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.Consumable
{
    class IonBattery : Recipe
    {
        public IonBattery()
        {
            Name = "Ion Battery";
            Type = "Consumable";
            Ingredients.Add(IngredientType.COBALT, 25);
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
