using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.CraftingComponent
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
