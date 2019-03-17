using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsConcrete
{
    class ThinConcreteWall : Recipe
    {
        public ThinConcreteWall()
        {
            Name = "Thin Concrete Wall";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
