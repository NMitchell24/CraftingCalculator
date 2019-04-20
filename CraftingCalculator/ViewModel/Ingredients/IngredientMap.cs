using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace CraftingCalculator.ViewModel.Ingredients
{
    public class IngredientMap
    {
        private List<IngredientQuantity> _internalList = new List<IngredientQuantity>();
        public ReadOnlyCollection<IngredientQuantity> IngredientList
        {
            get => _internalList.AsReadOnly();
            private set { }
        }

        public IngredientMap(IngredientMap map)
        {
            _internalList = new List<IngredientQuantity>();
            foreach(IngredientQuantity i in map.IngredientList)
            {
                // Insures we don't retain references to the original IngredientQuantity object and cause those 
                // objects to get mutated elsewhere.
                _internalList.Add(i.Clone());
            }
        }

        public IngredientMap()
        {
            //nothing
        }
        /// <summary>
        /// Adds the ingredient and quantity to the list if it does not exist.
        /// If it does exist then it will just increase the quantity of the existing record
        /// by the quantity that is passed into this Add function.
        /// </summary>
        /// <param name="ingredient"></param>
        /// <param name="quantity"></param>
        public void Add(string ingredient, string description, long quantity)
        {
            if(_internalList.Any(i => i.Name == ingredient))
            {
                _internalList.Find(i => i.Name == ingredient).Quantity += quantity;
            } 
            else
            {
                _internalList.Add(new IngredientQuantity(ingredient, description, quantity));
            }
        }

        /// <summary>
        /// Decrement an IngredientQuantity by the provided amount.  
        /// If the current Quantity - the provided quantity would be less than or equal to 0
        /// Then the Ingredient will be removed.
        /// </summary>
        /// <param name="ingredient"></param>
        /// <param name="quantity"></param>
        public void Remove(string ingredient, string description, long quantity)
        {
            if (_internalList.Any(i => i.Name == ingredient && i.Quantity - quantity > 0))
            {
                Add(ingredient, description, -quantity);
            }
            else
            {
                RemoveAll(ingredient);
            }

        }

        /// <summary>
        /// Remove an IngredientType from the list entirely if it exists.
        /// </summary>
        /// <param name="ingredient"></param>
        public void RemoveAll(string ingredient)
        {
            if (_internalList.Any(i => i.Name == ingredient))
            {
                _internalList.Remove(_internalList.Find(i => i.Name == ingredient));
            }
        }

        /// <summary>
        /// Reset the contents of the map and empty it.
        /// </summary>
        public void Reset()
        {
            _internalList.Clear();
        }
    }
}
