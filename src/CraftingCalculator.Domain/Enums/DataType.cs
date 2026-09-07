using CraftingCalculator.Domain.Models;
using System;
// Aliased rather than importing System.ComponentModel wholesale: that namespace also has a
// Component type, which is ambiguous with Models.Component in GetDataRecord below.
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;
using System.Reflection;

namespace CraftingCalculator.Domain.Enums;

public enum DataType
{
    [Description("Component")]
    Component = 0,
    [Description("Category")]
    Category = 1,
    [Description("Blueprint")]
    Blueprint = 2
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
            DataType.Component => new Component(),
            DataType.Blueprint => new Blueprint(),
            DataType.Category => new Category(),
            _ => new Component(),
        };
    }
}
