#if ANDROID
using Android.Content;
using Android.Provider;
using AndroidEnvironment = Android.OS.Environment;
using AndroidUri = Android.Net.Uri;
#endif
using CraftingCalculator.Application.Common.Interfaces;

namespace CraftingCalculator.UI.Platform;

public class ExportDownloader : IExportDownloader
{
#if ANDROID
    // .ccdata is the app's own extension, so no registered media type describes it. MediaStore requires one,
    // and this is what Android itself falls back to for a file it cannot identify.
    private const string MimeType = "application/octet-stream";

    // Android is the only platform that hides FileSystem.AppDataDirectory from the user, so it is the only one
    // where an export has to be copied anywhere for them to reach it.
    public bool IsSupported => true;

    public async Task SaveToDownloadsAsync(string path)
    {
        ContentResolver resolver = Android.App.Application.Context.ContentResolver!;

        ContentValues values = new();
        values.Put(MediaStore.IMediaColumns.DisplayName, Path.GetFileName(path));
        values.Put(MediaStore.IMediaColumns.MimeType, MimeType);
        values.Put(MediaStore.IMediaColumns.RelativePath, AndroidEnvironment.DirectoryDownloads);

        // A pending row is hidden from every other app, so a file manager can never open a half-written export.
        // Cleared once the copy is complete.
        values.Put(MediaStore.IMediaColumns.IsPending, 1);

        // MediaStore settles the file name itself, appending " (1)" and so on where the folder already holds
        // that name, which is why the destination is inserted rather than composed here.
        AndroidUri target = resolver.Insert(MediaStore.Downloads.ExternalContentUri, values)
                            ?? throw new IOException("The downloads folder refused a new file.");

        try
        {
            // Scoped rather than declared: the streams have to be flushed and closed before the row is
            // published, and a declaration would dispose them at the end of this try instead.
            await using (Stream source = File.OpenRead(path))
            await using (Stream destination = resolver.OpenOutputStream(target)
                                              ?? throw new IOException("The new file in the downloads folder couldn't be opened."))
            {
                await source.CopyToAsync(destination);
            }

            values.Clear();
            values.Put(MediaStore.IMediaColumns.IsPending, 0);

            // Update reports how many rows it changed, and none means the row is no longer there to clear the
            // pending flag on. The file would stay invisible, so this counts as a failure rather than a save.
            if (resolver.Update(target, values, null, null) == 0)
            {
                throw new IOException("The downloads folder didn't publish the new file.");
            }
        }
        catch
        {
            // Anything that failed before the flag was cleared leaves the row pending, stranding a file the
            // user can neither see nor delete.
            resolver.Delete(target, null, null);
            throw;
        }
    }
#else
    public bool IsSupported => false;

    public Task SaveToDownloadsAsync(string path) => throw new NotSupportedException();
#endif
}
