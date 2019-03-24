using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Consumable
{
    class ProjectileAmmunition : Recipe
    {
        public ProjectileAmmunition()
        {
            Name = "Projectile Ammunition (500)";
            Type = RecipeFilterLabels.Consumables;
            Ingredients.Add(IngredientType.FERRITE_DUST, 60);
        }
    }
}
