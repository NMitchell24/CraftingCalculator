using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
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

    [Inject] private IDatasetService DatasetService { get; set; } = null!;
    [Inject] private AppBarState AppBarState { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

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
            ? await DatasetService.GetRecordAsync(_type, Id)
            : CopyFrom > 0
                ? (await DatasetService.GetRecordAsync(_type, CopyFrom))?.CopyForSave()
                : _type.GetDataRecord();

        if (_record is null)
        {
            Navigation.NavigateTo("/dataset");
            return;
        }

        AppBarState.Configure(this, new AppBarConfig(Title())
        {
            MenuItems = MenuItems(),
            BackHref = ListHref,
            TitleIsUserContent = Id > 0
        });
    }

    private string Title() => Id > 0 ? _record?.Name ?? "" : $"New {_type.GetDescription()}";

    private IReadOnlyList<AppBarMenuItem> MenuItems() => Id > 0
        ? [new AppBarMenuItem("Delete", Icons.Material.Filled.Delete, DeleteAsync)]
        : [];

    private void MarkDirty() => _isDirty = true;

    private void Cancel() => Navigation.NavigateTo(ListHref);

    private async Task SaveAsync()
    {
        await DatasetService.SaveRecordAsync(_record);

        _isDirty = false;
        Snackbar.Add($"Saved '{_record?.Name}'", Severity.Success);
        Navigation.NavigateTo(ListHref);
    }

    private async Task DeleteAsync()
    {
        if (_record is null || !await DatasetPrompts.ConfirmDeleteAsync(DialogService, _record))
        {
            return;
        }

        await DatasetService.DeleteRecordAsync(_record);

        _isDirty = false;
        Snackbar.Add($"Deleted '{_record.Name}'", Severity.Success);
        Navigation.NavigateTo(ListHref);
    }

    /// <summary>
    /// Guards the back arrow, Cancel, and the bottom nav / side rail alike. The WPF app discarded
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

        bool? discard = await DialogService.ShowMessageBoxAsync(
            "Discard changes?",
            "Your edits have not been saved.",
            yesText: "Discard", cancelText: "Keep editing");

        if (discard == true)
        {
            _isDirty = false;
            Navigation.NavigateTo(context.TargetLocation);
        }
    }

    public void Dispose()
    {
        _navigationGuard?.Dispose();
        AppBarState.Reset(this);
    }
}
