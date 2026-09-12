namespace CraftingCalculator.Application.Common.Interfaces;

/// <summary>
/// The dataset the app is currently showing, persisted across launches. Every read and write through
/// <c>CraftingDataContext</c> is scoped to it.
/// </summary>
public interface ISelectedDatasetState
{
    /// <summary>The selected dataset's id, or 0 before startup has resolved one.</summary>
    int Id { get; }

    /// <summary>Selects <paramref name="id"/> and persists it across launches.</summary>
    void Set(int id);
}
