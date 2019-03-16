using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class DihydrogenJelly : Recipe
    {
        public DihydrogenJelly()
        {
            Name = "Di-Hydrogen Jelly";
            Type = "Component";
            Ingredients.Add(IngredientType.DIHYDROGEN, 40);
        }
    }
}
