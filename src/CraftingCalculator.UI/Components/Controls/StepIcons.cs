namespace CraftingCalculator.UI.Components.Controls;

/// <summary>SVG markup, for MudBlazor's 24x24 icon viewBox, of the icons that step a batch quantity by ten.</summary>
// Hand-rolled: Material's numbered icons stop at LooksTwo, and _10k draws "10K". Each is a sign drawn in the left
// third followed by the digits 1 and 0. docs/help/assets/x10.svg, minus-10.svg and plus-10.svg wrap this same
// markup for the help pages.
public static class StepIcons
{
    private const string Ten =
        "<path d=\"M14 6h-1.4L10 8.4v1.7l2.6-1.9V18h1.4z\"/>" +
        "<path d=\"M19 6a3.2 6 0 1 0 0 12 3.2 6 0 1 0 0-12zm0 2.4a1.6 3.6 0 1 1 0 7.2 1.6 3.6 0 1 1 0-7.2z\"/>";

    private const string Background = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/>";

    /// <summary>"x10": the Craft screen's Step by 10 action.</summary>
    public const string TimesTen =
        Background +
        "<path d=\"M8.5 9.21L7.8 8.5 5 11.3 2.21 8.5 1.5 9.21 4.3 12 1.5 14.8 2.21 15.5 5 12.71 7.8 15.5 8.5 14.8 5.71 12z\"/>" +
        Ten;

    /// <summary>"-10": a stepper button that takes ten off.</summary>
    public const string MinusTen =
        Background +
        "<path d=\"M8.5 12.7h-7v-1.4h7z\"/>" +
        Ten;

    /// <summary>"+10": a stepper button that adds ten.</summary>
    public const string PlusTen =
        Background +
        "<path d=\"M8.5 12.7H5.7v2.8H4.3v-2.8H1.5v-1.4h2.8V8.5h1.4v2.8h2.8z\"/>" +
        Ten;
}
