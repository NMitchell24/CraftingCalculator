namespace CraftingCalculator.UI;

// Sibling namespace CraftingCalculator.Application means the bare identifier "Application" here
// resolves to that namespace, not Microsoft.Maui.Controls.Application - same gotcha as
// CraftingCalculator (the WPF project)'s App.xaml.cs. Keep the base class and any reference to
// Microsoft.Maui.Controls.Application fully qualified.
public partial class App : Microsoft.Maui.Controls.Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState) => new(new MainPage());
}
