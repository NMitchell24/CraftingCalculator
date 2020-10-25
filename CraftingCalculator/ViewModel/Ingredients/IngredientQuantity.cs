
namespace CraftingCalculator.ViewModel.Ingredients
{
    /// <summary>
    /// Represents an ingredient and quantity.
    /// </summary>
    public class IngredientQuantity : AbstractPropertyChanged, IBaseQuantityRecord
    {
        public int Id { get; set; }
        public Ingredient Ingredient { get; set; }
        public long Quantity { get; set; }
        public string Name { get => Ingredient.Name; set { } }
        public string Description { get => Ingredient.Description; set { } }
        public string Tooltip { get => Ingredient.Tooltip; set { } }
        public DataType Type { get => DataType.Ingredient; set { } }
        public double TotalCost { get => Ingredient.Cost * Quantity; set { } }

        public string DisplayName
        {
            get => Name + " x" + Quantity;
            private set { }
        }

        public IngredientQuantity(Ingredient ing, long quantity, int id)
        {
            Ingredient = ing;
            Quantity = quantity;
            Id = id;
        }

        public IngredientQuantity Clone()
        {
            return new IngredientQuantity (Ingredient, Quantity, Id);
        }

        public IngredientQuantity CloneForSave()
        {
            IngredientQuantity ret = this.Clone();
            ret.Id = 0;
            return ret;
        }
    }
}
