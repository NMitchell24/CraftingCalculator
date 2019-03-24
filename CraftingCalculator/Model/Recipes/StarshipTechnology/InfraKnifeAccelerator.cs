using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnology
{
    class InfraKnifeAccelerator : Recipe
    {
        public InfraKnifeAccelerator()
        {
            Name = "Infra-Knife Accelerator";
            Type = RecipeFilterLabels.StarshipTech;
            Ingredients.Add(IngredientType.PHOSPHORUS, 200);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 2);
        }
    }
}
