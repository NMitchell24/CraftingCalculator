using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

public partial class BlueprintPickerDialog : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private IBlueprintService BlueprintService { get; set; } = null!;

    private List<BlueprintModel> _blueprints = [];
    private IReadOnlyCollection<BlueprintModel> _selected = [];
    private RecordFilter _filter = RecordFilter.Empty;

    private List<BlueprintModel> FilteredBlueprints => RecordFilterProcessor.Apply(_blueprints, _filter);

    protected override async Task OnInitializedAsync()
    {
        _blueprints = await BlueprintService.GetAllBlueprintsAsync();
    }

    private void OnFilterChanged(RecordFilter filter) => _filter = filter;

    private void Confirm() => MudDialog.Close(DialogResult.Ok(_selected));

    private void Cancel() => MudDialog.Cancel();
}
