namespace CraftingCalculator.Domain.Models.Transfer;

/// <summary>An export file on the device.</summary>
/// <param name="FullPath">Where the file is.</param>
/// <param name="FileName">The file's name, without its folder.</param>
/// <param name="DatasetName">The name of the dataset the file was exported from.</param>
/// <param name="CreatedAt">When the export was made.</param>
public sealed record ExportFileInfo(string FullPath, string FileName, string DatasetName, DateTimeOffset CreatedAt);
