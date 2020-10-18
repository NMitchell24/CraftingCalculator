using System.Collections.ObjectModel;
using System.Linq;

namespace CraftingCalculator.ViewModel.Recipes
{
    public class RecipeTree
    {
        RecipeTree parent;
        public string Name { get; set; }
        public ObservableCollection<RecipeTree> RecipeNodes { get; set; }
        public bool IsNodeExpanded { get; set; }
        public string Id { get; set; }

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

        public void SetExpandedNodes(RecipeTree tree)
        {
            if(Id == tree.Id)
            {
                IsNodeExpanded = tree.IsNodeExpanded;
            }
            foreach(RecipeTree node in tree.RecipeNodes)
            {
                RecipeTree thisNode = RecipeNodes.Where(x => x.Id == node.Id).FirstOrDefault();
                if(thisNode != null)
                {
                    thisNode.SetExpandedNodes(node);
                }
            }
        }

        public void ExpandAll()
        {
            this.IsNodeExpanded = true;
            foreach(RecipeTree node in RecipeNodes)
            {
                node.ExpandAll();
            }
        }

        public void CollapseAll()
        {
            this.IsNodeExpanded = false;
            foreach(RecipeTree node in RecipeNodes)
            {
                node.CollapseAll();
            }
        }
    }
}
