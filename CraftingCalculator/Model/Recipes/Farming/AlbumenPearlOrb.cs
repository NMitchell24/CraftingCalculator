using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Farming
{
    class AlbumenPearlOrb : Recipe
    {
        public AlbumenPearlOrb()
        {
            Name = "Albumen Pearl Orb";
            Type = RecipeFilterLabels.Farming;
            Ingredients.Add(IngredientType.INDIUM, 60);
            Ingredients.Add(IngredientType.PARAFFINIUM, 20);
        }
    }
}
