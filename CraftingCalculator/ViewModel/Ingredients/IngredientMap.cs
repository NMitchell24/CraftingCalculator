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
        public List<IngredientQuantity> RemovedIngredients = new List<IngredientQuantity>();

        public IngredientMap(IngredientMap map, bool cloneForSave)
        {
            _internalList = new List<IngredientQuantity>();
            foreach(IngredientQuantity i in map.IngredientList)
            {
                // Insures we don't retain references to the original IngredientQuantity object and cause those 
                // objects to get mutated elsewhere.
                if(cloneForSave)
                {
                    _internalList.Add(i.CloneForSave());
                }
                else
                {
                    _internalList.Add(i.Clone());
                }
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
        public void Add(Ingredient ingredient, long quantity)
        {
            Add(ingredient, quantity, 0);
        }

        public void Add(Ingredient ingredient, long quantity, int id)
        {
            if (_internalList.Any(i => i.Name == ingredient.Name))
            {
                _internalList.Find(i => i.Name == ingredient.Name).Quantity += quantity;
            }
            else
            {
                _internalList.Add(new IngredientQuantity(ingredient, quantity, id));
            }
        }

        /// <summary>
        /// Decrement an IngredientQuantity by the provided amount.  
        /// If the current Quantity - the provided quantity would be less than or equal to 0
        /// Then the Ingredient will be removed.
        /// </summary>
        /// <param name="ingredient"></param>
        /// <param name="quantity"></param>
        public void Remove(Ingredient ingredient, long quantity)
        {
            if (_internalList.Any(i => i.Name == ingredient.Name && i.Quantity - quantity > 0))
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
        public void RemoveAll(Ingredient ingredient)
        {
            if (_internalList.Any(i => i.Name == ingredient.Name))
            {
                RemovedIngredients.Add(_internalList.Find(i => i.Name == ingredient.Name));
                _internalList.Remove(_internalList.Find(i => i.Name == ingredient.Name));
            }
        }

        /// <summary>
        /// Reset the contents of the map and empty it.
        /// </summary>
        public void Reset()
        {
            _internalList.Clear();
        }

        /// <summary>
        /// Return a clone of this map.
        /// </summary>
        /// <returns></returns>
        public IngredientMap Clone()
        {
            return new IngredientMap(this, false);
        }

        /// <summary>
        /// Return a clone of this map with the ID values on the internal IngredientQuantity objects cleared.
        /// </summary>
        /// <returns></returns>
        public IngredientMap CloneForSave()
        {
            return new IngredientMap(this, true);
        }
    }
}
