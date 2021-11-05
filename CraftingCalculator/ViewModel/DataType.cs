using CraftingCalculator.ViewModel.Ingredients;
using CraftingCalculator.ViewModel.Recipes;
using System;
using System.ComponentModel;
using System.Reflection;

namespace CraftingCalculator.ViewModel
{
    public enum DataType
    {
        [Description("Ingredient")]
        Ingredient = 0,
        [Description("Recipe Category")]
        RecipeFilter = 1,
        [Description("Recipe")]
        Recipe = 2
    }

    public static class DataTypeExtensions
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
                case DataType.RecipeFilter:
                    return new RecipeFilter();
                default:
                    return null;
            }
        }
    }
}
