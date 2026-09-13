using System.Text.Json;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.BusinessLogic.Transfer.Format;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.Application.BusinessLogic.Transfer;

/// <summary>
/// Reads an import file and checks everything about it that could stop it importing cleanly. Nothing a file holds
/// reaches the app until it has passed.
/// </summary>
public static class TransferDocumentReader
{
    /// <summary>The largest file, in bytes, that is read at all.</summary>
    // Ten times the largest export measured on a device: 6.2 MB for 10,000 components and 5,000 blueprints.
    public const long MaxFileBytes = 64 * 1024 * 1024;

    private const int MaxListedErrors = 50;

    // The editor puts no limit on a name or a description, so these only have to stop a string nobody typed. Both
    // are far past anything a person would.
    private const int MaxNameLength = 10_000;
    private const int MaxDescriptionLength = 1_000_000;

    // Ten times the largest dataset measured on a device.
    private const int MaxRecordsPerKind = 100_000;

    private const int MaxQuotedNameLength = 60;

    private const string NotAnExport = "This isn't a Crafting Calculator export file.";

    // An export nests five deep: document, list, record, link list, link. The margin allows for a file
    // someone reformatted by hand, and the cap stops one built to exhaust the parser.
    private static readonly JsonDocumentOptions ParseOptions = new()
    {
        MaxDepth = 16,
        CommentHandling = JsonCommentHandling.Disallow,
        AllowTrailingCommas = false
    };

    /// <summary>
    /// Reads the export file in <paramref name="stream"/>, which has to support seeking, and returns the records it
    /// holds, or every reason it can't be imported.
    /// </summary>
    public static ImportValidationResult Read(Stream stream)
    {
        // Checked before a byte is parsed, so a huge file costs nothing.
        if (stream.Length > MaxFileBytes)
        {
            return Invalid($"This file is bigger than {MaxFileBytes / (1024 * 1024)} MB, which is far bigger than any export.");
        }

        TransferDocument document;

        try
        {
            using JsonDocument json = JsonDocument.Parse(stream, ParseOptions);

            if (CheckFormat(json.RootElement) is { } formatError)
            {
                return Invalid(formatError);
            }

            // An older file deserializes through the current types: every member added since its version has a
            // default. Only a version whose change was not additive needs an upgrade step here, applied to the
            // parsed JSON one version at a time before deserialization (docs/transfer-format-maintenance.md).
            try
            {
                document = json.Deserialize(TransferJsonContext.Default.TransferDocument)
                           ?? throw new JsonException();
            }
            catch (JsonException exception)
            {
                string where = string.IsNullOrEmpty(exception.Path) ? "" : $" The problem is at {exception.Path}.";
                return Invalid($"Part of this file isn't laid out the way an export is.{where}");
            }
        }
        catch (JsonException)
        {
            // JsonDocument.Parse: not JSON at all, cut off partway, or nested past ParseOptions.MaxDepth.
            return Invalid("This file is damaged or isn't a Crafting Calculator export file.");
        }

        List<string> errors = new DocumentChecker(document).Run();

        if (errors.Count > 0)
        {
            return new ImportValidationResult(null, errors);
        }

        DatasetSnapshot snapshot = ToSnapshot(document);
        errors.AddRange(CheckNesting(snapshot));

        return errors.Count > 0
            ? new ImportValidationResult(null, errors)
            : new ImportValidationResult(new ImportFile(snapshot, document.ExportedAt, document.AppVersion), []);
    }

    private static ImportValidationResult Invalid(string error) => new(null, [error]);

    /// <summary>An error when the document is not an export this build of the app can read, or null.</summary>
    private static string? CheckFormat(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object
            || !root.TryGetProperty(JsonName(nameof(TransferDocument.Format)), out JsonElement format)
            || format.ValueKind != JsonValueKind.String
            || format.GetString() != TransferFormat.Name
            || !root.TryGetProperty(JsonName(nameof(TransferDocument.FormatVersion)), out JsonElement version)
            || version.ValueKind != JsonValueKind.Number
            || !version.TryGetInt32(out int number)
            || number < 1)
        {
            return NotAnExport;
        }

