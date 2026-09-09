using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

/// <summary>
/// One record's category, rendered as a chip. Renders nothing when the record has no category, or is
/// of a type that is not filed under one, so a call site can place it unconditionally.
/// </summary>
public partial class CategoryChip : ComponentBase
{
    /// <summary>The blueprint or component whose category is shown.</summary>
    [Parameter, EditorRequired] public IBaseDataRecord Record { get; set; } = null!;

    /// <summary>Positioning class for the chip within the row it sits in.</summary>
    [Parameter] public string? Class { get; set; }

    private string? CategoryName => (Record as ICategorizedRecord)?.Category?.Name;
}
