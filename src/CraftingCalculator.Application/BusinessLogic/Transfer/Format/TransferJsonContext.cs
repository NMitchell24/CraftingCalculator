using System.Text.Json.Serialization;

namespace CraftingCalculator.Application.BusinessLogic.Transfer.Format;

/// <summary>
/// Reads and writes export files. Source-generated so the Android and iOS Release builds, which trim, never
/// depend on reflection to find the document's members. Reading is strict: a member the document does not
/// declare, a missing constructor value, or a null where the document does not allow one is an error.
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    WriteIndented = true,
    UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    RespectNullableAnnotations = true,
    RespectRequiredConstructorParameters = true)]
[JsonSerializable(typeof(TransferDocument))]
public partial class TransferJsonContext : JsonSerializerContext;
