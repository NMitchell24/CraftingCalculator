using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;

// MudBlazor.Color and Microsoft.Maui.Graphics.Color are both in scope in this project's global usings.
using Color = MudBlazor.Color;

namespace CraftingCalculator.UI.Components.Controls;

/// <summary>
/// The search field and category filter shared by <see cref="Pages.DatasetList" />
/// and <see cref="Dialogs.RecordPickerDialog" />. It owns the filter and raises
/// <see cref="FilterChanged" />; the host applies it to its own records with
/// <see cref="RecordFilterProcessor.Apply{T}" />. The selected categories are remembered per
/// <see cref="FilterList" /> for the app session; the search text is not.
/// </summary>
public partial class RecordSearchBar : ComponentBase
{
    /// <summary>The label of the entry that selects records filed under no category.</summary>
    private const string UncategorizedLabel = "Uncategorized";

    /// <summary>
    /// Every record the host is listing, before filtering. The categories on offer are derived from
    /// these, so passing the filtered list instead would collapse the menu to the current selection.
    /// </summary>
    [Parameter, EditorRequired] public IReadOnlyList<IBaseDataRecord> Records { get; set; } = [];

    /// <summary>Raised whenever the search text or the selected categories change.</summary>
    [Parameter] public EventCallback<RecordFilter> FilterChanged { get; set; }

    /// <summary>The list whose category selection this bar restores and remembers.</summary>
    [Parameter, EditorRequired] public FilterList FilterList { get; set; }

    [Inject] private CategoryFilterState CategoryFilterState { get; set; } = null!;

    private RecordFilter _filter = RecordFilter.Empty;

    // Uncategorized first, then the categories by name: the order both the menu and the active chips
    // render in. Held rather than derived in the markup, because building it is a full pass over
    // Records and the markup reads it twice per render.
    private List<(int Id, string Label)> _options = [];
    private bool _showFilter;

    protected override async Task OnParametersSetAsync()
    {
        List<CategoryModel> categoriesInUse = RecordFilterProcessor.CategoriesInUse(Records);

        _options = [];

        if (RecordFilterProcessor.HasUncategorized(Records))
        {
            _options.Add((RecordFilter.UncategorizedId, UncategorizedLabel));
        }

        _options.AddRange(categoriesInUse.Select(category => (category.Id, category.Name ?? "")));

        // Offering the filter with no category in use would leave a menu whose only entry selects
        // everything. The Categories list falls out of this for free, because CategoryModel is not an
        // ICategorizedRecord and so contributes nothing to CategoriesInUse.
        _showFilter = categoriesInUse.Count > 0;

        // Records change as the user edits, so a selected category can stop being offered - the last
        // record in it is recategorized, or the category itself is deleted. The stale id would go on
        // filtering with no chip left to explain why, so it is dropped and the host re-notified.
        // The remembered selection is pruned rather than _filter: DatasetList renders this bar with no
        // Records while it loads, and pruning _filter against that would lose the selection before the
        // records arrive.
        RecordFilter pruned = RecordFilterProcessor.Prune(
            _filter with { CategoryIds = CategoryFilterState.Get(FilterList) }, Records);

        if (!pruned.CategoryIds.SetEquals(_filter.CategoryIds))
        {
            _filter = pruned;
            await FilterChanged.InvokeAsync(_filter);
        }
    }

    /// <summary>Tertiary marks the one entry that is not a category the user created.</summary>
    private static Color ColorFor(int categoryId) =>
        categoryId == RecordFilter.UncategorizedId ? Color.Tertiary : Color.Primary;

    private Task OnSearchChangedAsync(string? search) =>
        SetFilterAsync(_filter with { Search = search ?? "" });

    private Task OnCategoriesChangedAsync(IReadOnlyCollection<int>? categoryIds) =>
        SetCategoriesAsync(new HashSet<int>(categoryIds ?? []));

    private Task RemoveAsync(int categoryId)
    {
        HashSet<int> remaining = [.. _filter.CategoryIds];
        remaining.Remove(categoryId);

        return SetCategoriesAsync(remaining);
    }

    private Task SetCategoriesAsync(IReadOnlySet<int> categoryIds)
    {
        CategoryFilterState.Set(FilterList, categoryIds);
        return SetFilterAsync(_filter with { CategoryIds = categoryIds });
    }

    private async Task SetFilterAsync(RecordFilter filter)
    {
        _filter = filter;
        await FilterChanged.InvokeAsync(_filter);
    }
}
