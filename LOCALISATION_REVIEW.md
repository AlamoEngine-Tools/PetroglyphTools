# Code Review — `PG.StarWarsGame.Localisation` & `PG.StarWarsGame.Localisation.Baseline`

**Reviewer baseline:** `PG.Commons`, `PG.StarWarsGame.Files.{MEG,DAT,MTD,AnimationSfxMap}`, `PG.StarWarsGame.Files`
**Date:** 2026-06-11
**Branch:** `features/gruenwaldlk/localisation_layer`
**Verdict:** Solid, readable, well-tested first cut that would pass a casual look — but it quietly diverges from several load-bearing conventions of the mature libraries, ships some dead/speculative code, and has two genuine correctness traps. **Not yet at parity with the rest of the repo.**

---

## TL;DR scorecard (vs. the established libraries as the "good" baseline)

| Dimension | Baseline standard | Localisation libs | Grade |
|---|---|---|---|
| Project/csproj hygiene | TWAE, doc-gen, multi-target, NB.GitVersioning | **Matches exactly** | A |
| XML doc coverage | Full public API docs | Full on public API (enforced) | A |
| File/namespace/layout | one-type-per-file, namespace=folder | Consistent | A |
| Test breadth & readability | AAA, `Method_Condition_Expected` | Numerous, clean, well-named | A− |
| Equality / value semantics | Correct `Equals`/`GetHashCode` | Correct & thorough | A |
| **Service base class** | everything derives `ServiceBase` | **Nothing does** | D |
| **Encapsulation** | `internal sealed` impl + public iface | **Impls are `public`** | C− |
| **DI discipline** | resolve from container | **`new`s up collaborators** | C− |
| **Error handling / logging** | throws `BinaryCorruptedException`, has `ILogger` | **silent swallow, zero logging** | D |
| **Dead / speculative code** | none tolerated (TWAE) | v1 config + descriptor metadata unused | C |
| **Test hermeticity** | `PGTestBase` + in-memory FS | Baseline tests hit **real disk** | C |

---

## What's genuinely good (credit where due)

These are not throwaway compliments — the library gets the basics right in ways that many first drafts don't:

- **Build/packaging parity is exact.** Both `.csproj` files mirror the baseline: `netstandard2.0;netstandard2.1;net10.0`, `TreatWarningsAsErrors`, `GenerateDocumentationFile`, `InheritDocEnabled`, the `IsExternalInit`/`Nullable`/`Required` polyfills, snupkg symbols, and proper `AlamoEngineTools.*` package IDs. Both projects are correctly registered in `PetroglyphTools.slnx`.
- **Public API is fully documented.** Every public type/member carries `<summary>`, and interfaces document `<exception>`/`<returns>` (e.g. `ILanguageService.cs:23-51`, `ITranslationDatabase.cs:18-38`). With TWAE on, this is enforced, but it's still clean.
- **Value semantics are done properly.** `AlamoLanguageDefinitionBase` implements `IEquatable<>`, `Equals(object)`, and a matching `GetHashCode` over an ordinal-ignore-case identifier (`AlamoLanguageDefinitionBase.cs:62-74`). `TranslationProjectDescriptor` does the same including `SequenceEqual` over languages (`TranslationProjectDescriptor.cs:47-68`). This is why using `IAlamoLanguageDefinition` as a dictionary key in `TranslationEntry` is actually safe.
- **Format adapters depend on abstractions, not the filesystem.** Importers/exporters take `TextReader` / `XDocument` / `IDatModel` (e.g. `CsvTranslationImporter.cs:29`, `XmlTranslationExporter.cs:21`). That's a clean, testable seam and correctly avoids `System.IO` in the core library.
- **The reflection-based integrity test is a nice touch** (`AlamoLanguageDefinitionIntegrityTest.cs`): asserts exactly 11 languages, exactly one default, unique identifiers. That's the kind of invariant guard the baseline libraries favor.
- **Tests are plentiful and readable**, following the repo's `Method_Condition_Expected` xUnit convention with clear AAA structure.

