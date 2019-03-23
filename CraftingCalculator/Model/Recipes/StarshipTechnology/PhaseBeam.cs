using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnology
{
    class PhaseBeam : Recipe
    {
        public PhaseBeam()
        {
            Name = "Phase Beam";
            Type = RecipeFilterLabels.StarshipTech;
            Ingredients.Add(IngredientType.TRITIUM, 100);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
