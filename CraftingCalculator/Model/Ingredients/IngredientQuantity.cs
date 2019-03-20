
namespace CraftingCalculator.Model.Ingredients
{
    /// <summary>
    /// Represents an ingredient and quantity.
    /// </summary>
    public class IngredientQuantity
    {
        public IngredientType Ingredient { get; set; }
        public int Quantity { get; set; }

        public string Name
        {
            get => Ingredient.GetDisplayName();
            private set { }
        }

        public IngredientQuantity(IngredientType i, int q)
        {
            Ingredient = i;
            Quantity = q;
        }


    }
}
