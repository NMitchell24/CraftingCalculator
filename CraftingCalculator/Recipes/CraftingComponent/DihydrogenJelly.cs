using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.CraftingComponent
{
    class DihydrogenJelly : Recipe
    {
        public DihydrogenJelly()
        {
            Name = "Di-Hydrogen Jelly";
            Type = "Crafting Component";
            Ingredients.Add(IngredientType.DIHYDROGEN, 40);
        }
    }
}
