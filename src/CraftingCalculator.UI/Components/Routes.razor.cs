using CraftingCalculator.UI.Components.Controls;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components;

/// <summary>The app's root component: the router, wrapped in the boundary of last resort.</summary>
public partial class Routes : ComponentBase
{
    // Handed to FatalError, whose "Try again" recovers it. Assigned by @ref on the first render, which is
    // long before any ErrorContent of it can render.
    private LoggingErrorBoundary _appBoundary = null!;
}
