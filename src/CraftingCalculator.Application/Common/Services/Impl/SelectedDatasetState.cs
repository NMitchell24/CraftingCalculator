using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using System.Globalization;

namespace CraftingCalculator.Application.Common.Services.Impl;

/// <summary>
/// The selected dataset, its id stored through <see cref="IPreferenceStore"/> the same way
/// <c>ThemeState</c> stores the theme mode. Singleton: startup resolves it in its own scope, which is
/// not the scope <c>BlazorWebView</c> holds for the session.
/// </summary>
public sealed class SelectedDatasetState : ISelectedDatasetState
{
    private const string SelectedDatasetKey = "selectedDatasetId";

    private readonly IPreferenceStore _preferences;

    public SelectedDatasetState(IPreferenceStore preferences)
    {
        _preferences = preferences;

        // An absent or unparseable value leaves Id at 0, which DatasetService.InitializeAsync treats
        // the same way it treats a stored id whose dataset has since been deleted.
        Id = int.TryParse(preferences.Get(SelectedDatasetKey), out int stored) ? stored : 0;
    }

    public int Id { get; private set; }

    public Datasettings Settings { get; private set; } = Datasettings.Default;

    public void Set(int id, Datasettings settings)
    {
        // Held here, not read from the database: the settings live in the Datasets row, and DatasetService is what
        // reads it whenever it selects a dataset or changes the selected one's settings.
        Settings = settings;

        if (id == Id)
        {
            return;
        }

        Id = id;
        _preferences.Set(SelectedDatasetKey, id.ToString(CultureInfo.InvariantCulture));
    }
}
