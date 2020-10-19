using CraftingCalculator.ViewModel.Ingredients;
using CraftingCalculator.ViewModel.Recipes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CraftingCalculator.ViewModel
{
    public enum DataType
    {
        [Description("Ingredient")]
        Ingredient = 0,
        [Description("Recipe Type")]
        RecipeType = 1,
        [Description("Recipe")]
        Recipe = 2
    }

    public static class DurationExtensions
    {
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }

        public static IBaseDataRecord GetDataRecord(this Enum value)
        {
            switch (value)
            {
                case DataType.Ingredient:
                    return new Ingredient();
                case DataType.Recipe:
                    return new Recipe();
                case DataType.RecipeType:
                    return new RecipeFilter();
                default:
                    return null;
            }
        }
    }
}
