using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Applies a <see cref="RecordFilter"/> to a list of records, and derives what that list can be
/// filtered by. Every searchable list in the app filters through here, so the predicate has one home.
/// </summary>
public static class RecordFilterProcessor
{
    /// <summary>
    /// The records matching <paramref name="filter"/>, in their original order. A record matches when
    /// it satisfies the search text and the selected categories together.
    /// </summary>
    public static List<T> Apply<T>(IEnumerable<T> records, RecordFilter filter) where T : IBaseDataRecord =>
        [.. records.Where(record => MatchesSearch(record, filter.Search) && MatchesCategory(record, filter.CategoryIds))];

    /// <summary>
    /// The distinct categories <paramref name="records"/> are filed under, ordered by name. These are
    /// the only categories worth offering as a filter: any other selection would match nothing.
    /// </summary>
    public static List<CategoryModel> CategoriesInUse(IEnumerable<IBaseDataRecord> records) =>
        [.. records.OfType<ICategorizedRecord>()
            .Select(record => record.Category)
            .OfType<CategoryModel>()
            .DistinctBy(category => category.Id)
            .OrderBy(category => category.Name)];

    /// <summary>
    /// Whether any of <paramref name="records"/> is filed under no category, which is what
    /// <see cref="RecordFilter.UncategorizedId"/> selects.
    /// </summary>
    public static bool HasUncategorized(IEnumerable<IBaseDataRecord> records) =>
        records.OfType<ICategorizedRecord>().Any(record => record.Category is null);

    /// <summary>
    /// <paramref name="filter"/> with every category id that <paramref name="records"/> no longer
    /// offers dropped. Returns the same instance when there is nothing to drop, so a caller can use
    /// reference equality to decide whether the filter actually changed.
    /// </summary>
    public static RecordFilter Prune(RecordFilter filter, IEnumerable<IBaseDataRecord> records)
    {
        if (filter.CategoryIds.Count == 0)
        {
            return filter;
        }

        // The same set CategoriesInUse and HasUncategorized describe, built in one pass because this
        // runs on every parameter set of the search bar.
        HashSet<int> offered = [];

        foreach (ICategorizedRecord record in records.OfType<ICategorizedRecord>())
        {
            offered.Add(record.Category?.Id ?? RecordFilter.UncategorizedId);
        }

        HashSet<int> kept = [.. filter.CategoryIds.Where(offered.Contains)];

        return kept.Count == filter.CategoryIds.Count ? filter : filter with { CategoryIds = kept };
    }

    private static bool MatchesCategory(IBaseDataRecord record, IReadOnlySet<int> categoryIds)
    {
        if (categoryIds.Count == 0)
        {
            return true;
        }

        // A record that cannot be filed under a category is never excluded by a category filter - the
        // Categories list is the case, and it never shows the filter control anyway.
        return record is not ICategorizedRecord categorized
            || categoryIds.Contains(categorized.Category?.Id ?? RecordFilter.UncategorizedId);
    }

    private static bool MatchesSearch(IBaseDataRecord record, string search) =>
        string.IsNullOrWhiteSpace(search)
        || (record.Name?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false);
}
