namespace CraftingCalculator.Domain.Models;

/// <summary>
/// What a list is currently filtered by. An empty <see cref="CategoryIds"/> means no category filter
/// at all, which is why there is no "All" entry anywhere in the UI.
/// </summary>
public sealed record RecordFilter(string Search, IReadOnlySet<int> CategoryIds)
{
    /// <summary>The id standing for records with no category. No persisted category has id 0.</summary>
    public const int UncategorizedId = 0;

    /// <summary>A filter that excludes nothing.</summary>
    public static RecordFilter Empty { get; } = new("", new HashSet<int>());
}
