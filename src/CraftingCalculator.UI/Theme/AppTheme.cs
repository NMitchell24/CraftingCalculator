using MudBlazor;

namespace CraftingCalculator.UI.Theme;

/// <summary>
/// The app's single <see cref="MudTheme" /> - the "Forge" palette (slate neutrals, copper primary) in
/// light and dark, the Inter type scale, and the shared corner radius. Applied by <c>MainLayout</c>'s
/// <c>MudThemeProvider</c>; which of the two palettes renders follows the user's
/// <see cref="State.ThemeState" /> choice.
/// </summary>
public static class AppTheme
{
    // Single source of truth for every Forge colour. Names are by meaning, not palette slot: a value
    // that fills several slots (LightMuted, LightLines, ...) is one constant, with the slots it serves
    // noted inline.
    private static class ForgeColors
    {
        public const string White = "#FFFFFF"; // PrimaryContrastText + SecondaryContrastText + AppbarText, light only

        // Light - white surfaces on a cool grey ground, copper primary, slate app bar
        public const string LightPrimary = "#B7601F";
        public const string LightSlate = "#2E3440"; // Secondary + AppbarBackground
        public const string LightSurface = "#FFFFFF"; // Surface + DrawerBackground
        public const string LightBackground = "#F5F6F8";
        public const string LightBackgroundGray = "#EDEFF3"; // also the segmented control's track (app.css)
        public const string LightInk = "#1C2027"; // TextPrimary + DrawerText
        public const string LightMuted = "#5A6270"; // TextSecondary + ActionDefault + DrawerIcon
        public const string LightLines = "#E1E4EA"; // Divider + LinesDefault + TableLines
        public const string LightSuccess = "#2E7D32";

        // Dark - near-black ground, lifted slate surfaces, warmed copper primary
        public const string DarkPrimary = "#E39A5C";
        public const string DarkPrimaryText = "#1B1E24";
        public const string DarkSlate = "#2C313B"; // Secondary + Divider + LinesDefault + TableLines
        public const string DarkSurface = "#1B1E24"; // Surface + AppbarBackground
        public const string DarkDrawer = "#171A20";
        public const string DarkBackground = "#12141A";
        public const string DarkBackgroundGray = "#0E1016"; // also the segmented control's track (app.css)
        public const string DarkInk = "#E4E7EC"; // SecondaryContrastText + AppbarText + DrawerText + TextPrimary
        public const string DarkMuted = "#98A1B0"; // TextSecondary + ActionDefault + DrawerIcon
        public const string DarkSuccess = "#66BB6A";
    }

    // Inter is bundled locally (wwwroot/Fonts) and registered via @font-face in app.css. The display
    // face (Jersey 20) is deliberately absent here: MudBlazor's Typography has no display variant, so
    // adding it would mean claiming a text variant like H6 and dragging it onto every Typo.h6 in the
    // app. It is applied through app.css's .display-title class instead.
    private static readonly string[] BodyFont =
        ["Inter", "Helvetica Neue", "Helvetica", "Arial", "sans-serif"];

    // MudBlazor Typography's default letter spacing; repeated across most variants below.
    private const string NormalSpacing = "normal";

