using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.Farming
{
    class AlbumenPearlOrb : Recipe
    {
        public AlbumenPearlOrb()
        {
            Name = "Albumen Pearl Orb";
            Type = "Farming (Orb Plant)";
            Ingredients.Add(IngredientType.INDIUM, 60);
            Ingredients.Add(IngredientType.PARAFFINIUM, 20);
        }
    }
}
