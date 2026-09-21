using CraftingCalculator.UI.Components.Controls;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CraftingCalculator.UI.Components.Layout;

/// <summary>
/// What the app shows when an exception escapes every other boundary: what happened, that the user's data is
/// intact, and the two ways back in. Rendered as the app boundary's error content (<c>Routes.razor</c>).
/// </summary>
public partial class FatalError : ComponentBase
{
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] private IJSRuntime Js { get; set; } = null!;

    /// <summary>The boundary showing this content, which "Try again" recovers.</summary>
    [Parameter, EditorRequired] public LoggingErrorBoundary Boundary { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        try
        {
            // A failure early enough in MainLayout leaves the startup cloak (index.html) on screen, and it
            // covers everything - including this.
            await Js.InvokeVoidAsync("appCloak.dismiss");
        }
        catch (JSException)
        {
            // Load-bearing: js/cloak.js is the only thing that defines appCloak, and an exception thrown out
            // of an ErrorContent is the one the boundary above cannot catch - it is faulted already, so the
            // throw would reach the renderer and take the screen reporting the failure with it.
        }
    }

    private void TryAgain() => Boundary.Recover();

    // A full reload rather than a route change: everything scoped to the Blazor app - every state class, the
    // renderer itself - is rebuilt, which is the difference between this and Try again.
    private void Restart() => Navigation.NavigateTo(Navigation.BaseUri, forceLoad: true);
}
