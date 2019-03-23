using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class LabLamp : Recipe
    {
        public LabLamp()
        {
            Name = "Lab Lamp";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.SODIUM, 10);
        }
    }
}
