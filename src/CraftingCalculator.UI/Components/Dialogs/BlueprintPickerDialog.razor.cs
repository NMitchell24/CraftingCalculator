using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

public partial class BlueprintPickerDialog : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private IBlueprintService BlueprintService { get; set; } = null!;
    [Inject] private IBlueprintFilterService BlueprintFilterService { get; set; } = null!;

    private List<Blueprint> _blueprints = [];
    private List<BlueprintFilter> _filters = [];
    private IReadOnlyCollection<Blueprint> _selected = [];
    private string _search = "";
    private string _selectedFilterName = BlueprintFilter.ALL;

    private List<Blueprint> _filteredBlueprints =>
        [.. _blueprints.Where(r =>
            (_selectedFilterName == BlueprintFilter.ALL || r.Filter?.Name == _selectedFilterName) &&
            (string.IsNullOrWhiteSpace(_search) || (r.Name?.Contains(_search, StringComparison.OrdinalIgnoreCase) ?? false)))];

    protected override async Task OnInitializedAsync()
    {
        _blueprints = await BlueprintService.GetAllBlueprintsAsync();

        // The seeded "All" row is the sentinel this dialog already renders as its own first chip, not
        // a real category - excluded here the same way WPF dropped it positionally from
        // ConfigureBlueprintsViewModel's filter list. Matched on id rather than name so a user category
        // of their own called "All" (which the Library screen lets them create) still shows up.
        _filters = [.. (await BlueprintFilterService.GetBlueprintFiltersAsync())
            .Where(f => f.Id != DatabaseSeedConstants.AllFilterId)];
    }

    private void OnFilterChanged(string filterName) => _selectedFilterName = filterName;

    private void Confirm() => MudDialog.Close(DialogResult.Ok(_selected));

    private void Cancel() => MudDialog.Cancel();
}
