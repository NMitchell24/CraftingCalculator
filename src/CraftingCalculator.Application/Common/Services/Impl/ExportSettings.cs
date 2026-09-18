using System.Globalization;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Constants;

namespace CraftingCalculator.Application.Common.Services.Impl;

/// <summary>
/// The export preferences, stored through <see cref="IPreferenceStore"/> the same way
/// <see cref="SelectedDatasetState"/> stores the selected dataset. Singleton: the Settings screen sets the
/// value and <see cref="DatasetTransferService"/> reads it on the next export.
/// </summary>
public sealed class ExportSettings : IExportSettings
{
    private const string KeptExportsKey = "kept_exports";

    private readonly IPreferenceStore _preferences;

    public ExportSettings(IPreferenceStore preferences)
    {
        _preferences = preferences;

        // Clamped on the way in as well as out: the stored value survives a release that narrows the range.
        KeptExports = int.TryParse(preferences.Get(KeptExportsKey), CultureInfo.InvariantCulture, out int stored)
            ? Clamp(stored)
            : ExportConstants.DefaultKeptExports;
    }

    public int KeptExports { get; private set; }

    public void SetKeptExports(int count)
    {
        int clamped = Clamp(count);

        if (clamped == KeptExports)
        {
            return;
        }

        KeptExports = clamped;
        _preferences.Set(KeptExportsKey, clamped.ToString(CultureInfo.InvariantCulture));
    }

    private static int Clamp(int count) =>
        Math.Clamp(count, ExportConstants.MinimumKeptExports, ExportConstants.MaximumKeptExports);
}
