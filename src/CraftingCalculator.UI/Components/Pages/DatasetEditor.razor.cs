using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Pages;

public partial class DatasetEditor : ComponentBase, IDisposable
{
    /// <summary>The <see cref="DataType"/> being edited, as its enum name.</summary>
    [Parameter] public string Type { get; set; } = "";

    /// <summary>The record's id, or 0 to create a new one.</summary>
    [Parameter] public int Id { get; set; }

    /// <summary>Id of the record this one starts as a copy of. Only read when <see cref="Id"/> is 0.</summary>
    [Parameter, SupplyParameterFromQuery(Name = "copyFrom")] public int CopyFrom { get; set; }

    [Inject] private IRecordService RecordService { get; set; } = null!;
    [Inject] private PageShellState PageShellState { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] private IJSRuntime Js { get; set; } = null!;

    private IBaseDataRecord? _record;
    private DataType _type;
    private bool _isDirty;
    private IDisposable? _navigationGuard;

    private string ListHref => $"/dataset/{_type}";

    protected override void OnInitialized()
    {
        _navigationGuard = Navigation.RegisterLocationChangingHandler(ConfirmDiscardAsync);
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!Enum.TryParse(Type, ignoreCase: true, out _type))
        {
            Navigation.NavigateTo("/dataset");
            return;
        }

        _record = Id > 0
            ? await RecordService.GetRecordAsync(_type, Id)
            : CopyFrom > 0
                ? (await RecordService.GetRecordAsync(_type, CopyFrom))?.CopyForSave()
                : _type.GetDataRecord();

        if (_record is null)
        {
            Navigation.NavigateTo("/dataset");
            return;
        }

        PageShellState.Configure(this, new PageShellConfig(Title())
        {
            ShowBack = true,
            TitleIsUserContent = Id > 0,
            ConfirmLeaveAsync = ConfirmLeaveAsync
        });
    }

    private string Title() => Id > 0 ? _record?.Name ?? "" : $"New {_type.GetDescription()}";

    private void MarkDirty() => _isDirty = true;

    // The editor is only ever opened from its list, so stepping back returns there and takes the editor out
    // of history. A NavigateTo would push the list on top instead, leaving the editor behind it for the
    // system back gesture to reopen.
    private async Task ReturnToListAsync() => await Js.InvokeVoidAsync("history.back");

    private async Task SaveAsync()
    {
        if (_record is null || !await DatasetPrompts.ConfirmSaveAsync(DialogService, _record))
        {
            return;
        }

        await RecordService.SaveRecordAsync(_record);

        _isDirty = false;
        Snackbar.Add($"Saved '{_record.Name}'", Severity.Success);
        await ReturnToListAsync();
    }

    private async Task DeleteAsync()
    {
        if (_record is null || !await DatasetPrompts.ConfirmDeleteAsync(DialogService, _record))
        {
            return;
        }

        await RecordService.DeleteRecordAsync(_record);

        _isDirty = false;
        Snackbar.Add($"Deleted '{_record.Name}'", Severity.Success);
        await ReturnToListAsync();
    }

    /// <summary>
    /// Guards Cancel, the back arrow, the system back gesture, and opening Help or Settings over the page. The
    /// bottom nav / side rail ask through <see cref="ConfirmLeaveAsync"/> instead. The WPF app discarded
    /// in-progress edits silently whenever the selection changed.
    /// </summary>
    private async ValueTask ConfirmDiscardAsync(LocationChangingContext context)
    {
        if (!_isDirty)
        {
            return;
        }

        // Has to happen before the first await - once the handler yields it is too late to stop the
        // navigation.
        context.PreventNavigation();

        if (!await ConfirmLeaveAsync())
        {
            return;
        }

        // The list is the entry beneath this one, so only a step back targets it. Opening Help or Settings
        // replaces the editor's entry instead, or back from there would reopen the edits just discarded.
        // TargetLocation stays relative when the navigation came from a NavigateTo call.
        if (Navigation.ToAbsoluteUri(context.TargetLocation).AbsolutePath
            .Equals(ListHref, StringComparison.OrdinalIgnoreCase))
        {
            await ReturnToListAsync();
        }
        else
        {
            Navigation.NavigateTo(context.TargetLocation, replace: true);
        }
    }

    /// <summary>True when there are no unsaved edits, or the user chose to discard them.</summary>
    private async Task<bool> ConfirmLeaveAsync()
    {
        if (!_isDirty)
        {
            return true;
        }

        bool? discard = await DialogService.ShowMessageBoxAsync(
            "Discard changes?",
            "Your edits have not been saved.",
            yesText: "Discard", cancelText: "Keep editing");

        _isDirty = discard != true;

        return !_isDirty;
    }

    public void Dispose()
    {
        _navigationGuard?.Dispose();
        PageShellState.Reset(this);
    }
}