---

## Critical findings (correctness — fix before merge)

### C1. `OrderedTranslationDatabase` violates the `ITranslationDatabase.SetTranslation` contract
`ITranslationDatabase.cs:18-22` documents `SetTranslation` as *"Adds **or updates** … returns `true` if a new entry was created; `false` if an existing entry was **updated**."*

`KeyedTranslationDatabase.SetTranslation` honors this (`KeyedTranslationDatabase.cs:42-57`). But `OrderedTranslationDatabase.SetTranslation` **always appends and always returns `true`** — it never updates (`OrderedTranslationDatabase.cs:41-50`). The test `SetTranslation_AllowsDuplicateKeys` (`OrderedTranslationDatabaseTest.cs:38-45`) shows this is intentional, but the shared interface is never updated to say so.

This is a Liskov substitution problem: code written against `ITranslationDatabase` gets silently different behavior depending on the concrete type, with no compile-time signal. Worse, for an ordered DB, setting two languages on "the same key" produces **two separate single-language entries** (`MultiLanguage_StoredPerLanguagePerEntry`, `Count == 2`), which is surprising under a method named `SetTranslation`. Either split the contract (e.g. `AddEntry` vs `SetTranslation`) or document the divergence on the interface explicitly.

### C2. `KeyedTranslationDatabase` indexer relies on unspecified ordering and is O(n)
```csharp
public ITranslationEntry this[int index] => _entries.Values.ElementAt(index);   // KeyedTranslationDatabase.cs:23
```
The type implements `IReadOnlyList<ITranslationEntry>`, whose indexer implies a **stable, positional** order. `Dictionary<,>` enumeration order is not contractually guaranteed and visibly changes after `Remove`. So `db[i]` can return different entries before/after a removal. It's also `O(n)` per access → `O(n²)` for an index loop. The baseline libraries are deliberately careful about allocation/throughput (spans, CRC); this indexer would not pass that bar. Back the keyed store with something order-preserving (e.g. keep a parallel `List<string>` of keys, or use an insertion-ordered structure).

### C3. CSV export/import is asymmetric — quoted multi-line values cannot round-trip
`CsvTranslationExporter.Escape` deliberately quotes values containing `\n` (`CsvTranslationExporter.cs:58-59`), but `CsvTranslationImporter.Import` reads the file with `source.ReadLine()` (`CsvTranslationImporter.cs:48`). A quoted field that contains a newline spans multiple `ReadLine()` results, so the hand-rolled `ParseCsvLine` (which is single-line) will corrupt it. Any translation string containing a line break exports fine and re-imports broken. This is a real data-loss bug, and it's the standard reason teams reach for a vetted CSV parser instead of hand-rolling one.

---

## Major findings (convention drift from the baseline)

### M1. No service derives from `ServiceBase`
The single strongest convention in this repo is *"services derive from `PG.Commons.Services.ServiceBase`,"* which gives every service a DI-resolved `IFileSystem`, `IServiceProvider`, and `ILogger` (NullLogger by default). **Not one Localisation service does this:**
- `LanguageService` (`LanguageService.cs:14`) — plain class, ctor takes `IEnumerable<IAlamoLanguageDefinition>`.
- `DatTranslationExporter` (`DatTranslationExporter.cs:17-25`) — stashes a raw `IServiceProvider`.
- `BaselineTranslationProvider` (`BaselineTranslationProvider.cs:42-49`) — stashes individual services pulled via `GetRequiredService`.
- `CsvTranslationImporter` / `XmlTranslationImporter` — ctor-inject `ILanguageService`.

The result is **four different injection styles in one library** where the baseline has exactly one. A direct consequence is M3 (no logging anywhere).

