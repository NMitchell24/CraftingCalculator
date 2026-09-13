# Export file format — keeping import backward compatible

The `.ccdata` export file has one hard requirement: **every file a released build of the app has ever written must
import into every later build.** This page is the procedure for changing what the file carries without breaking
that. Read it whenever `TransferSchemaTripwireTests` or `TransferFixtureTests` fails, or before adding anything to
the export on purpose.

The `craftingcalculator-dev` skill points here; the code comments on the document types do too.

## The pieces

| What | Where |
|---|---|
| The document types the file is read into and written from | `src/CraftingCalculator.Application/BusinessLogic/Transfer/Format/TransferDocument.cs` |
| Serializer options (source-generated, strict) | `.../Transfer/Format/TransferJsonContext.cs` |
| `Name`, `CurrentVersion`, `LatestShippedVersion`, `FileExtension` | `.../Transfer/TransferFormat.cs` |
| Snapshot → document (export) | `.../Transfer/TransferDocumentProcessor.cs` (`ToDocument`) |
| File → validated snapshot (import), and where an upgrade step goes | `.../Transfer/TransferDocumentReader.cs` (`Read`, `ToSnapshot`, `DocumentChecker`) |
| The in-memory shape both sides share | `src/CraftingCalculator.Domain/Models/Transfer/DatasetSnapshot.cs` and the `Snapshot*` records beside it |
| Database ↔ snapshot | `src/CraftingCalculator.Infrastructure/DAO/Impl/DatasetDAO.cs` (`GetSnapshotAsync`, `ImportAsNewAsync`, `MergeAsync`) |
| One real export per format version | `tests/CraftingCalculator.Application.UnitTests/BusinessLogic/Transfer/Fixtures/v{n}/*.ccdata` |
| The app version the file records | `ApplicationDisplayVersion` in `src/CraftingCalculator.UI/CraftingCalculator.UI.csproj` |
| The tests that enforce all of this | `TransferFixtureTests`, `TransferFixtureImportTests`, `TransferSchemaTripwireTests` |

The envelope every file starts with:

```json
{
  "format": "crafting-calculator-dataset",
  "formatVersion": 1,
  "exportedAt": "2026-09-12T18:04:00+00:00",
  "appVersion": "2.0",
  "datasetName": "Valheim",
  ...
}
```

`formatVersion` is what the reader checks; `appVersion` is only shown to the user. The two move independently.

## Shipped, or still in development?

A format version exists in one of two states, and which one decides everything below:

```csharp
// TransferFormat.cs
public const int CurrentVersion = 1;        // what this build writes
public const int LatestShippedVersion = 0;  // the newest version a released build has written; 0 until the first release that exports
```

- **`CurrentVersion == LatestShippedVersion`: the current version has shipped.** Users have files of it. A change to the
  file bumps `CurrentVersion` and adds `Fixtures/v{n+1}`; `Fixtures/v{n}` is frozen.
- **`CurrentVersion > LatestShippedVersion`: the current version is in development.** No user has a file of it, so a
  change to the file regenerates `Fixtures/v{CurrentVersion}` in place. No bump, no new folder, no upgrade step.

`TransferFixtureTests.TheFormatVersion_IsBumpedAtMostOncePerRelease` holds `CurrentVersion` to at most
`LatestShippedVersion + 1`. That is the whole point of the split: **one format version per released app version, and
only when the file actually changed.** A version that gets bumped for every edit during development leaves behind
fixtures no user could ever have produced, and every one of them has to import forever.

The app itself is on the same footing. Version 1.x was a different app on a different stack with no export; version
2.0 is the first that exports, and it is in development. Until it ships, every file the app writes is format 1, and
format 1 is regenerated as often as it changes.

## After a release

Releases are tagged from the build pipeline. The first commit after a tag does two things, together:

1. Bump `ApplicationDisplayVersion` in `CraftingCalculator.UI.csproj`, so every later build falls after the tag.
2. Set `LatestShippedVersion` to `CurrentVersion` in `TransferFormat.cs`.

From that commit on, every `Fixtures/v{n}` with `n <= LatestShippedVersion` is frozen: people have files of each of
those versions. The tests read whatever is checked in, so nothing in CI notices a frozen fixture being edited; the review
of the PR is the guard. Treat a diff under a shipped `Fixtures/v{n}` the way you would treat an edit to a released
migration.

## The rule

There is one set of document types, and it always describes the **newest** format version. Older files read through
the same types. That works because of three rules:

1. **Members are only ever added, never renamed, removed or re-meant, and every new member has a default** that
   stands in for what an older file does not say. The serializer fills it in, so a version 1 file deserializes through
   version 3's types with no code that knows about version 1.
2. **A change to the file made on a shipped version bumps `TransferFormat.CurrentVersion` and adds a fixture under
   `Fixtures/v{n}`.** The bump is what lets an older app say "made by a newer version of Crafting Calculator, update
   the app" instead of failing on an unknown member. The fixture is what proves, on every PR from now on, that files
   of that version still import. A change made while the version is in development regenerates its fixtures instead.
