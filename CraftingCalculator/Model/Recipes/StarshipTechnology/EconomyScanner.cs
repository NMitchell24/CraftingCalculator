using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.StarshipTechnology
{
    class EconomyScanner : ComplexRecipe
    {
        public EconomyScanner()
        {
            Name = "Economy Scanner";
            Type = RecipeFilterLabels.StarshipTech;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new Microprocessor(), 5);
        }
    }
}
