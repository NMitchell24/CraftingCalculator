using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExosuitTechnology
{
    class EfficientWaterJets : Recipe
    {
        public EfficientWaterJets()
        {
            Name = "Efficient Water Jets";
            Type = RecipeFilterLabels.ExosuitTech;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 100);
            Ingredients.Add(IngredientType.OXYGEN, 100);
            Ingredients.Add(IngredientType.LIVING_PEARL, 6);
        }
    }
}
