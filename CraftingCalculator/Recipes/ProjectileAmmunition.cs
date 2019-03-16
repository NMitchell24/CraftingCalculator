using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class ProjectileAmmunition : Recipe
    {
        public ProjectileAmmunition()
        {
            Name = "Projectile Ammunition (500)";
            Type = "Ammunition";
            Ingredients.Add(IngredientType.FERRITE_DUST, 60);
        }
    }
}
