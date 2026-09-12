using CraftingCalculator.Application.Common.Interfaces;
using System.Globalization;

namespace CraftingCalculator.Application.Common.Services.Impl;

/// <summary>
/// The selected dataset, stored through <see cref="IPreferenceStore"/> the same way
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

    public void Set(int id)
    {
        if (id == Id)
        {
            return;
        }

        Id = id;
        _preferences.Set(SelectedDatasetKey, id.ToString(CultureInfo.InvariantCulture));
    }
}
