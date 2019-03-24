using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.CraftingComponent
{
    class DihydrogenJelly : Recipe
    {
        public DihydrogenJelly()
        {
            Name = "Di-Hydrogen Jelly";
            Type = RecipeFilterLabels.CraftingComponents;
            Ingredients.Add(IngredientType.DIHYDROGEN, 40);
        }
    }
}
