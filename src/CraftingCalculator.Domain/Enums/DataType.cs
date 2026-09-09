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
        Type type = value.GetType();
        string? name = Enum.GetName(type, value);
        if (name != null
            && type.GetField(name) is { } field
            && Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
        {
            return attr.Description;
        }
        return "";
    }

    public static IBaseDataRecord GetDataRecord(this Enum value)
    {
        return value switch
        {
            DataType.Component => new ComponentModel(),
            DataType.Blueprint => new BlueprintModel(),
            DataType.Category => new CategoryModel(),
            _ => new ComponentModel(),
        };
    }
}