### M2. Implementations are `public`; the baseline hides them as `internal sealed`
Baseline pattern: public interface + `internal sealed` implementation, so the concrete type can evolve freely. Localisation exposes the concrete classes: `TranslationDatabaseFactory` (`public sealed`, `TranslationDatabaseFactory.cs:15`), `CsvTranslationExporter`, `XmlTranslationImporter`, etc. are all `public`. Consumers can (and the tests do) `new TranslationDatabaseFactory()` directly, bypassing DI and welding the public surface to today's implementation. The only types that *should* be public are the interfaces and the data/enum/descriptor types.

### M3. Silent failure everywhere, and zero logging
Baseline parsers throw `BinaryCorruptedException` on malformed input and carry an `ILogger`. The Localisation importers swallow everything:
- `CsvTranslationImporter.cs:35,38` — `null`/short header → silent `return`; unknown language column → silently dropped (`:58-59`).
- `XmlTranslationImporter.cs:34,39,48` — missing root / missing `key` / unknown language → silent `continue`.
- `PropertiesTranslationImporter.cs:32` — line without `=` silently skipped.

No `ILogger` exists in the entire library, so a half-imported file produces **no diagnostic at all**. For a tool whose whole job is moving translation data between formats, "imported 0 of 5000 rows, said nothing" is a bad failure mode. At minimum, log skips; ideally surface a structured result or throw on clearly malformed input, matching the baseline's fail-loud posture.

### M4. `DatTranslationExporter` bypasses DAT's DI construction
```csharp
IDatBuilder builder = source is IKeyedTranslationDatabase
    ? new EmpireAtWarMasterTextBuilder(false, _services)
    : new EmpireAtWarCreditsTextBuilder(_services);   // DatTranslationExporter.cs:33-35
```
It `new`s up DAT builders and hands them the service provider, instead of resolving the DAT library's intended builder abstractions from the container. This couples Localisation to concrete DAT types and is exactly the pattern the `*ServiceContribution`/DI design exists to avoid. (The tests repeat this — `DatTranslationAdapterTest.cs:27,45,90`.)

---

## Moderate findings

### D1. `Config/v1` is dead code
`LocalisationType`, `TranslationType`, `LocalisationDataType` are XSD-generated XML DTOs (`[GeneratedCode("xsd", …)]`, `Config/v1/`) and are **referenced by nothing** outside their own folder — the XML importer/exporter use raw `XDocument` instead (`XmlTranslationImporter.cs`). Either wire them up or delete them. Shipping unused generated types in a TWAE codebase is out of character for this repo.

### D2. `TranslationProjectDescriptor` metadata is write-only
`Game`, `OverrideType`, `ResourceType` are stored and compared but **never read by any logic** — `TranslationDatabaseFactory.CreateKeyed/CreateOrdered(descriptor)` use only `descriptor.Languages` (`TranslationDatabaseFactory.cs:25-29,39-43`); the only readers are the descriptor's own unit test. There's no parser that builds a descriptor from a config file, despite the `Data/Config/v1` + `v2` namespace split implying a planned on-disk config format. This reads as speculative scaffolding that isn't finished. Decide whether the descriptor drives anything; if not, trim it.

### D3. `BaselineTranslationProvider` writes temp files to real disk to load embedded DATs
`LoadInto` copies each embedded resource to `GetTempPath()/pg_baseline_{guid}.dat`, calls `_datFileService.Load(tempPath)`, then deletes it (`BaselineTranslationProvider.cs:96-122`). But `IDatFileService` already exposes `Load(FileSystemStream)` (`IDatFileService.cs:65`). The temp-file dance is avoidable; even if a `FileSystemStream` is required, copying into the injected `IFileSystem` (in tests an in-memory FS) would keep it hermetic. As written, and because the Baseline tests register a `RealFileSystem` (see T3), these tests touch the actual OS temp directory.

