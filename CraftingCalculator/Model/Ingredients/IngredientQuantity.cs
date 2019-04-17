
namespace CraftingCalculator.Model.Ingredients
{
    /// <summary>
    /// Represents an ingredient and quantity.
    /// </summary>
    public class IngredientQuantity
    {
        public long Quantity { get; set; }

        public string Name { get; set; }

        public string DisplayName
        {
            get => Name + " x" + Quantity;
            private set { }
        }

        public IngredientQuantity(string name, long quantity)
        {
            Name = name;
            Quantity = quantity;
        }

        public IngredientQuantity Clone()
        {
            return new IngredientQuantity (Name, Quantity);
        }

    }
}
