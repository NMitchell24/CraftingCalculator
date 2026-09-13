using CraftingCalculator.Application.BusinessLogic.Transfer;
using CraftingCalculator.Application.Common.Interfaces;

namespace CraftingCalculator.UI.Platform;

public class ImportFilePicker : IImportFilePicker
{
    private static readonly FilePickerFileType ExportFiles = new(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        [DevicePlatform.iOS] = [TransferFormat.UniformTypeIdentifier],
        [DevicePlatform.MacCatalyst] = [TransferFormat.FileExtension.TrimStart('.')],
        [DevicePlatform.WinUI] = [TransferFormat.FileExtension],

        // Android picks by MIME type, and a custom extension has none, so every file is offered and the reader
        // turns away anything that isn't an export.
        [DevicePlatform.Android] = ["*/*"]
    });

    public async Task<string?> PickAsync()
    {
        FileResult? picked = await MainThread.InvokeOnMainThreadAsync(() =>
            FilePicker.Default.PickAsync(new PickOptions { PickerTitle = "Choose a file to import", FileTypes = ExportFiles }));

        if (picked is null)
        {
            return null;
        }

        // Copied straight away: on iOS the picked file is a security-scoped URL and on Android a content:// URI, and
        // neither stays readable for as long as the import wizard can sit waiting on the user.
        string folder = Path.Combine(FileSystem.CacheDirectory, "import");
        Directory.CreateDirectory(folder);
        string path = Path.Combine(folder, "current" + TransferFormat.FileExtension);

        await using Stream source = await picked.OpenReadAsync();
        await using FileStream copy = File.Create(path);

        // One byte past the reader's limit is all it takes to reject the file, so a huge file picked by mistake never
        // fills the cache.
        byte[] buffer = new byte[81_920];
        long remaining = TransferDocumentReader.MaxFileBytes + 1;
        int read;

        while (remaining > 0 && (read = await source.ReadAsync(buffer.AsMemory(0, (int)Math.Min(buffer.Length, remaining)))) > 0)
        {
            await copy.WriteAsync(buffer.AsMemory(0, read));
            remaining -= read;
        }

        return path;
    }
}
