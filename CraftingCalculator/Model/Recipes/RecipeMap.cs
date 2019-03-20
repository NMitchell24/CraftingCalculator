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

    }
}
