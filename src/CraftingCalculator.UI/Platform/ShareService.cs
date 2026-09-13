using CraftingCalculator.Application.Common.Interfaces;

namespace CraftingCalculator.UI.Platform;

public class ShareService : IShareService
{
    public Task ShareFileAsync(string path, string title) =>
        Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = title,
            File = new ShareFile(ShareablePath(path)),

            // iPadOS presents the share sheet as a popover, which needs a point to hang from. The action that
            // opened it is WebView markup with no native view to measure, so the popover anchors near the top
            // edge instead, the placement the MAUI docs give for this case.
            PresentationSourceBounds = DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Idiom == DeviceIdiom.Tablet
                ? new Rect(0, 20, 0, 0)
                : Rect.Zero
        });

#if ANDROID
    private static string ShareablePath(string path)
    {
        // MAUI hands a shared file to other apps through a FileProvider, and a provider left at its defaults
        // can expose the app's whole cache and data directory. Resources/xml/
        // microsoft_maui_essentials_fileprovider_file_paths.xml narrows it to cache/sharing-root, so the export
        // is copied in there first rather than shared from where it is saved.
        string root = Path.Combine(FileSystem.CacheDirectory, "sharing-root");
        Directory.CreateDirectory(root);

        string copy = Path.Combine(root, Path.GetFileName(path));
        File.Copy(path, copy, overwrite: true);

        return copy;
    }
#else
    private static string ShareablePath(string path) => path;
#endif
}
