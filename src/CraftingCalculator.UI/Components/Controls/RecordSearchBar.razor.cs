using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;

// MudBlazor.Color and Microsoft.Maui.Graphics.Color are both in scope in this project's global usings.
using Color = MudBlazor.Color;

namespace CraftingCalculator.UI.Components.Controls;

/// <summary>
/// The search field and category filter shared by <see cref="Pages.DatasetList" /> and
/// <see cref="Dialogs.BlueprintPickerDialog" />. It owns the filter and raises
/// <see cref="FilterChanged" />; the host applies it to its own records with
/// <see cref="RecordFilterProcessor.Apply{T}" />.
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

    /// <summary>Placeholder text for the search field.</summary>
    [Parameter] public string Placeholder { get; set; } = "Search";

    /// <summary>Raised whenever the search text or the selected categories change.</summary>
    [Parameter] public EventCallback<RecordFilter> FilterChanged { get; set; }

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
        // record in it is recategorised, or the category itself is deleted. The stale id would go on
        // filtering with no chip left to explain why, so it is dropped and the host re-notified.
        RecordFilter pruned = RecordFilterProcessor.Prune(_filter, Records);

        if (!ReferenceEquals(pruned, _filter))
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
        SetFilterAsync(_filter with { CategoryIds = new HashSet<int>(categoryIds ?? []) });

    private Task RemoveAsync(int categoryId)
    {
        HashSet<int> remaining = [.. _filter.CategoryIds];
        remaining.Remove(categoryId);

        return SetFilterAsync(_filter with { CategoryIds = remaining });
    }

    private async Task SetFilterAsync(RecordFilter filter)
    {
        _filter = filter;
        await FilterChanged.InvokeAsync(_filter);
    }
}
