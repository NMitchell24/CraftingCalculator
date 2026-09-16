namespace CraftingCalculator.UI.Components.Controls;

/// <summary>SVG markup, for MudBlazor's 24x24 icon viewBox, of the icons that step a quantity by one or by ten.</summary>
// Hand-rolled: Material's numbered icons stop at LooksTwo, and _10k draws "10K". Each is the sign and digits of
// Jersey 20 (wwwroot/Fonts/Jersey20.ttf, the app's display face), laid out with the font's own advances, scaled so
// one font pixel is 0.6 units (the digits are 12 units tall) and centered in the box. docs/help/assets/minus-10.svg,
// minus-1.svg, plus-1.svg and plus-10.svg wrap this same markup for the help pages.
public static class StepIcons
{
    private const string Background = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/>";

    /// <summary>"-10": a stepper button that takes ten off.</summary>
    public const string MinusTen =
        Background + "<path d=\"M0.9 10.8H8.1V13.2H0.9ZM9.9 6H14.1V18H11.7V8.4H9.9ZM15.9 7.8H16.5V6.6H17.7V6H21.3V6.6H22.5V7.8H23.1V16.2H22.5V17.4H21.3V18H17.7V17.4H16.5V16.2H15.9ZM18.3 15H18.9V15.6H20.1V15H20.7V9H20.1V8.4H18.9V9H18.3Z\"/>";

    /// <summary>"-1": a stepper button that takes one off.</summary>
    public const string MinusOne =
        Background + "<path d=\"M5.4 10.8H12.6V13.2H5.4ZM14.4 6H18.6V18H16.2V8.4H14.4Z\"/>";

    /// <summary>"+1": a stepper button that adds one.</summary>
    public const string PlusOne =
        Background + "<path d=\"M5.1 11.4H8.1V8.4H9.9V11.4H12.9V13.2H9.9V16.2H8.1V13.2H5.1ZM14.7 6H18.9V18H16.5V8.4H14.7Z\"/>";

    /// <summary>"+10": a stepper button that adds ten.</summary>
    public const string PlusTen =
        Background + "<path d=\"M0.6 11.4H3.6V8.4H5.4V11.4H8.4V13.2H5.4V16.2H3.6V13.2H0.6ZM10.2 6H14.4V18H12V8.4H10.2ZM16.2 7.8H16.8V6.6H18V6H21.6V6.6H22.8V7.8H23.4V16.2H22.8V17.4H21.6V18H18V17.4H16.8V16.2H16.2ZM18.6 15H19.2V15.6H20.4V15H21V9H20.4V8.4H19.2V9H18.6Z\"/>";
}
