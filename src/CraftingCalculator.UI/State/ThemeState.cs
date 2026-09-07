using CraftingCalculator.Application.Common.Interfaces;

namespace CraftingCalculator.UI.State;

/// <summary>Which palette the app renders in. <see cref="System"/> follows the OS setting.</summary>
public enum ThemeMode
{
    System,
    Light,
    Dark
}

/// <summary>
/// The user's theme choice, persisted across launches. Scoped; <c>MainLayout</c> owns the
/// <c>MudThemeProvider</c> and subscribes to <see cref="Changed"/>, and the Settings page sets the mode.
/// </summary>
public sealed class ThemeState
{
    private const string ModeKey = "theme_mode";

    private readonly IPreferenceStore _preferences;

    public ThemeState(IPreferenceStore preferences)
    {
        _preferences = preferences;

        // An unrecognized or absent stored value falls back to System, which is also the enum default.
        Enum.TryParse(preferences.Get(ModeKey), out ThemeMode mode);
        Mode = mode;
    }

    public ThemeMode Mode { get; private set; }

    public event Action? Changed;

    public void SetMode(ThemeMode mode)
    {
        if (mode == Mode)
        {
            return;
        }

        Mode = mode;
        _preferences.Set(ModeKey, mode.ToString());
        Changed?.Invoke();
    }
}
