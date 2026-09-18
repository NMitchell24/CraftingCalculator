using CraftingCalculator.Application.Common.Interfaces;

namespace CraftingCalculator.Application.UnitTests.Common.Services;

/// <summary>Stands in for MAUI Preferences, which the Application layer never sees directly.</summary>
internal sealed class FakePreferenceStore : IPreferenceStore
{
    private readonly Dictionary<string, string> _values = [];

    public string? Get(string key) => _values.GetValueOrDefault(key);

    public void Set(string key, string value) => _values[key] = value;
}
