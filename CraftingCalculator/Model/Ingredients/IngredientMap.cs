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
    }
}
