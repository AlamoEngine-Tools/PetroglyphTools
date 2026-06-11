# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository purpose

PetroglyphTools is a set of .NET libraries (published as `AlamoEngineTools.*` NuGet packages) for reading and writing the proprietary file formats used by Petroglyph's Alamo / Glyphx engines — most notably Star Wars: Empire at War. Each format gets its own library; `PG.Commons` is the shared foundation.

## Solution layout

The solution file is **`PetroglyphTools.slnx`** (the new XML solution format — not `.sln`). Each format library lives in a top-level folder containing two sibling projects: `PG.<Name>` (production) and `PG.<Name>.Test` (xUnit v3 test runner). Notable cross-cutting projects:

- `PG.Commons` — CRC32 hashing, numerics, `ServiceBase`, file-name validation, stream/string utilities. Foundation for everything else.
- `PG.StarWarsGame.Files` — abstract `PetroglyphFileHolder<TModel, TFileInfo>` and `PetroglyphFileInformation` record types that all file holders extend.
- `PG.Testing` — `PGTestBase` (built on `AnakinRaW.CommonUtilities.Testing.TestBaseWithFileSystem` + Testably.Abstractions for an in-memory `IFileSystem`).
- `PG.StarWarsGame.Files.Testing` — shared file-builder test base.
- `PG.Benchmarks` — BenchmarkDotNet harness (not in the slnx test pipeline).

Format libraries: `PG.StarWarsGame.Files.{MEG,DAT,MTD,Xml,AnimationSfxMap}` and `PG.StarWarsGame.Localisation`. The `Xml` projects are present in the slnx but currently have `<Build Project="false" />` — they are not built or tested by the default solution build.

## Common commands

Run from the repo root. The repo uses `Microsoft.Testing.Platform` (declared in `global.json`), so test projects build as executables and `dotnet test` invokes MTP rather than VSTest.

```pwsh
dotnet build --configuration Release
dotnet test  --configuration Release                 # full test suite (matches CI)
dotnet test  PG.Commons/PG.Commons.Test               # tests for one project
dotnet test  PG.Commons/PG.Commons.Test --filter "FullyQualifiedName~PgFileNameUtilitiesTest"
dotnet pack  --configuration Release --output ./packages   # produces NuGet packages under bin/Packages/
```

Benchmarks are run by executing the `PG.Benchmarks` project directly (`dotnet run -c Release --project PG.Benchmarks`).

## Target frameworks

- Production libraries: `netstandard2.0;netstandard2.1;net10.0` (some are `netstandard2.0;netstandard2.1` only — e.g. `PG.StarWarsGame.Files`).
- Test projects: `net8.0;net10.0`, plus `net481` on Windows.
- `Directory.Build.props` enforces `Nullable=enable`, `ImplicitUsings=disable`, `LangVersion=latest`, and `EnableNETAnalyzers=true`. Most production projects additionally set `TreatWarningsAsErrors=true` and `GenerateDocumentationFile=true` — XML doc comments on public API are mandatory; missing-doc warnings will fail the build.
- Modern language features on `netstandard2.0` are enabled via the `Nullable`, `IsExternalInit`, and `Required` polyfill packages — do not remove these references when editing csproj files.

## Architectural conventions

**Service registration via `*ServiceContribution` classes.** Each library exposes a static class (e.g. `MegServiceContribution.SupportMEG(IServiceCollection)`, `DatServiceContribution.SupportDAT(...)`, `PetroglyphCommons.ContributeServices(...)`) that registers all of its services in DI. Consumers compose the libraries they need; tests call the relevant `Support*` extension from a `Common*TestBase` deriving from `PGTestBase`. **When adding a new service, register it in the corresponding `*ServiceContribution` method** — there is no auto-discovery.

**`ServiceBase` for services, `PetroglyphFileHolder<TModel, TFileInfo>` for file containers.** Services derive from `PG.Commons.Services.ServiceBase`, which captures `IServiceProvider`, `IFileSystem` (always resolved from DI — never use `System.IO` directly so the in-memory test FS works), and an `ILogger`. File holders derive from `PetroglyphFileHolder<TModel, TFileInfo>` and pair a parsed model with a `PetroglyphFileInformation` record (a `record` so it supports `with` expressions; the holder stores its own copy and disposes it).

**MEG packable files.** `PetroglyphMegPackableFileInformation` skips the on-disk existence check when `IsInsideMeg` is true. When writing code that constructs file information for content extracted from a MEG, use this subtype rather than `PetroglyphFileInformation` directly.

**MEG versions.** The MEG library handles three on-disk format versions (`MegFileVersion.V1/V2/V3`) — V1 is what EaW/FoC ship; V3 may be encrypted. Binary readers/writers are split into `Binary/Reader/V1/...` and `Binary/Construction/V1/...` subfolders; do not assume there is only one version when adding code under `Binary/`.

**CRC32 is foundational.** Many Petroglyph formats key entries by CRC32 of an ASCII (uppercased) name. `ICrc32HashingService` and `CrcUtilities` in `PG.Commons` are the canonical entry points; do not introduce a second CRC32 implementation.

## Branching & release

- Day-to-day work targets **`develop`** (CI: `test.yml` runs build+test on Windows and Ubuntu for PRs into `develop`).
- Merges into **`master`** trigger `release.yml`: it runs the test workflow, then `dotnet pack`, pushes packages to nuget.org, and creates a GitHub release tagged `v{SemVer2}`. Versioning is computed by **Nerdbank.GitVersioning** from `version.json` — do not hand-edit version numbers in csproj files.
- The `.vscode/settings.json` references `PetroglyphTools.sln`, which does not exist in the repo. Use the `.slnx` file (or open the folder) instead.
