using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace CraftingCalculator.UI.Components.Controls;

/// <summary>
/// Renders an in-memory list through <see cref="Virtualize{TItem}"/>: only the rows in and near the viewport are in
/// the DOM. The list is read again every time the parent renders, so a new or changed <see cref="Items"/> shows up
/// without any call from the parent.
/// </summary>
/// <typeparam name="TItem">The type of the rows.</typeparam>
public partial class VirtualList<TItem> : ComponentBase
{
    /// <summary>The rows, in display order.</summary>
    [Parameter, EditorRequired]
    public IReadOnlyList<TItem> Items { get; set; } = [];

    /// <summary>The expected height of a row in CSS px, including its margin. Measured rows replace it.</summary>
    [Parameter, EditorRequired]
    public float ItemSize { get; set; }

    /// <summary>How many rows to render beyond each edge of the viewport.</summary>
    [Parameter]
    public int OverscanCount { get; set; } = 3;

    /// <summary>The markup for one row.</summary>
    [Parameter, EditorRequired]
    public RenderFragment<TItem> ChildContent { get; set; } = null!;

    private Virtualize<TItem>? _virtualize;

    protected override Task OnParametersSetAsync() => _virtualize?.RefreshDataAsync() ?? Task.CompletedTask;

    // Virtualize.Items is not used because of its first render: every item is then in the after spacer, so the
    // first IntersectionObserver callback reports both spacers visible, and OnAfterSpacerVisible forces the window
    // one row down, which drops the first row for a frame or two until the before spacer brings it back. Its JS
    // skips that callback when the after spacer is 0 px, so the list reports no items until the first measurement
    // has set a capacity: before it, the request is always for 0 items, however often the parent re-renders.
    // The provider is synchronous, so no placeholder ever renders.
    private ValueTask<ItemsProviderResult<TItem>> ProvideItems(ItemsProviderRequest request) =>
        ValueTask.FromResult(request.Count == 0
            ? new ItemsProviderResult<TItem>([], 0)
            : new ItemsProviderResult<TItem>(Items.Skip(request.StartIndex).Take(request.Count), Items.Count));
}
