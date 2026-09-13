using System.Buffers;
using System.Text.Json;
using CraftingCalculator.Application.BusinessLogic.Transfer;
using CraftingCalculator.Application.BusinessLogic.Transfer.Format;
using CraftingCalculator.Application.BusinessLogic.Transfer.Format.V1;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.Infrastructure.Files;

/// <summary>Export files in one folder on the device, named <c>{dataset}-{yyyyMMdd-HHmmss}.ccdata</c>.</summary>
/// <param name="directory">The folder the exports are saved to. Created on the first save.</param>
public class ExportFileStore(string directory) : IExportFileStore
{
    private const int KeptExports = 5;

    private const string TempExtension = ".tmp";

    // Windows' reserved characters, stripped on every platform: a name has to stay valid wherever the file
    // is shared to, and the tests have to see the same name on the Linux CI runner as on a PC.
    private static readonly SearchValues<char> ReservedFileNameChars = SearchValues.Create("\\/:*?\"<>|");

    private const int MaxDatasetNameLength = 100;

    // The header fields come before any record list, so the start of the file holds them whatever the size
    // of the dataset. Generous against a long dataset name.
    private const int HeaderBytes = 64 * 1024;

    public async Task<ExportFileInfo> SaveAsync(TransferDocumentV1 document)
    {
        Directory.CreateDirectory(directory);

        // Only one export runs at a time, so a temp file found here was left by one the app was killed
        // partway through.
        foreach (FileInfo stale in new DirectoryInfo(directory).EnumerateFiles($"*{TempExtension}"))
        {
            stale.Delete();
        }

        string path = UniquePath($"{SafeDatasetName(document.DatasetName)}-{document.ExportedAt.ToLocalTime():yyyyMMdd-HHmmss}");
        string tempPath = path + TempExtension;

        // Written under a temp name and moved into place only once complete, so a half-written file can never
        // be picked up as the latest export.
        await using (FileStream stream = File.Create(tempPath))
        {
            await JsonSerializer.SerializeAsync(stream, document, TransferJsonContext.Default.TransferDocumentV1);
        }

        File.Move(tempPath, path);

        foreach (FileInfo old in ExportFiles().Skip(KeptExports))
        {
            old.Delete();
        }

        return new ExportFileInfo(path, Path.GetFileName(path), document.DatasetName, document.ExportedAt);
    }

    public ExportFileInfo? GetLatest() => ExportFiles().Select(TryReadHeader).FirstOrDefault(info => info is not null);

    /// <summary>The export files in the folder, newest first.</summary>
    private IEnumerable<FileInfo> ExportFiles()
    {
        DirectoryInfo folder = new(directory);

        if (!folder.Exists)
        {
            return [];
        }

        // Filtered on Extension as well as the pattern: Windows matches a search pattern against short file
        // names too, which can let a longer extension through.
        return folder.EnumerateFiles($"*{TransferFormat.FileExtension}")
            .Where(file => file.Extension.Equals(TransferFormat.FileExtension, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(file => file.LastWriteTimeUtc)
            .ThenByDescending(file => file.Name, StringComparer.Ordinal);
    }

    /// <summary>
    /// Reads the dataset name and export time from the top of <paramref name="file"/>, or returns null if it
    /// is not an export file.
    /// </summary>
    private static ExportFileInfo? TryReadHeader(FileInfo file)
    {
        try
        {
            byte[] head = new byte[HeaderBytes];
            int length;

            using (FileStream stream = file.OpenRead())
            {
                length = stream.ReadAtLeast(head, head.Length, throwOnEndOfStream: false);
            }

            Utf8JsonReader reader = new(head.AsSpan(0, length), isFinalBlock: length < head.Length, state: default);
            string? format = null;
            DateTimeOffset? exportedAt = null;
            string? datasetName = null;

            while (datasetName is null && reader.Read())
            {
                if (reader.TokenType != JsonTokenType.PropertyName || reader.CurrentDepth != 1)
                {
                    continue;
                }

                string property = reader.GetString()!;
                reader.Read();

                if (property == HeaderName(nameof(TransferDocumentV1.Format)))
                {
                    format = reader.GetString();
                }
                else if (property == HeaderName(nameof(TransferDocumentV1.ExportedAt)))
                {
                    exportedAt = reader.GetDateTimeOffset();
                }
                else if (property == HeaderName(nameof(TransferDocumentV1.DatasetName)))
                {
                    datasetName = reader.GetString();
                }
            }

            return format == TransferFormat.Name && exportedAt is { } createdAt && datasetName is not null
                ? new ExportFileInfo(file.FullName, file.Name, datasetName, createdAt)
                : null;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException
                                              or JsonException or InvalidOperationException or FormatException)
        {
            return null;
        }
    }

    private static string HeaderName(string member) => JsonNamingPolicy.CamelCase.ConvertName(member);

    private string UniquePath(string stem)
    {
        string path = Path.Combine(directory, stem + TransferFormat.FileExtension);
        int copy = 1;

        // Two exports finished within the same second.
        while (File.Exists(path))
        {
            copy++;
            path = Path.Combine(directory, $"{stem}-{copy}{TransferFormat.FileExtension}");
        }

        return path;
    }

    private static string SafeDatasetName(string name)
    {
        string kept = new([.. name.Where(character => !char.IsControl(character) && !ReservedFileNameChars.Contains(character))]);

        // Windows refuses a name ending in a dot or a space.
        string trimmed = kept.Length > MaxDatasetNameLength ? kept[..MaxDatasetNameLength] : kept;
        trimmed = trimmed.Trim().TrimEnd('.');

        return trimmed.Length > 0 ? trimmed : "Dataset";
    }
}
