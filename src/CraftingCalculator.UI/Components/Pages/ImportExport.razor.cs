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
    private sealed record TransferCard(string Title, string Icon, string Description);

    private static readonly TransferCard[] Cards =
    [
        new("Export Data", Icons.Material.Filled.Output,
            "Export any of your Categories, Components, Blueprints, or Favorites to back them up or share with friends."),
        new("Import Data", Icons.Material.Filled.Input,
            "Import Categories, Components, Blueprints, or Favorites from a backup file that you created, or one that a friend shared with you.")
    ];

    [Inject] private PageShellState PageShellState { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    protected override void OnInitialized() =>
        PageShellState.Configure(this, new PageShellConfig("Import/Export Data") { BackHref = "/dataset" });

    private void ShowComingSoon() => Snackbar.Add("Coming Soon!", Severity.Info);

    public void Dispose() => PageShellState.Reset(this);
}
