# Logging on Apple platforms — what still needs checking on the Mac

The diagnostic log (`Infrastructure/Logging/FileLoggerProvider`, wired up in `UI/MauiProgram.cs`) was built and
verified on Windows and Android. Its iOS code path **compiles but has never run** — there was no Mac when it
was written. This is the list of things to confirm the first time the app runs on Apple hardware, and how to
confirm each one.

Background on the feature itself is in the "Logging and privacy" sections of `CLAUDE.md` and
`.claude/skills/craftingcalculator-dev/SKILL.md`. Getting the iOS head building at all is
[dev-environment.md](dev-environment.md) — do that first; nothing here is useful until the Simulator runs the app.

## What the iOS path is supposed to do

```csharp
// MauiProgram.GetLogsPath(), the #if IOS branch
string logsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Logs");
Directory.CreateDirectory(logsPath);
using NSUrl url = NSUrl.FromFilename(logsPath);
_ = url.SetResource(NSUrl.IsExcludedFromBackupKey, NSNumber.FromBoolean(true));
```

`MyDocuments` is the app's `Documents` directory, which `Info.plist` already publishes to the Files app
(`UIFileSharingEnabled` + `LSSupportsOpeningDocumentsInPlace`). That part of the mechanism is proven — `Exports`
has shipped in the same folder since the Import/Export work. What is unproven is the `Logs` subfolder and the
iCloud exclusion on it.

## Getting the log off the device

There is no iOS equivalent of Android's `run-as`, and none is needed: the Files app shows the folder for **any**
build, Debug or Release.

- **Files app:** On My iPhone → Crafting Calculator → `Logs` → `crafting-calculator.log`. Share it to yourself.
- **Simulator, from the Mac shell:**
  ```bash
  open "$(xcrun simctl get_app_container booted com.sterlingtp.craftingcalculator data)/Documents/Logs"
  ```
- **Physical device:** Xcode → Window → Devices and Simulators → pick the app → gear → Download Container, then
  Show Package Contents → `AppData/Documents/Logs`. Works for anything installed with a development profile.

> **Note:** a Debug log is deliberately **not** redacted, and its session header says so. Every privacy check
> below has to run against a **Release** build. Release on the Simulator is fine and is the cheapest way to get
> one.

## The checks

### 1. The log is written, in the right place

Launch the app once, then read the file. The first entry is the session header, and it must name the device
**model**, never the name the user gave the device:

```
2026-09-20 13:17:50.122 -05:00 [Information] Startup: Session started
App: Crafting Calculator 2.0 (build 1)
OS: iOS 26.0
Device: Apple iPhone17,3 (Phone)
Redaction: on
```

`Redaction: off (Debug build)` is correct for a Debug build and is the tell that you are not looking at a
shareable log.

### 2. The Logs folder is excluded from iCloud backup

This is the one line nobody has ever executed. `SetResource` returns `false` on failure and the result is
deliberately discarded — the logger does not exist yet at that point, so there is nowhere to report it. That
means a failure here is **silent**, and diagnostics would quietly ride along into the user's iCloud backup.

Read the flag back to prove it took. A throwaway line in `GetLogsPath` is enough:

```csharp
url.GetResourceValue(NSUrl.IsExcludedFromBackupKey, out NSObject excluded, out NSError _);
System.Diagnostics.Debug.WriteLine($"excluded from backup: {excluded}");   // expect 1 / true
```

On the Simulator you can also check it from the Mac shell, where the flag is stored as an extended attribute:

```bash
xattr -l "$(xcrun simctl get_app_container booted com.sterlingtp.craftingcalculator data)/Documents/Logs"
```

Expect `com.apple.metadata:com_apple_backup_excludeItem` to be present. If neither check shows the flag, the
call is failing and the fix is to take the `out NSError` overload and act on it — see "If the exclusion fails"
below.

### 3. Redaction actually strips the dataset

Release build, then force an export failure the way the Android check did it. On the Simulator the container is
a real folder on the Mac, so the Exports directory can simply be made read-only:

```bash
CONTAINER=$(xcrun simctl get_app_container booted com.sterlingtp.craftingcalculator data)
chmod 555 "$CONTAINER/Documents/Exports"
```

Create a dataset with a recognizable two-word name ("Bronze Age" is a good one — the space is the interesting
case), export once to create the folder, apply the `chmod`, then export again. The log entry must have the full
type and stack trace and **neither** the dataset name nor the container path:

```
System.UnauthorizedAccessException: UnauthorizedAccess_IODenied_Path, <path>.tmp
   at CraftingCalculator.Infrastructure.Files.ExportFileStore.SaveAsync(TransferDocument document, Int32 keep)
---> Inner exception:
System.IO.IOException: Permission denied
```

Put the permissions back with `chmod 755` afterwards.

### 4. The linker leaves the logging intact

iOS Release uses the **interpreter** plus a `LinkDescription.xml`, and trimming is the one thing that can break
this feature in Release only. Two pieces to watch:

- the `[LoggerMessage]` source-generated partial methods, and
- `LogRedactor`'s `[GeneratedRegex]` types.

Both are source-generated precisely so they are trim-safe, so this should be a non-event — but "should be" is
why it is on the list. Check 3 passing on a **Release device build** (not just the Simulator) is the proof: it
exercises a generated log method and both regexes in one go. If a regex is stripped, masking silently stops
happening, which is exactly the failure worth catching before release.

## If the exclusion fails

The call has a second overload that reports why:

```csharp
Boolean SetResource(NSString nsUrlResourceKey, NSObject value, out NSError error)   // the diagnostic one
Boolean SetResource(NSString nsUrlResourceKey, NSObject value)                      // what ships today
```

Making the failure visible means splitting the path from the exclusion, since `GetLogsPath()` runs before
`builder.Build()` and there is no logger yet:

```csharp
string logsPath = GetLogsPath();               // pure
// ... register logging, build the app, create startupLogger, write the session header ...
#if IOS
if (!ExcludeFromBackup(logsPath)) { LogBackupExclusionFailed(startupLogger); }
#endif
```

About ten lines and one more `[LoggerMessage]`. It was left out on purpose rather than written blind against a
platform nobody could run.

## Mac Catalyst

There is no `net10.0-maccatalyst` head yet (see [dev-environment.md](dev-environment.md)). When one is added,
note that **`MACCATALYST` and `IOS` are separate symbols** — `IOS` is not defined for a Catalyst build. So
`GetLogsPath()` would fall through to the `#else` branch, putting the log in `AppDataDirectory/Logs` with no
backup exclusion at all. `GetExportsPath()` has exactly the same `#if IOS`, so both need revisiting together
when that head lands; decide then whether Catalyst wants the Documents folder or app data.

## Later phases

The logging work is phased, and this page covers the file sink only. Phases still to come add a View Logs
dialog, global exception hooks (including iOS's `ObjCRuntime.Runtime.MarshalManagedException`) and a native
startup-failure page — each brings its own Apple-side checks. Add them here as they land.
