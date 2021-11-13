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
            Type? type = value.GetType();
            string? name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo? field = type.GetField(name);
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                    {
                        return attr.Description;
                    }
                }
            }
            return "";
        }

        public static IBaseDataRecord GetDataRecord(this Enum value)
        {
            return value switch
            {
                DataType.Ingredient => new Ingredient(),
                DataType.Recipe => new Recipe(),
                DataType.RecipeFilter => new RecipeFilter(),
                _ => new Ingredient(),
            };
        }
    }
}
