namespace CraftingCalculator.UI.State;

/// <summary>A list that remembers its own category filter in <see cref="CategoryFilterState" />.</summary>
public enum FilterList
{
    /// <summary>Dataset → Blueprints.</summary>
    Blueprints,

    /// <summary>Dataset → Components.</summary>
    Components,

    /// <summary>Dataset → Categories.</summary>
    Categories,

    /// <summary>The picker the blueprint editor adds requirements from.</summary>
    RequirementsPicker,

    /// <summary>The picker the Craft screen adds blueprints to the batch from.</summary>
    CraftPicker
}

/// <summary>
/// The categories each <see cref="FilterList" /> was last filtered by, so leaving a list and coming back
/// finds it filtered the same way. Scoped, so a selection lasts for the app session and is gone after a restart.
/// </summary>
public sealed class CategoryFilterState
{
    private static readonly IReadOnlySet<int> None = new HashSet<int>();

    private readonly Dictionary<FilterList, IReadOnlySet<int>> _selections = [];

    /// <summary>
    /// The category ids last selected on <paramref name="list" />; empty when it has never been filtered. Ids may
    /// include categories the list no longer offers.
    /// </summary>
    public IReadOnlySet<int> Get(FilterList list) => _selections.GetValueOrDefault(list, None);

    public void Set(FilterList list, IReadOnlySet<int> categoryIds) => _selections[list] = categoryIds;
}
