using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

/// <summary>
/// The dataset the app is currently showing, persisted across launches, and that dataset's settings. Every read and
/// write through <c>CraftingDataContext</c> is scoped to it.
/// </summary>
public interface ISelectedDatasetState
{
    /// <summary>The selected dataset's id, or 0 before startup has resolved one.</summary>
    int Id { get; }

    /// <summary>The selected dataset's settings, or <see cref="Datasettings.Default"/> before startup has resolved one.</summary>
    Datasettings Settings { get; }

    /// <summary>Selects <paramref name="id"/>, whose settings are <paramref name="settings"/>, and persists the id across launches.</summary>
    void Set(int id, Datasettings settings);
}
