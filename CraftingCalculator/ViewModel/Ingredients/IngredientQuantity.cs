
namespace CraftingCalculator.ViewModel.Ingredients
{
    /// <summary>
    /// Represents an ingredient and quantity.
    /// </summary>
    public class IngredientQuantity
    {
        public long Quantity { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string DisplayName
        {
            get => Name + " x" + Quantity;
            private set { }
        }

        public IngredientQuantity(string name, string description, long quantity)
        {
            Name = name;
            Description = description;
            Quantity = quantity;
        }

        public IngredientQuantity Clone()
        {
            return new IngredientQuantity (Name, Description, Quantity);
        }
    }
}
