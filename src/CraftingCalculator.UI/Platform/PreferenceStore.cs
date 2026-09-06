using CraftingCalculator.Application.Common.Interfaces;

namespace CraftingCalculator.UI.Platform;

public class PreferenceStore : IPreferenceStore
{
    public string? Get(string key) => Preferences.Default.Get<string?>(key, null);

    public void Set(string key, string value) => Preferences.Default.Set(key, value);
}
