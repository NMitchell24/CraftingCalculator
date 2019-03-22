using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace CraftingCalculator.Model.Ingredients
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
            _internalList = new List<IngredientQuantity>(map.IngredientList);
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
        public void Add(IngredientType ingredient, int quantity)
        {
            if(_internalList.Any(i => i.Ingredient == ingredient))
            {
                _internalList.Find(i => i.Ingredient == ingredient).Quantity += quantity;
            } 
            else
            {
                _internalList.Add(new IngredientQuantity(ingredient, quantity));
            }
        }

        /// <summary>
        /// Decrement an IngredientQuantity by the provided amount.  
        /// If the current Quantity - the provided quantity would be less than or equal to 0
        /// Then the Ingredient will be removed.
        /// </summary>
        /// <param name="ingredient"></param>
        /// <param name="quantity"></param>
        public void Remove(IngredientType ingredient, int quantity)
        {
            if (_internalList.Any(i => i.Ingredient == ingredient && i.Quantity - quantity > 0))
            {
                Add(ingredient, -quantity);
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
        public void RemoveAll(IngredientType ingredient)
        {
            if (_internalList.Any(i => i.Ingredient == ingredient))
            {
                _internalList.Remove(_internalList.Find(i => i.Ingredient == ingredient));
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
