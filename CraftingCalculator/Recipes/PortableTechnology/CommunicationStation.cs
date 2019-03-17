using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.PortableTechnology
{
    class CommunicationStation : Recipe
    {
        public CommunicationStation()
        {
            Name = "Communication Station";
            Type = "Portable Message Technology";
            Ingredients.Add(IngredientType.PURE_FERRITE, 20);
        }
    }
}