3. **A change that cannot be additive gets an upgrade step in the reader**, written against the raw JSON and keyed
   by the shipped version it upgrades from. The types still only describe the newest version.

What never happens:

- A shipped version's fixture is never edited or deleted. A test failing on one means a change broke files people
  have.
- A new member is never required. `RespectRequiredConstructorParameters` is on, so a parameter without a default
  *is* required, and every older file stops importing.
- A `V2` copy of the document types is never made. Old versions live in fixtures and, when needed, in an upgrade
  step, not in parallel type sets.
- A shipped version number is never reused, and a version is never bumped twice between releases.
- An upgrade step is never written for a version that has not shipped. Nobody has a file to upgrade.

## Deciding whether the file carries a change

`TransferSchemaTripwireTests` fails the moment an exported entity gains, loses or renames a column. That is the
prompt to decide, not a bug to silence. Two outcomes:

- **The file carries it.** Follow one of the procedures below, then add the column to the pinned list in
  `TransferSchemaTripwireTests`.
- **The file deliberately leaves it out** (a cache, a UI-only preference, something recomputed on import). Add the
  column to the pinned list with nothing else; the export ignores it and import leaves it at the database default.

## Procedure A — a new column on an exported record

Example: components gain a `Weight` (`double`, default 0) in the database.

1. **Snapshot record.** Add the value to the `Snapshot*` record in `Domain/Models/Transfer` and fill it in
   `DatasetDAO.GetSnapshotAsync`, `ImportAsNewAsync` and `MergeAsync`. `SnapshotModelProcessor` and
   `ImportConflictProcessor` need it only if the value takes part in the UI or in conflict detection.

   ```csharp
   public sealed record SnapshotComponent(
       int Id, string Name, string Description, double Cost, TimeSpan ProductionTime, int? CategoryId,
       double Weight);
   ```

2. **Document type.** Add the member *last*, with the default an older file implies. Use the same default the
   database migration gave existing rows, so an old file and an old row agree.

   ```csharp
   public sealed record TransferComponent(
       int Ref, string Name, string Description, double Cost, TimeSpan ProductionTime, int? Category,
       double Weight = 0);   // format 2
   ```

   A nullable reference gets `= null`; a list is `IReadOnlyList<T>? Foo = null` and is normalized in `ToSnapshot`
   with `document.Foo ?? []`.

3. **Writer and reader.** Copy the value across in `TransferDocumentProcessor.ToDocument` and in
   `TransferDocumentReader.ToSnapshot`. If the value has rules (a range, a required link), add the check to
   `DocumentChecker.Run` next to the existing ones, so a bad file lists it with everything else.

4. **Bump the version, only if the current one has shipped.**

   ```csharp
   public const int CurrentVersion = 2;   // was 1, and 1 == LatestShippedVersion
   ```

   If `CurrentVersion` is already ahead of `LatestShippedVersion`, skip this step: the version in development
   absorbs the change.

5. **Fixtures.** Two files per version, both real exports, under `Fixtures/v{CurrentVersion}`. After a bump that is a
   new folder; during development it is the existing one, overwritten.

   - `bronze-chain.ccdata` is the writer's own output for the in-memory bronze chain, and the test that compares
     them fails until the file matches. Regenerate it with the explicit test, which writes into the source tree:

     ```bash
     dotnet test tests/CraftingCalculator.Application.UnitTests --filter FullyQualifiedName~WriteTheCurrentBronzeChainFixture
     ```

   - `valheim.ccdata` is a full export of the Valheim test dataset from a device or the Windows build. Export it
     from the app and copy the file in.

   Every folder up to `v{LatestShippedVersion}` stays exactly as it is. Both test projects pick up every `*.ccdata`
   under `Fixtures` by glob; nothing to add to a `.csproj`.

6. **Tripwire list.** Add the column to the entity's pinned properties in `TransferSchemaTripwireTests`.

7. **Help.** If the user can see the value, `docs/help/import-export.md` or the record's own page says so, in the
   same PR.

That is the whole cost: one defaulted parameter, two copy lines, two fixture files, a list entry, and a constant if
the version had shipped. No upgrader, because a version 1 file already reads correctly: it lacks `weight`, so the
deserializer supplies 0.

## Procedure B — a new record kind in the export

Example: the app gains **tags**, a fourth kind of record with its own table and a link table to blueprints.

Everything in Procedure A applies. The new list is nullable-with-null-default on the document, so an older file,
which has no `tags` list at all, reads as empty:

```csharp
public sealed record TransferDocument(
    string Format,
    int FormatVersion,
    DateTimeOffset ExportedAt,
    string AppVersion,
    string DatasetName,
    IReadOnlyList<TransferCategory> Categories,
    IReadOnlyList<TransferComponent> Components,
    IReadOnlyList<TransferBlueprint> Blueprints,
    IReadOnlyList<TransferFavorite> Favorites,
    IReadOnlyList<TransferTag>? Tags = null);   // format 3: a format 1 or 2 file has no tags

public sealed record TransferTag(int Ref, string Name);
```