        return number > TransferFormat.CurrentVersion
            ? "This file was made by a newer version of Crafting Calculator. Update the app, then import it again."
            : null;
    }

    private static string JsonName(string member) => JsonNamingPolicy.CamelCase.ConvertName(member);

    /// <summary>The blueprints that nest themselves, or nest deeper than the app can work out.</summary>
    /// <remarks>Only run on a snapshot every link of which resolves.</remarks>
    private static IEnumerable<string> CheckNesting(DatasetSnapshot snapshot)
    {
        IReadOnlySet<RecordKey> cyclic = DependencyGraphProcessor.FindCycles(DependencyGraphProcessor.Build(snapshot));

        if (cyclic.Count > 0)
        {
            List<string> names =
            [
                .. snapshot.Blueprints
                    .Where(blueprint => cyclic.Contains(new RecordKey(RecordKind.Blueprint, blueprint.Id)))
                    .Select(blueprint => Quoted(blueprint.Name))
            ];

            yield return names.Count == 1
                ? $"The blueprint {names[0]} is nested inside itself."
                : $"The blueprints {string.Join(", ", names[..^1])} and {names[^1]} are nested inside each other.";

            // Depth has no answer on a loop.
            yield break;
        }

        if (snapshot.Blueprints.Count == 0)
        {
            yield break;
        }

        (SnapshotBlueprint deepest, int depth) = Deepest(snapshot);

        if (depth > BlueprintProcessor.MaxBlueprintDepth)
        {
            yield return $"The blueprint {Quoted(deepest.Name)} has blueprints nested {depth} levels deep inside it. "
                         + $"The most the app can work out is {BlueprintProcessor.MaxBlueprintDepth}.";
        }
    }

    /// <summary>
    /// The blueprint with the most levels of blueprints nested below it, and that number of levels, counted the way
    /// <see cref="BlueprintProcessor"/> counts depth. Only run on a snapshot with at least one blueprint and no loops.
    /// </summary>
    private static (SnapshotBlueprint Blueprint, int Depth) Deepest(DatasetSnapshot snapshot)
    {
        Dictionary<int, SnapshotBlueprint> byId = snapshot.Blueprints.ToDictionary(blueprint => blueprint.Id);
        Dictionary<int, int> depthOf = [];

        // Post-order with an explicit stack rather than recursion: a file can chain blueprints deeper than the call
        // stack allows. A blueprint's frame is pushed back under its children, so they all have a depth by the time
        // it is popped again.
        foreach (SnapshotBlueprint start in snapshot.Blueprints)
        {
            Stack<(int Id, bool ChildrenDone)> pending = new([(start.Id, false)]);

            while (pending.TryPop(out (int Id, bool ChildrenDone) frame))
            {
                if (depthOf.ContainsKey(frame.Id))
                {
                    continue;
                }

                IReadOnlyList<QuantityLink> children = byId[frame.Id].Blueprints;

                if (frame.ChildrenDone)
                {
                    depthOf[frame.Id] = children.Select(child => 1 + depthOf[child.TargetId]).DefaultIfEmpty(0).Max();
                    continue;
                }

                pending.Push((frame.Id, true));

                foreach (QuantityLink child in children.Where(child => !depthOf.ContainsKey(child.TargetId)))
                {
                    pending.Push((child.TargetId, false));
                }
            }
        }

        return snapshot.Blueprints
            .Select(blueprint => (Blueprint: blueprint, Depth: depthOf[blueprint.Id]))
            .MaxBy(pair => pair.Depth);
    }

    private static DatasetSnapshot ToSnapshot(TransferDocument document) => new(
        document.DatasetName,
        [.. document.Categories.Select(category => new SnapshotCategory(category.Ref, category.Name, category.Description))],
        [
            .. document.Components.Select(component => new SnapshotComponent(
                component.Ref, component.Name, component.Description, component.Cost, component.ProductionTime,
                component.Category))
        ],
        [
            .. document.Blueprints.Select(blueprint => new SnapshotBlueprint(
                blueprint.Ref, blueprint.Name, blueprint.Description, blueprint.Value, blueprint.Yield,
                blueprint.ProductionTime, blueprint.Category, Links(blueprint.Components), Links(blueprint.Blueprints)))
        ],
        [.. document.Favorites.Select(favorite => new SnapshotFavorite(favorite.Ref, favorite.Name, Links(favorite.Blueprints)))]);

    private static List<QuantityLink> Links(IEnumerable<QuantityRef> links) =>
        [.. links.Select(link => new QuantityLink(link.Ref, link.Quantity))];

    private static string Quoted(string name)
    {
        string trimmed = name.Trim();
        return $"'{(trimmed.Length > MaxQuotedNameLength ? trimmed[..MaxQuotedNameLength] + "…" : trimmed)}'";
    }

    /// <summary>
    /// The record-by-record checks on a deserialized document, which find every problem rather than stopping at the
    /// first, list the first <see cref="MaxListedErrors"/> and count the rest. One instance serves one
    /// <see cref="Run"/>.
    /// </summary>
    private sealed class DocumentChecker(TransferDocument document)
    {
        private const string Category = "category";
        private const string Component = "component";
        private const string Blueprint = "blueprint";
        private const string Favorite = "favorite";

        private readonly List<string> _errors = [];
        private int _unlisted;

        public List<string> Run()
        {
            // The serializer enforces the nullability of every member except the entries of a list.
            if (HasNullEntry())
            {
                Add("Part of this file is empty where a record should be.");
                return _errors;
            }

            CheckLength("The dataset name", document.DatasetName, MaxNameLength);
            CheckLength("The app version", document.AppVersion, MaxNameLength);

            HashSet<int> categories = CheckRefs(Category, "categories", document.Categories, category => category.Ref, category => category.Name);
            HashSet<int> components = CheckRefs(Component, "components", document.Components, component => component.Ref, component => component.Name);
            HashSet<int> blueprints = CheckRefs(Blueprint, "blueprints", document.Blueprints, blueprint => blueprint.Ref, blueprint => blueprint.Name);
            CheckRefs(Favorite, "favorites", document.Favorites, favorite => favorite.Ref, favorite => favorite.Name);

            foreach (TransferCategory category in document.Categories)
            {
                CheckName(Category, category.Ref, category.Name);
                CheckText(Named(Category, category.Name), category.Name, category.Description);
            }

            foreach (TransferComponent component in document.Components)
            {
                string subject = Named(Component, component.Name);

                CheckName(Component, component.Ref, component.Name);
                CheckText(subject, component.Name, component.Description);
                CheckProductionTime(subject, component.ProductionTime);
                CheckCategory(subject, component.Category, categories);
            }

            foreach (TransferBlueprint blueprint in document.Blueprints)
            {
                string subject = Named(Blueprint, blueprint.Name);

                CheckName(Blueprint, blueprint.Ref, blueprint.Name);
                CheckText(subject, blueprint.Name, blueprint.Description);
                CheckProductionTime(subject, blueprint.ProductionTime);
                CheckCategory(subject, blueprint.Category, categories);

                if (blueprint.Yield < 1)
                {
                    Add($"{subject} makes fewer than 1 item per craft.");
                }

                CheckLinks(subject, Component, blueprint.Components, components);
                CheckLinks(subject, Blueprint, blueprint.Blueprints, blueprints);
            }

            foreach (TransferFavorite favorite in document.Favorites)
            {
                string subject = Named(Favorite, favorite.Name);

                CheckName(Favorite, favorite.Ref, favorite.Name);
                CheckLength($"The name of {subject}", favorite.Name, MaxNameLength);
                CheckLinks(subject, Blueprint, favorite.Blueprints, blueprints);
            }

            return _unlisted == 0 ? _errors : [.. _errors, $"…and {_unlisted} more problems."];
        }

        // Capped as the checks run rather than afterwards: a file within the size limit can still hold millions of bad
        // links, and a list of every one would cost far more memory than the file.
        private void Add(string error)
        {
            if (_errors.Count < MaxListedErrors)
            {
                _errors.Add(error);
            }
            else
            {
                _unlisted++;
            }
        }

        private bool HasNullEntry() =>
            document.Categories.Contains(null)
            || document.Components.Contains(null)
            || document.Blueprints.Contains(null)
            || document.Favorites.Contains(null)
            || document.Blueprints.Any(blueprint => blueprint.Components.Contains(null) || blueprint.Blueprints.Contains(null))
            || document.Favorites.Any(favorite => favorite.Blueprints.Contains(null));

        /// <summary>
        /// Checks the list's size and that each record's ref is positive and its own, and returns the refs in the list.
        /// </summary>
        private HashSet<int> CheckRefs<T>(string noun, string pluralNoun, IReadOnlyList<T> records, Func<T, int> refOf, Func<T, string> nameOf)
        {
            if (records.Count > MaxRecordsPerKind)
            {
                Add($"This file has more than {MaxRecordsPerKind:N0} {pluralNoun}.");
            }

            HashSet<int> refs = [];

            foreach (T record in records)
            {
                int reference = refOf(record);

                if (reference < 1)
                {
                    Add($"{Named(noun, nameOf(record))} has an invalid ref ({reference}).");
                }
                else if (!refs.Add(reference))
                {
                    Add($"More than one {noun} uses ref {reference}, including {Quoted(nameOf(record))}.");
                }
            }

            return refs;
        }

        private void CheckText(string subject, string name, string description)
        {
            CheckLength($"The name of {subject}", name, MaxNameLength);
            CheckLength($"The description of {subject}", description, MaxDescriptionLength);
        }

        private void CheckName(string noun, int reference, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Add($"A {noun} has no name (ref {reference}).");
            }
        }

        private void CheckLength(string what, string text, int maxLength)
        {
            if (text.Length > maxLength)
            {
                Add($"{what} is longer than {maxLength:N0} characters.");
            }
        }

        private void CheckProductionTime(string subject, TimeSpan productionTime)
        {
            if (productionTime < TimeSpan.Zero)
            {
                Add($"{subject} has a negative production time.");
            }
        }

        private void CheckCategory(string subject, int? category, HashSet<int> categories)
        {
            if (category is { } reference && !categories.Contains(reference))
            {
                Add($"{subject} is filed under a category that isn't in the file (ref {reference}).");
            }
        }

        private void CheckLinks(string subject, string targetNoun, IEnumerable<QuantityRef> links, HashSet<int> targets)
        {
            foreach (QuantityRef link in links)
            {
                if (!targets.Contains(link.Ref))
                {
                    Add($"{subject} uses a {targetNoun} that isn't in the file (ref {link.Ref}).");
                }

                // Zero is allowed: the editor and the Craft screen both let a quantity sit at 0.
                if (link.Quantity < 0)
                {
                    Add($"{subject} uses a negative quantity of a {targetNoun} (ref {link.Ref}).");
                }
            }
        }

        /// <summary>"The component 'Copper'", or "A component with no name".</summary>
        private static string Named(string noun, string name) =>
            string.IsNullOrWhiteSpace(name) ? $"A {noun} with no name" : $"The {noun} {Quoted(name)}";
    }
}
