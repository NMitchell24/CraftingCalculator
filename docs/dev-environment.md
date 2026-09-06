# Dev environment — running the app in an IDE (Rider/VS), device & Apple setup

Practical setup notes for building/running this solution locally. Architecture and code conventions
live in the `craftingcalculator-dev` skill (`.claude/skills/craftingcalculator-dev/SKILL.md`); this
file is the "how do I actually launch it on each platform" companion.

> Note for AI assistants on a fresh machine: chat history and local agent memory do **not** sync
> across machines — only this repo does. Treat this file as the source of truth for environment
> setup so you don't re-derive it.

## Windows desktop (CraftingCalculator.UI, `net10.0-windows`)

- Default MAUI Windows builds are **MSIX-packaged**, which Rider can't launch directly (build
  succeeds, then it hangs deploying — endless spinner, no window). If that trips you up, set
  `WindowsPackageType=None` in the `Debug|net10.0-windows…` PropertyGroup of
  `CraftingCalculator.UI.csproj` (leave Release MSIX-packaged for any future Store/sideload
  distribution), and use `"commandName": "Project"` (not `"MsixPackage"`) in
  `src/CraftingCalculator.UI/Properties/launchSettings.json`.
- Use the **"Windows Machine"** run config, not **UWP** (UWP gives no target-framework selector).

## Android (CraftingCalculator.UI, `net10.0-android`)

- **JDK 17** is required (Microsoft.OpenJDK.17). JDK 21 fails `ValidateJavaVersion` (XA0034) with a
  null-path crash. Set `JAVA_HOME` to the 17 path and point Rider's Android JDK setting at it.
- Install SDK deps with:
  ```
  dotnet build src/CraftingCalculator.UI/CraftingCalculator.UI.csproj -t:InstallAndroidDependencies -f net10.0-android -p:AndroidSdkDirectory="<sdk>" -p:AcceptAndroidSDKLicenses=True
  ```
  Run it against the **UI .csproj, not the solution root** — from the root it forces
  `-f net10.0-android` onto every project and throws `NETSDK1005` for the non-android ones (noise).
- Emulator needs **Windows Hypervisor Platform** (WHPX) enabled in Windows Features + reboot — NOT
  full Hyper-V (AEHD is the AMD fallback).
- **Release build is the check that matters** — trimming and interpreter failures do not exist in
  Debug:
  ```
  dotnet build src/CraftingCalculator.UI/CraftingCalculator.UI.csproj -f net10.0-android -c Release
  ```

## Apple — iOS (CraftingCalculator.UI, `net10.0-ios`) — working on the Mac directly

iOS building and device testing happen **on the Mac directly** (clone the repo there), not by
pairing from Windows.

- **Must build on macOS** (Xcode toolchain); the iOS Simulator is macOS-only. Simplest path is to run
  **Rider directly on the Mac**.
- Suggested install order on the Mac: **Xcode** (App Store) → launch once to install components →
  **.NET 10 SDK** + workload (`dotnet workload install maui-ios`) → **Rider** (auto-detects Xcode +
  SDK). Ensure the real **.NET 10 runtime** is present, not just the SDK.
- **Workload version must match Xcode** — the biggest trap. .NET 10 ships the **Apple SDK 26 band**
  (`Microsoft.iOS` 26.x), which requires **Xcode 26**. On an older Xcode the build fails with
  *"requires Xcode 26.x. The current version of Xcode is …"*. Update Xcode first; do **not** roll the
  workload back to an older band to match an old Xcode, because the `net10.0-ios` TFM needs the 26
  band. Verify with `dotnet workload update --print-rollback`. (`allowPrerelease:true` in a stray
  `global.json` is how a preview band gets pulled in accidentally — see the `global.json` note below.)
- **Never `sudo dotnet workload install`.** The SDK lives in `~/.dotnet` (user-owned); installing
  under `sudo` makes `~/.dotnet/metadata` + manifest folders **root-owned**, after which
  non-elevated workload commands fail with *"Inadequate permissions"* and **Rider** (which can't run
  elevated) can't manage workloads. If it already happened:
  `sudo chown -R "$(whoami):staff" ~/.dotnet`, then re-run workload commands without sudo.
- **Simulator first** — it needs no signing/cert. Get "runs in the iOS Simulator" green before
  touching device signing.
- **Simulator signing should be gated in the csproj**: the Debug automatic-provisioning +
  `Apple Development` PropertyGroup should be conditioned to device RIDs only
  (`RuntimeIdentifier` starts with `iossimulator` ⇒ skipped), so the Simulator builds with zero
  Apple-account setup.
- **iOS signing is split by configuration:** **Debug → automatic provisioning + `Apple Development`
  cert** so on-device debugging works against the team account; **Release → manual provisioning +
  `iPhone Distribution`** for App Store submission. Simulator builds aren't codesigned regardless.
  For automatic provisioning to resolve on the Mac, be signed into the Apple ID in the IDE with the
  team selected.
- iOS Release intentionally uses the **interpreter** (`UseInterpreter`, `MtouchInterpreter=all`,
  `PublishAot=false`); a `LinkDescription.xml` handles linker preservation — NativeAOT is not a
  supported combination with EF Core.
- Data layer (`Microsoft.EntityFrameworkCore.Sqlite`) bundles native SQLite for iOS — no extra
  native-SQLite work needed.

## No `MacOS.slnf` needed

Unlike apps that keep a WPF-only companion project, CraftingCalculator's WPF app is deleted once the
migration lands (see the migration plan / PR sequence). After that, every project in
`CraftingCalculator.sln` builds on macOS as-is: `CraftingCalculator.UI.csproj` adds the Windows TFM
only when `$([MSBuild]::IsOSPlatform('windows'))`, so the Mac naturally resolves `ios;android`
without needing a solution filter to exclude anything.

## `global.json` — SDK pin (repo root, committed)

Pins the SDK to **stable .NET 10** (`version 10.0.100`, `rollForward latestMinor`,
`allowPrerelease false`) so no machine accidentally selects a **preview** SDK. Resolves to the
highest installed 10.0.x (e.g. 10.0.301) and never crosses to major 11.

## Cross-machine note

IDE files are git-ignored (`.idea/`). Local agent memory and chat history are **not** shared between
machines; commit anything worth keeping into the repo (like this file and `global.json`).
