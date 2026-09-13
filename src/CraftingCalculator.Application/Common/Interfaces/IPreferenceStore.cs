namespace CraftingCalculator.Application.Common.Interfaces;

/// <summary>
/// Abstraction over platform-persisted user preferences.
/// Implemented in the UI project against Microsoft.Maui.Storage.Preferences so Application has no
/// MAUI dependency.
/// </summary>
public interface IPreferenceStore
{
    string? Get(string key);

    void Set(string key, string value);
}
