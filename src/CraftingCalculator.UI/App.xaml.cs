using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace CraftingCalculator.UI;

// Sibling namespace CraftingCalculator.Application means the bare identifier "Application" here
// resolves to that namespace, not Microsoft.Maui.Controls.Application. Keep the base class and any
// reference to Microsoft.Maui.Controls.Application fully qualified.
public partial class App : Microsoft.Maui.Controls.Application
{
    private readonly IServiceProvider _services;

    // UseMauiApp<App> resolves this type from the container. The provider itself rather than
    // StartupErrorPage's own dependencies: the page is built only when the database failed, and this class
    // has no other reason to know what it takes.
    public App(IServiceProvider services)
    {
        _services = services;

        InitializeComponent();

        // MAUI applies its own soft-input mode to the activity at runtime, and its default - Pan -
        // wins over MainActivity's WindowSoftInputMode, so this is the setting that actually takes
        // effect. Pan slides the whole window up when the keyboard opens, which carries the fixed app
        // bar off the top of the screen; Resize shrinks the window instead and leaves it in place.
        // Same namespace collision as the class declaration below: bare "Android" resolves to the
        // Android bindings namespace on the android head, not to the platform-configuration marker.
        this.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>()
            .UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
    }

    protected override Window CreateWindow(IActivationState? activationState) => new(
        MauiProgram.DatabaseReady
            ? new MainPage()
            : _services.GetRequiredService<StartupErrorPage>());
}
