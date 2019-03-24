using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.PortableTechnology
{
    class BaseComputer : Recipe
    {
        public BaseComputer()
        {
            Name = "Base Computer";
            Type = RecipeFilterLabels.PortableTech;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 40);
        }
    }
}
