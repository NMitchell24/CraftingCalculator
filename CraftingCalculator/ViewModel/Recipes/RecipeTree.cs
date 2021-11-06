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
        public string Tooltip { get; set; }

        public RecipeTree()
        {
            RecipeNodes = new ObservableCollection<RecipeTree>();
        }

        public RecipeTree(string name, string tooltip)
        {
            Name = name;
            Tooltip = tooltip;
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

        public void ExpandCollapseAll(bool isExpand)
        {
            this.IsNodeExpanded = isExpand;
            foreach(RecipeTree node in RecipeNodes)
            {
                node.ExpandCollapseAll(isExpand);
            }
        }
    }
}
