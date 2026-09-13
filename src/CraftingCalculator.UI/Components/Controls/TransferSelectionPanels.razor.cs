using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Domain.Models.Transfer;
using CraftingCalculator.UI.Components.Dialogs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Controls;

/// <summary>
/// One expansion panel per kind of record in a <see cref="DatasetSnapshot"/>, for choosing which records to
/// export or import. A tap selects or deselects one record, and the header's button a whole kind; either way
/// the dependency rules in <see cref="TransferSelectionProcessor"/> keep the selection complete, asking first
/// when a tap would change more than was tapped.
/// </summary>
public partial class TransferSelectionPanels : ComponentBase
{
    private sealed record Row(RecordKey Key, string Name);

    // Matches .transfer-row's height in app.css, which is fixed so Virtualize can position rows exactly.
    private const float RowHeight = 48;

    [Inject] private IDialogService DialogService { get; set; } = null!;

    /// <summary>The records to choose from.</summary>
    [Parameter, EditorRequired] public DatasetSnapshot Snapshot { get; set; } = null!;

    /// <summary>The dependency graph of <see cref="Snapshot"/>.</summary>
    [Parameter, EditorRequired] public DependencyGraph Graph { get; set; } = null!;

    /// <summary>The selection, which the panels change in place.</summary>
    [Parameter, EditorRequired] public HashSet<RecordKey> Selected { get; set; } = null!;

    /// <summary>Raised after every change the panels make to <see cref="Selected"/>.</summary>
    [Parameter] public EventCallback SelectionChanged { get; set; }

    private DatasetSnapshot? _rowsFor;
    private Dictionary<RecordKind, List<Row>> _rows = [];

    protected override void OnParametersSet()
    {
        // Sorted once per snapshot rather than per render: a large dataset has tens of thousands of rows.
        if (ReferenceEquals(_rowsFor, Snapshot))
        {
            return;
        }

        _rowsFor = Snapshot;
        _rows = new Dictionary<RecordKind, List<Row>>
        {
            [RecordKind.Category] = Sorted(Snapshot.Categories.Select(record => new Row(new RecordKey(RecordKind.Category, record.Id), record.Name))),
            [RecordKind.Component] = Sorted(Snapshot.Components.Select(record => new Row(new RecordKey(RecordKind.Component, record.Id), record.Name))),
            [RecordKind.Blueprint] = Sorted(Snapshot.Blueprints.Select(record => new Row(new RecordKey(RecordKind.Blueprint, record.Id), record.Name))),
            [RecordKind.Favorite] = Sorted(Snapshot.Favorites.Select(record => new Row(new RecordKey(RecordKind.Favorite, record.Id), record.Name)))
        };
    }

    private static List<Row> Sorted(IEnumerable<Row> rows) => [.. rows.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase)];

    private string RowClass(Row row) => Selected.Contains(row.Key) ? "transfer-row transfer-row-selected" : "transfer-row";

    private async Task ToggleRowAsync(Row row)
    {
        string subject = $"'{row.Name}'";

        if (Selected.Contains(row.Key))
        {
            await DeselectAsync(TransferSelectionProcessor.Deselect(Graph, Selected, row.Key), subject);
            return;
        }

        await ApplyAsync(TransferSelectionProcessor.Select(Graph, Selected, row.Key));

        if (row.Key.Kind == RecordKind.Category)
        {
            await OfferUsersAsync([row.Key], subject);
        }
    }

    private async Task ToggleAllAsync(TransferRecordKinds.Display panel, SelectionState state)
    {
        string subject = $"your {panel.Title.ToLowerInvariant()}";

        if (state == SelectionState.All)
        {
            await DeselectAsync(TransferSelectionProcessor.DeselectAll(Graph, Selected, panel.Kind), subject);
            return;
        }

        await ApplyAsync(TransferSelectionProcessor.SelectAll(Graph, Selected, panel.Kind));

        if (panel.Kind == RecordKind.Category)
        {
            await OfferUsersAsync([.. _rows[RecordKind.Category].Select(row => row.Key)], subject);
        }
    }

    private async Task DeselectAsync(SelectionChange change, string subject)
    {
        if (change.CascadedByKind.Count > 0
            && !await TransferPrompts.ConfirmDeselectAsync(DialogService, subject, change.CascadedByKind))
        {
            return;
        }

        await ApplyAsync(change);
    }

    /// <summary>
    /// The category shortcut: a category depends on nothing, so selecting one selects only it, and this offers
    /// what is filed under it as well.
    /// </summary>
    private async Task OfferUsersAsync(List<RecordKey> categories, string subject)
    {
        SelectionChange users = TransferSelectionProcessor.SelectUsersOf(Graph, Selected, categories);

        if (users.Added.Count > 0
            && await TransferPrompts.ConfirmSelectUsersAsync(DialogService, subject, users.CascadedByKind))
        {
            await ApplyAsync(users);
        }
    }

    private Task ApplyAsync(SelectionChange change)
    {
        Selected.UnionWith(change.Added);
        Selected.ExceptWith(change.Removed);

        return SelectionChanged.InvokeAsync();
    }

    private Task ShowInfoAsync(Row row) => row.Key.Kind switch
    {
        RecordKind.Category => InfoDialog.ShowAsync(DialogService, SnapshotModelProcessor.ToCategoryModel(Snapshot, row.Key.Id)),
        RecordKind.Component => InfoDialog.ShowAsync(DialogService, SnapshotModelProcessor.ToComponentModel(Snapshot, row.Key.Id)),
        RecordKind.Blueprint => InfoDialog.ShowAsync(DialogService, SnapshotModelProcessor.ToBlueprintModel(Snapshot, row.Key.Id)),
        _ => InfoDialog.ShowAsync(DialogService, new BlueprintFavorite { Id = row.Key.Id, Name = row.Name },
            SnapshotModelProcessor.ToFavoriteBlueprints(Snapshot, row.Key.Id))
    };
}
