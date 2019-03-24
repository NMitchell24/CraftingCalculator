using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace CraftingCalculator.Model.Recipes
{
    public class RecipeMap
    {
        private List<RecipeQuantity> _internalList = new List<RecipeQuantity>();
        public ReadOnlyCollection<RecipeQuantity> RecipeList
        {
            get => _internalList.AsReadOnly();
            private set { }
        }

        public RecipeMap(RecipeMap map)
        {
            _internalList = new List<RecipeQuantity>(map.RecipeList);
        }

        public RecipeMap()
        {
            //nothing
        }

        /// <summary>
        /// Adds the Recipe and quantity to the list if it does not exist.
        /// If it does exist then it will just increase the quantity of the existing record
        /// by the quantity that is passed into this Add function.
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="quantity"></param>
        public void Add(Recipe recipe, int quantity)
        {
            if (_internalList.Any(i => i.Recipe.Name == recipe.Name))
            {
                _internalList.Find(i => i.Recipe.Name == recipe.Name).Quantity += quantity;
            }
            else
            {
                _internalList.Add(new RecipeQuantity(recipe, quantity));
            }
        }

        public void AddifDoesNotExist(Recipe recipe, int quantity)
        {
            if (!_internalList.Any(i => i.Recipe.Name == recipe.Name))
            {
                _internalList.Add(new RecipeQuantity(recipe, quantity));
            }
        }

        /// <summary>
        /// Decrement a RecipeQuantity by the provided amount.  
        /// If the current Quantity - the provided quantity would be less than or equal to 0
        /// Then the recipe will be removed.
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="quantity"></param>
        public void Remove(Recipe recipe, int quantity)
        {
            if(_internalList.Any(i => i.Recipe.Name == recipe.Name && i.Quantity - quantity > 0))
            {
                Add(recipe, -quantity);
            }
            else
            {
                RemoveAll(recipe);
            }
            
        }

        /// <summary>
        /// Remove a RecipeQuantity from the list entirely if it exists.
        /// </summary>
        /// <param name="recipe"></param>
        public void RemoveAll(Recipe recipe)
        {
            if(_internalList.Any(i => i.Recipe.Name == recipe.Name)) {
                _internalList.Remove(_internalList.Find(i => i.Recipe.Name == recipe.Name));
            }
        }

        /// <summary>
        /// Reset the Map and clear all RecipeQuantity objects
        /// </summary>
        public void Reset()
        {
            _internalList.Clear();
        }
    }
}
