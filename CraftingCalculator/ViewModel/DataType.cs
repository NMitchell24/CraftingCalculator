using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingCalculator.ViewModel
{
    public static class DataType
    {
        public const string Ingredient = "Ingredient";
        public const string RecipeType = "Recipe Type";
        public const string Recipe = "Recipe";

        public static List<String> DataTypeList
        {
            get
            {
                List<String> _typeList = new List<String>();
                _typeList.Add(Ingredient);
                _typeList.Add(RecipeType);
                _typeList.Add(Recipe);
                return _typeList;
            }

            private set { }
        }
    }
}
