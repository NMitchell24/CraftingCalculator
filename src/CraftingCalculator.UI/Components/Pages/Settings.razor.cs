using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Pages;

public partial class Settings : ComponentBase, IDisposable
{
    [Inject] private PageShellState PageShellState { get; set; } = null!;
    [Inject] private ThemeState ThemeState { get; set; } = null!;
    [Inject] private IExportSettings ExportSettings { get; set; } = null!;

    private static string Version => $"{AppInfo.Current.VersionString} ({AppInfo.Current.BuildString})";

    // No back arrow: Settings is an app-bar overlay, and the Settings icon that opened it is what closes it.
    protected override void OnInitialized() => PageShellState.Configure(this, new PageShellConfig("Settings"));

    public void Dispose() => PageShellState.Reset(this);
}
