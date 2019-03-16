using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.ExosuitTechnology
{
    class EfficientWaterJets : Recipe
    {
        public EfficientWaterJets()
        {
            Name = "Efficient Water Jets";
            Type = "Jetpack Module";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 100);
            Ingredients.Add(IngredientType.OXYGEN, 100);
            Ingredients.Add(IngredientType.LIVING_PEARL, 6);
        }
    }
}
