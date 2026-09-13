namespace CraftingCalculator.Domain.Enums;

/// <summary>
/// The four kinds of record a dataset holds, as import and export see them. Declared in the order a
/// dataset has to be built: a component is filed under a category, a blueprint uses components, and a
/// favorite holds blueprints.
/// </summary>
public enum RecordKind
{
    Category,
    Component,
    Blueprint,
    Favorite
}