### D4. `LanguageService` throws an undocumented exception on duplicate identifiers
`_byIdentifier = list.ToDictionary(l => l.LanguageIdentifier, …)` (`LanguageService.cs:39-42`) throws `ArgumentException` if two registrations share an identifier. That's a reasonable invariant, but it's undocumented and would surface as an opaque container-construction failure. Guard it with a clear message or document it.

---

## Testing findings

### T1. `CommonLocalisationTestBase` duplicates the entire `SupportLocalisation()` body
`CommonLocalisationTestBase.SetupServices` (`CommonLocalisationTestBase.cs:18-46`) re-lists all 11 language registrations and all 8 adapter registrations **verbatim** instead of calling `serviceCollection.SupportLocalisation()`. Two sources of truth that will drift. The baseline test bases call the real `Support*` extension (e.g. `CommonMegTestBase` → `SupportMEG()`). Just call `SupportLocalisation()`.

### T2. Tests bypass DI even when they have a DI test base
`DatTranslationAdapterTest` derives from `CommonLocalisationTestBase` (so it has a configured `ServiceProvider`) yet builds objects with `new TranslationDatabaseFactory()` and `new EmpireAtWarMasterTextBuilder(...)` (`DatTranslationAdapterTest.cs:27,32,45,…`). The data-layer tests (`KeyedTranslationDatabaseTest`, `OrderedTranslationDatabaseTest`) don't derive from the base at all. The registered object graph is therefore under-exercised.

### T3. Baseline tests abandon the repo's hermetic test harness
`CommonBaselineTestBase` (`CommonBaselineTestBase.cs`) does **not** derive from `PGTestBase`; it hand-builds a `ServiceProvider`, implements `IDisposable`, and `BaselineServiceContribution` registers a real `Testably.Abstractions.RealFileSystem` (`BaselineServiceContribution.cs:23`). Combined with D3, the baseline tests read/write real disk — slower, non-deterministic, platform-sensitive, and contrary to the whole point of the Testably in-memory `IFileSystem` the rest of the repo standardizes on. (Also note `SupportLocalisationBaseline` self-registers `IFileSystem`, which none of the other `Support*` methods do — the filesystem is normally contributed by `PetroglyphCommons`.)

---

## Minor / nits

- **No `ThrowHelper`.** The baseline uses `AnakinRaW…ThrowHelper.ThrowIfNullOrEmpty/WhiteSpace`; Localisation uses hand-written `if (x is null) throw new ArgumentNullException(...)` everywhere. Functionally fine (the baseline is itself mixed), but keys are never validated for empty/whitespace — `SetTranslation("", …)` is silently accepted.
- **CSV uses hardcoded `'\n'`** (`CsvTranslationExporter.cs:34,50`) rather than `Environment.NewLine`. Defensible for deterministic diffs, but undocumented.
- **Magic strings** in `BaselineTranslationProvider` (`"mastertextfile"`, `"creditstext"`, the language-folder map) would be better as named constants.
- **`PropertiesTranslationImporter`** only unescapes `\=`, `\:`, `\#` in the key and does no value unescaping / line-continuation handling — likely asymmetric with any escaping done elsewhere.

---

## Recommended order of attack

1. **C1–C3** (correctness): fix the ordered-DB contract, the keyed indexer ordering/perf, and CSV multi-line round-trip. These are bugs, not style.
2. **M3** (logging/fail-loud) and **M1** (`ServiceBase`): adopt `ServiceBase` so every service gets an `ILogger`, then stop swallowing malformed input silently.
3. **M2/M4**: make implementations `internal sealed`, expose only interfaces, and resolve DAT builders from DI.
4. **D1/D2**: delete the dead v1 config types and either finish or trim the descriptor metadata.
5. **T1–T3**: call `SupportLocalisation()` from the test base, route tests through DI, and put the Baseline tests back on `PGTestBase` + in-memory FS.

Once those land, this layer would sit comfortably alongside the MEG/DAT libraries. Today it's a good draft wearing the repo's clothes, not yet a member of the family.
