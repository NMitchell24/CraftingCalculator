using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.StarshipTechnology
{
    class PhaseBeam : Recipe
    {
        public PhaseBeam()
        {
            Name = "Phase Beam";
            Type = "Starship Weapon Module";
            Ingredients.Add(IngredientType.TRITIUM, 100);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
