using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.PortableTechnology
{
    class CommunicationStation : Recipe
    {
        public CommunicationStation()
        {
            Name = "Communication Station";
            Type = RecipeFilterLabels.PortableTech;
            Ingredients.Add(IngredientType.PURE_FERRITE, 20);
        }
    }
}