```csharp
// TransferDocumentReader.ToSnapshot
[.. (document.Tags ?? []).Select(tag => new SnapshotTag(tag.Ref, tag.Name))]
```

Beyond the file itself, a new kind touches the pieces that enumerate kinds: `RecordKind`,
`DependencyGraphProcessor.Build` (every link the new kind takes part in is an edge), `DocumentChecker.Run` (refs
unique and positive, links resolve, names present), `TransferSelectionProcessor`, the transfer panels in the UI, and
the pinned list in `TransferSchemaTripwireTests` gains the new entity and its link entity. The `TransferJsonContext`
needs no change: it registers the root document, and the generator follows the members.

## Procedure C — a change that is not additive

Renaming a member, removing one, or changing what a value means (units, sign, a ref that now points at a different
kind). Avoid this when an additive change would do: keep the old member, add the new one, and let the writer stop
filling the old one.

**While the current version is in development, none of this applies.** Change the types to the new shape,
regenerate the fixtures, done: the old shape never left the repo. The procedure below is for a shape that shipped.

Example: `cost` on a component becomes `unitCost`, and it is per unit rather than per stack, after version 2 shipped.

1. Change the document type to the new shape and bump the version, as in Procedure A. The type now describes
   version 3 only.

2. Add the upgrade step in `TransferDocumentReader.Read`, between the version check and deserialization, working on
   the parsed JSON. Sketch, to be written against the real change:

   ```csharp
   // TransferDocumentReader.Read, inside the using after CheckFormat, replacing the plain Deserialize call.
   // The stream is already consumed, so the node is built from the parsed root, which must stay undisposed.
   JsonObject root = JsonObject.Create(json.RootElement)!;

   for (int version = json.RootElement.GetProperty(JsonName(nameof(TransferDocument.FormatVersion))).GetInt32();
        version < TransferFormat.CurrentVersion; version++)
   {
       TransferUpgrades.Apply(root, version);   // version → version + 1
   }

   document = root.Deserialize(TransferJsonContext.Default.TransferDocument) ?? throw new JsonException();
   ```

   ```csharp
   /// <summary>The upgrade steps, one per shipped format version whose successor is not a superset of it.</summary>
   internal static class TransferUpgrades
   {
       public static void Apply(JsonObject root, int fromVersion)
       {
           switch (fromVersion)
           {
               case 2: CostBecomesUnitCost(root); break;
               // A version whose successor only added members needs no case.
           }

           root["formatVersion"] = fromVersion + 1;
       }

       private static void CostBecomesUnitCost(JsonObject root)
       {
           foreach (JsonObject component in root["components"]!.AsArray().Select(node => node!.AsObject()))
           {
               // A node has one parent; detach it before assigning it under the new name.
               JsonNode? cost = component["cost"];
               component.Remove("cost");
               component["unitCost"] = cost;
           }
       }
   }
   ```

   The loop runs each step in order, so a version 1 file passes through every step from 1 up. A step is only needed
   for the version that broke; the loop skips the additive ones.

3. Test the step directly with a hand-written JSON in the **old** shape, read through `TransferDocumentReader.Read`,
   asserting on the snapshot. The old fixtures under `Fixtures/v1` and `Fixtures/v2` keep passing through the same
   path, which is the real proof.

4. Fixtures, tripwire list and help as in Procedure A.

## The checklist for any change to the file

- [ ] The document types describe the new shape only; every new member has a default.
- [ ] `TransferFormat.CurrentVersion` went up by one **if and only if** it equaled `LatestShippedVersion`.
- [ ] `Fixtures/v{CurrentVersion}/bronze-chain.ccdata` regenerated with `WriteTheCurrentBronzeChainFixture`.
- [ ] `Fixtures/v{CurrentVersion}/valheim.ccdata` exported from a running app.
- [ ] No file under `Fixtures/v{n}` for `n <= LatestShippedVersion` changed (`git status` shows none).
- [ ] `TransferSchemaTripwireTests` pinned list updated.
- [ ] An upgrade step and its test, only if the change was not additive **and** the old shape had shipped.
- [ ] `docs/help/import-export.md` still tells the truth.
- [ ] `dotnet test CraftingCalculator.Tests.slnf` is green.

## Why it is set up this way

The first design froze each version's types (`TransferDocumentV1`, a future `V2`, ...) and required a new type set
plus an upgrader for any change. That made an additive column, the common case, cost as much as a breaking one, and
it left every old type set as dead code once its successor shipped: the upgrader works on JSON and never needed the
old types. The fixtures were always the thing that proved compatibility, so the rule was reshaped around them.

The second design bumped the version on every change to the file. That would have filled `Fixtures/` with versions
no released build ever wrote, each one a file to import forever. `LatestShippedVersion` is what lets the bump wait
for a release: a version nobody has can change freely, and the one users have is frozen the day they get it.

The serializer stays strict (`UnmappedMemberHandling.Disallow`, `RespectNullableAnnotations`,
`RespectRequiredConstructorParameters`) because a version bump on every shipped change means a newer file is turned
away by the version check before strictness ever sees it, and a mangled file is still rejected.