    public static MudTheme Theme { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = ForgeColors.LightPrimary,
            PrimaryContrastText = ForgeColors.White,
            Secondary = ForgeColors.LightSlate,
            SecondaryContrastText = ForgeColors.White,
            Background = ForgeColors.LightBackground,
            BackgroundGray = ForgeColors.LightBackgroundGray,
            Surface = ForgeColors.LightSurface,
            AppbarBackground = ForgeColors.LightSlate,
            AppbarText = ForgeColors.White,
            DrawerBackground = ForgeColors.LightSurface,
            DrawerText = ForgeColors.LightInk,
            DrawerIcon = ForgeColors.LightMuted,
            TextPrimary = ForgeColors.LightInk,
            TextSecondary = ForgeColors.LightMuted,
            ActionDefault = ForgeColors.LightMuted,
            Divider = ForgeColors.LightLines,
            LinesDefault = ForgeColors.LightLines,
            TableLines = ForgeColors.LightLines,
            Success = ForgeColors.LightSuccess
        },
        PaletteDark = new PaletteDark
        {
            Primary = ForgeColors.DarkPrimary,
            PrimaryContrastText = ForgeColors.DarkPrimaryText,
            Secondary = ForgeColors.DarkSlate,
            SecondaryContrastText = ForgeColors.DarkInk,
            Background = ForgeColors.DarkBackground,
            BackgroundGray = ForgeColors.DarkBackgroundGray,
            Surface = ForgeColors.DarkSurface,
            AppbarBackground = ForgeColors.DarkSurface,
            AppbarText = ForgeColors.DarkInk,
            DrawerBackground = ForgeColors.DarkDrawer,
            DrawerText = ForgeColors.DarkInk,
            DrawerIcon = ForgeColors.DarkMuted,
            TextPrimary = ForgeColors.DarkInk,
            TextSecondary = ForgeColors.DarkMuted,
            ActionDefault = ForgeColors.DarkMuted,
            Divider = ForgeColors.DarkSlate,
            LinesDefault = ForgeColors.DarkSlate,
            TableLines = ForgeColors.DarkSlate,
            Success = ForgeColors.DarkSuccess
        },
        Typography = BuildTypography(),
        // AppbarHeight is deliberately left at MudBlazor's 64px default. The app bar is Dense, and
        // MudBlazor derives the dense bar as AppbarHeight - AppbarHeight / 4, so 64px is what produces
        // the 48px bar on screen. Setting it to 48px (as it is tempting to do, to match what is
        // rendered) would give a 36px bar - under the 44px touch-target floor - and would shift the
        // two --mud-appbar-height calculations app.css uses to align the main content and side rail.
        LayoutProperties = new LayoutProperties { DefaultBorderRadius = "12px" }
    };

    // FontSize uses clamp(minPx, Xvw, maxPx) - px + vw, never rem. Android/iOS "Font size"
    // accessibility inflates the root em, so any rem-based bound balloons with it; px is immune. The
    // vw term keeps text a stable fraction of the screen and lets "Display size" enlarge it modestly;
    // maxPx holds the look on wide screens, minPx floors legibility.
    private static Typography BuildTypography()
    {
        return new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = BodyFont, FontWeight = "400", FontSize = "clamp(12px, 3.7vw, 15px)",
                LineHeight = "1.5", LetterSpacing = NormalSpacing
            },
            H1 = new H1Typography
            {
                FontFamily = BodyFont, FontWeight = "800", FontSize = "clamp(36px, 11.7vw, 48px)",
                LineHeight = "1.1", LetterSpacing = "-.02em"
            },
            H2 = new H2Typography
            {
                FontFamily = BodyFont, FontWeight = "800", FontSize = "clamp(30px, 9.3vw, 38px)",
                LineHeight = "1.15", LetterSpacing = "-.02em"
            },
            H3 = new H3Typography
            {
                FontFamily = BodyFont, FontWeight = "700", FontSize = "clamp(25px, 7.8vw, 32px)",
                LineHeight = "1.2", LetterSpacing = "-.01em"
            },
            H4 = new H4Typography
            {
                FontFamily = BodyFont, FontWeight = "700", FontSize = "clamp(20px, 6.2vw, 26px)",
                LineHeight = "1.25", LetterSpacing = "-.01em"
            },
            H5 = new H5Typography
            {
                FontFamily = BodyFont, FontWeight = "600", FontSize = "clamp(16px, 5.1vw, 21px)",
                LineHeight = "1.3", LetterSpacing = NormalSpacing
            },
            H6 = new H6Typography
            {
                FontFamily = BodyFont, FontWeight = "600", FontSize = "clamp(15px, 4.5vw, 18px)",
                LineHeight = "1.35", LetterSpacing = NormalSpacing
            },
            Subtitle1 = new Subtitle1Typography
            {
                FontFamily = BodyFont, FontWeight = "600", FontSize = "clamp(13px, 4.1vw, 17px)",
                LineHeight = "1.5", LetterSpacing = NormalSpacing
            },
            Subtitle2 = new Subtitle2Typography
            {
                FontFamily = BodyFont, FontWeight = "600", FontSize = "clamp(12px, 3.5vw, 14px)",
                LineHeight = "1.5", LetterSpacing = NormalSpacing
            },
            Body1 = new Body1Typography
            {
                FontFamily = BodyFont, FontWeight = "400", FontSize = "clamp(13px, 3.9vw, 16px)",
                LineHeight = "1.55", LetterSpacing = NormalSpacing
            },
            Body2 = new Body2Typography
            {
                FontFamily = BodyFont, FontWeight = "400", FontSize = "clamp(12px, 3.5vw, 14px)",
                LineHeight = "1.5", LetterSpacing = NormalSpacing
            },
            Button = new ButtonTypography
            {
                FontFamily = BodyFont, FontWeight = "600", FontSize = "clamp(12px, 3.5vw, 14px)",
                LineHeight = "1.75", LetterSpacing = ".02em", TextTransform = "none"
            },
            Caption = new CaptionTypography
            {
                FontFamily = BodyFont, FontWeight = "400", FontSize = "clamp(10px, 3vw, 12.5px)",
                LineHeight = "1.5", LetterSpacing = NormalSpacing
            },
            Overline = new OverlineTypography
            {
                FontFamily = BodyFont, FontWeight = "600", FontSize = "clamp(9px, 2.8vw, 11.5px)",
                LineHeight = "2.0", LetterSpacing = ".08em"
            }
        };
    }
}
