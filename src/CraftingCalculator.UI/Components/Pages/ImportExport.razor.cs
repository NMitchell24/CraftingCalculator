using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Pages;

/// <summary>
/// The Import/Export hub, opened from the Dataset landing page: one card for exporting records from the
/// current dataset to a file, and one for importing records from a file.
/// </summary>
public partial class ImportExport : ComponentBase, IDisposable
{
    /// <summary>One card, and the screen it opens.</summary>
    private sealed record TransferCard(string Title, string Icon, string Description, string Route);

    private static readonly TransferCard[] Cards =
    [
        new("Export Data", Icons.Material.Filled.Output,
            "Export any of your Categories, Components, Blueprints, or Favorites to back them up or share with friends.",
            "/dataset/import-export/export"),
        new("Import Data", Icons.Material.Filled.Input,
            "Import Categories, Components, Blueprints, or Favorites from a backup file that you created, or one that a friend shared with you.",
            "/dataset/import-export/import")
    ];

    [Inject] private PageShellState PageShellState { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    protected override void OnInitialized() =>
        PageShellState.Configure(this, new PageShellConfig("Import and Export") { ShowBack = true });

    private void Open(TransferCard card) => Navigation.NavigateTo(card.Route);

    public void Dispose() => PageShellState.Reset(this);
}
