using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Consumable
{
    class IonBattery : Recipe
    {
        public IonBattery()
        {
            Name = "Ion Battery";
            Type = RecipeFilterLabels.Consumables;
            Ingredients.Add(IngredientType.COBALT, 25);
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
