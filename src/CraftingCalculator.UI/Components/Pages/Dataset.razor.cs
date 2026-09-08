using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Pages;

/// <summary>
/// The Dataset landing page: one card per record type showing how many of it exist, each opening that
/// type's list. The lists themselves live in <see cref="DatasetList" />.
/// </summary>
public partial class Dataset : ComponentBase, IDisposable
{
    /// <summary>One row of the landing page - a record type, its heading, icon, and current count.</summary>
    private sealed record DatasetSection(DataType Type, string Title, string Icon, string Caption);

    [Inject] private IDatasetService DatasetService { get; set; } = null!;
    [Inject] private IDatabaseAdminService DatabaseAdminService { get; set; } = null!;
    [Inject] private CraftState State { get; set; } = null!;
    [Inject] private PageShellState PageShellState { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private List<DatasetSection> Sections { get; set; } = [];
    private bool _busy;

    protected override async Task OnInitializedAsync()
    {
        PageShellState.Configure(this, new PageShellConfig("Dataset")
        {
            // Built from SectionSpecs so an action and the card it opens always carry the same icon -
            // that pairing is what tells the user which list each action leads to.
            Actions =
            [
                .. SectionSpecs.Select(spec =>
                    new PageAction(spec.Title, spec.Icon, () => OpenList(spec.Type))),
                new PageAction("Delete all data", Icons.Material.Filled.DeleteForever, DeleteAllDataAsync)
            ]
        });

        await ReloadAsync();
    }

    // Ordered the way the records have to be created: a blueprint needs components, and a component is
    // filed under a category, so the landing page reads top to bottom as the path a new user takes.
    // Both noun forms are spelled out rather than derived from the heading: "Categories" does not
    // singularise by trimming an s, and it does not pluralise by adding one either.
    private static readonly (DataType Type, string Title, string Icon, string Singular, string Plural)[] SectionSpecs =
    [
        (DataType.Category, "Categories", Icons.Material.Filled.Label, "category", "categories"),
        (DataType.Component, "Components", Icons.Material.Filled.Inventory2, "component", "components"),
        (DataType.Blueprint, "Blueprints", Icons.Material.Filled.Handyman, "blueprint", "blueprints")
    ];

    // Counting means loading each type in full, since IDatasetService exposes no count. That is the same
    // work the list pages already do and the data is local SQLite, so it is not worth a service method
    // until one of these lists is large enough to notice.
    private async Task ReloadAsync()
    {
        List<DatasetSection> sections = [];

        foreach ((DataType type, string title, string icon, string singular, string plural) in SectionSpecs)
        {
            int count = (await DatasetService.GetRecordsAsync(type)).Count;
            sections.Add(new DatasetSection(type, title, icon, $"{count} {(count == 1 ? singular : plural)}"));
        }

        Sections = sections;
    }

    // Returns a Task only because PageAction.OnClick is a Func<Task> - the navigation is synchronous,
    // and the card taps in the markup bind the same method.
    private Task OpenList(DataType type)
    {
        Navigation.NavigateTo($"/dataset/{type}");
        return Task.CompletedTask;
    }

    private async Task DeleteAllDataAsync()
    {
        DialogParameters parameters = new()
        {
            ["Message"] = "This removes every favorite, blueprint, category, and component from the app. "
                          + "It cannot be undone.",
            ["ConfirmWord"] = "DELETE",
            ["ConfirmText"] = "Delete everything"
        };

        IDialogReference dialogRef = await DialogService.ShowAsync<TypedConfirmDialog>("Delete Everything?", parameters);
        DialogResult? result = await dialogRef.Result;

        if (result is null or { Canceled: true })
        {
            return;
        }

        _busy = true;
        StateHasChanged();

        try
        {
            await DatabaseAdminService.DeleteAllDataAsync();

            // The batch on the Craft screen holds Blueprint models that no longer exist in the
            // database - left alone it would keep pricing out deleted blueprints.
            State.Clear();

            await ReloadAsync();
        }
        finally
        {
            _busy = false;
        }

        Snackbar.Add("Deleted all data", Severity.Success);
    }

    public void Dispose() => PageShellState.Reset(this);
}
