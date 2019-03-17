using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class LabLamp : Recipe
    {
        public LabLamp()
        {
            Name = "Lab Lamp";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.SODIUM, 10);
        }
    }
}
