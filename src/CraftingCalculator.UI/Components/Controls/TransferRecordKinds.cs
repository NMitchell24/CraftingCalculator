using CraftingCalculator.Domain.Enums;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Controls;

/// <summary>The heading and icon the export and import screens show for each kind of record.</summary>
public static class TransferRecordKinds
{
    /// <summary>One kind of record, with its heading and icon.</summary>
    public sealed record Display(RecordKind Kind, string Title, string Icon);

    /// <summary>Every kind, in <see cref="RecordKind"/> order.</summary>
    // The Dataset landing page's icons for the same record types (Dataset.razor.cs).
    public static readonly IReadOnlyList<Display> All =
    [
        new(RecordKind.Category, "Categories", Icons.Material.Filled.Label),
        new(RecordKind.Component, "Components", Icons.Material.Filled.Inventory2),
        new(RecordKind.Blueprint, "Blueprints", Icons.Material.Filled.Handyman),
        new(RecordKind.Favorite, "Favorites", Icons.Material.Filled.Star)
    ];
}
