using CraftingCalculator.Domain.Constants;

namespace CraftingCalculator.Application.Common.Interfaces;

/// <summary>The user's export preferences, persisted across launches.</summary>
public interface IExportSettings
{
    /// <summary>
    /// The most exports the app keeps on the device, counting the one being saved. Between
    /// <see cref="ExportConstants.MinimumKeptExports"/> and <see cref="ExportConstants.MaximumKeptExports"/>.
    /// </summary>
    int KeptExports { get; }

    /// <summary>
    /// Stores <paramref name="count"/> as <see cref="KeptExports"/>, clamped to the range between
    /// <see cref="ExportConstants.MinimumKeptExports"/> and <see cref="ExportConstants.MaximumKeptExports"/>.
    /// </summary>
    void SetKeptExports(int count);
}
