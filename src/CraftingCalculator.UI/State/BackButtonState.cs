namespace CraftingCalculator.UI.State;

/// <summary>
/// What the platform's back action does in place of stepping back through history. A singleton, unlike the rest of
/// UI/State, so platform code outside the BlazorWebView's service scope reads the same instance the app writes.
/// </summary>
public sealed class BackButtonState
{
    /// <summary>Raised whenever <see cref="Handler" /> changes.</summary>
    public event Action? Changed;

    /// <summary>Runs in place of the back action; null when back steps back through history as usual.</summary>
    public Action? Handler { get; private set; }

    public void SetHandler(Action? handler)
    {
        Handler = handler;
        Changed?.Invoke();
    }
}
