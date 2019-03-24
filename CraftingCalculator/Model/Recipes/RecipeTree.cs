using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CraftingCalculator.Model.Recipes
{
    public class RecipeTree
    {
        RecipeTree parent;
        public string Name { get; set; }
        public ObservableCollection<RecipeTree> RecipeNodes { get; set; }

        public RecipeTree()
        {
            RecipeNodes = new ObservableCollection<RecipeTree>();
        }

        public RecipeTree(string name)
        {
            Name = name;
            RecipeNodes = new ObservableCollection<RecipeTree>();
        }

        public void AddRecipeNode(RecipeTree r)
        {
            RecipeNodes.Add(r);
        }

        public void SetParent()
        {
            foreach (RecipeTree tree in RecipeNodes)
            {
                tree.parent = this;
                tree.SetParent();
            }
        }
    }
}
