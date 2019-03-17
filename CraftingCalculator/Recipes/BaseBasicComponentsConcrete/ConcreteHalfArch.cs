using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsConcrete
{
    class ConcreteHalfArch : Recipe
    {
        public ConcreteHalfArch()
        {
            Name = "Concrete Half Arch";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
