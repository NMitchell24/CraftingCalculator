using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace CraftingCalculator.ViewModel.Recipes
{
    public class RecipeMap
    {
        private List<RecipeQuantity> _internalList = new List<RecipeQuantity>();
        public ReadOnlyCollection<RecipeQuantity> RecipeList
        {
            get => _internalList.AsReadOnly();
            private set { }
        }
        public List<RecipeQuantity> RemovedRecipes = new List<RecipeQuantity>();

        public RecipeMap(RecipeMap map, bool cloneForSave)
        {
            _internalList = new List<RecipeQuantity>();
            foreach(RecipeQuantity r in map.RecipeList)
            {
                if(cloneForSave)
                {
                    _internalList.Add(r.CloneForSave());
                }
                else
                {
                    _internalList.Add(r.Clone());
                }
            }
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
        public void Add(Recipe recipe, long quantity)
        {
            Add(recipe, quantity, 0);
        }

        /// <summary>
        /// Overload to preserve the data Identifier from the database.
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="quantity"></param>
        /// <param name="id"></param>
        public void Add(Recipe recipe, long quantity, int id)
        {
            if (_internalList.Any(i => i.Recipe.Name == recipe.Name))
            {
                _internalList.Find(i => i.Recipe.Name == recipe.Name).Quantity += quantity;
            }
            else
            {
                _internalList.Add(new RecipeQuantity(recipe, quantity, id));
            }
        }

        /// <summary>
        /// Decrement a RecipeQuantity by the provided amount.  
        /// If the current Quantity - the provided quantity would be less than or equal to 0
        /// Then the recipe will be removed.
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="quantity"></param>
        public void Remove(Recipe recipe, long quantity)
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
                RemovedRecipes.AddRange(_internalList.FindAll(i => i.Recipe.Name == recipe.Name));
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

        public RecipeMap Clone()
        {
            return new RecipeMap(this, false);
        }

        public RecipeMap CloneForSave()
        {
            return new RecipeMap(this, true);
        }
    }
}
