# Migrating a Connector/Enricher to Multi-Version Targeting

This document tracks the migration of `CluedIn.Enricher.CompanyHouse` from a single-version build to
the multi-version targeting pattern.

Prior art: `CluedIn.Connector.Dataverse.V2`, `CluedIn.Enricher.Gleif`, `CluedIn.Enricher.OpenCorporates`,
`CluedIn.Enricher.Permid`, `CluedIn.Enricher.Brreg`, `CluedIn.Enricher.GoogleMaps`,
`CluedIn.Connector.AzureEventHubs` migration docs, plus `crawler.build.jobs.yml` /
`common/detect-tfm.yml` directly (the authoritative source for the pipeline schema and package
suffix scheme).

Branch: `feature/multi-version-targeting` (off `develop`).

---

## Overview

| CluedIn version | .NET TFM | Package suffix |
|---|---|---|
| 4.7.0 | net6.0 | `.470` |
| 4.8.0 | net6.0 | `.480` |
| 5.0.0-beta.* | net10.0 | `.500` |

Verified independently (2026-09-10/11): `5.0.0-*` resolves to `5.0.0-beta.576` against this repo's
own feeds. 4.6.0 excluded - no evidence this provider needs it (small `ExternalSearch` API surface,
no stream-repository usage).

---

## Step 1 — Pipeline template (`azure-pipelines.yml`)

Status: **Done**

Switched from `crawler.build.yml` to `crawler.build.jobs.yml`, added `multiVersionCluedInTargets`
and `probeCluedInVersion`. Explicitly set `useGitVersionDotNetTool: true` — Permid's migration found
that omitting this falls back to the retired legacy `GitVersionTask@5` (Node6 runtime), which fails
outright on the hosted agent. Kept `pool: ubuntu-22.04` even though this repo's old pipeline used
`windows-latest` (Brreg's migration found no reason it couldn't switch either).

---

## Step 2 — `Directory.Build.props`

Status: **Done**

Honours `CluedInMultiVersionTargetFramework` (net10.0 local fallback); derives
`CLUEDIN_V47`/`V48`/`V50` `DefineConstants` and `_CluedInPackageSuffix` (dotless
`Major.Minor.Patch`, needed for the `CluedIn.Testing.Base` reference in Step 4); pinned
`LangVersion` to 13.0 up front (multiple prior repos hit `CS8936` on net6.0 without this — this
repo turned out not to need it, but pinning defensively costs nothing).

---

## Step 3 — `Packages.props`

Status: **Done**

Guarded `_CluedIn`. Split `Microsoft.NET.Test.Sdk`/`AutoFixture.Xunit3`/`xunit.v3`/
`xunit.runner.visualstudio` into `CLUEDIN_V50`-conditional pairs (this repo's `Packages.props` had
them hardcoded to the v3/net10.0 set unconditionally — same shape MasterDataServices' doc warned
about). Switched `CluedIn.Testing.Base` to the version-suffixed package ID
(`CluedIn.Testing.Base.$(_CluedInPackageSuffix)`) — confirmed `.470`/`.480`/`.500` all exist on the
feed before wiring it up.

---

## Step 4 — Test project

Status: **Done**

- `test/Directory.Build.props` — stripped to just `IsTestProject`; removed the unconditional
  `PackageReference`s (avoids the xunit v2/v3 clash).
- `test/integration/Integration.Tests/ExternalSearch.CompanyHouse.Integration.Tests.csproj` — added
  the conditional `ItemGroup`s (xunit v3 + AutoFixture.Xunit3 under `CLUEDIN_V50`, xunit v2 +
  AutoFixture.Xunit2 otherwise) and switched to `CluedIn.Testing.Base.$(_CluedInPackageSuffix)`.
- No `GlobalUsings.cs` needed — the one test file (`CompanyHouseGraphTests.cs`) uses only `[Fact]`/
  `Assert` (compatible namespace on both xunit v2/v3) and no AutoFixture attributes.
- **Enabled `executeIntegrationTests: true`** (was implicitly `false`/unset before). This is a
  judgment call worth flagging: the *old* `crawler.build.yml` pipeline built `**/*.sln`
  unconditionally, so this integration test project was always at least *compiled* by CI even
  though never *run*. The new jobs-template's main build job only builds `src/*.csproj` - if
  `executeIntegrationTests` were left off, this project would stop being touched by CI at all
  (a silent coverage regression, not just "tests don't run"). Enabling it instead preserves (and
  improves on) the existing coverage. The one test (`CompanyHouseGraphTests.Test`) is fully mocked
  (`BaseExternalSearchTest`, no live API calls), so it's safe to actually run in CI.

---

## Step 5 — API compatibility audit across 4.7.0 / 4.8.0 / 5.0.0-beta.*

Status: **Done**

Built (`dotnet build`) **and ran real `dotnet test`** against all three legs — 0 build errors, all
tests pass on every leg.

### RestSharp 106-vs-114 break (same family every enricher so far has hit)

`CompanyHouseClient.cs` and `CompanyHouseExternalSearchProvider.cs` were written against RestSharp
114.0.0's API (net10.0/5.0+). RestSharp 106.15.0 (net6.0/4.7-4.8) differs in several ways, confirmed
via reflection against the actual restored DLLs (both versions loaded side-by-side), not guessed:

- `RestClient(string, configureSerialization:)` and `RestSharp.Serializers.Json.UseSystemTextJson`
  are 107+-only — 106.x has no equivalent constructor overload or extension method. Guarded the
  whole constructor body with `#if CLUEDIN_V50`; the 106.x branch just does
  `new RestClient("https://api.companieshouse.gov.uk")` with RestSharp's default deserializer.
- `Method.Get` (PascalCase, 114.x) vs `Method.GET` (106.x) — 2 call sites.
- `ExecuteAsync<T>` returns `Task<RestResponse<T>>` (114.x, a concrete class deriving from the
  non-generic `RestResponse`) vs `Task<IRestResponse<T>>` (106.x, an *interface* implementing the
  non-generic `IRestResponse` - does **not** derive from the concrete `RestResponse` class). This
  meant every place a generic response was passed to a method expecting the non-generic response
  type needed the parameter type itself guarded (`RestResponse` vs `IRestResponse`), not just the
  method body — done with inline `#if` blocks in the method signatures themselves (valid C#
  preprocessor usage, since it operates on raw text) rather than duplicating whole methods.
- `Parameter.Value` (106.x, on `IRestResponse.Headers: IList<Parameter>`) is typed `object`;
  `HeaderParameter.Value` (114.x, on `RestResponse.Headers: IReadOnlyCollection<HeaderParameter>`)
  is typed `string`. Rather than another `#if`, normalized with `.Value?.ToString()` — works
  identically on both versions since 106.x's boxed value is always a string in practice, so no
  conditional compilation was needed here at all.

No other API breaks found — `Microsoft.PowerPlatform`/Dataverse-style SDKs aren't used by this
repo; it's a plain `CluedIn.Core`/`CluedIn.ExternalSearch` provider.

---

## Step 6 — Reset the semantic version (`GitVersion.yml`)

Status: **Done**

```yaml
next-version: 1.0
ignore:
  sha: []
  commits-before: 2026-03-20T00:00:00
```

**Note:** this repo's `GitVersion.yml` already had an `ignore: sha: []` block further down the
file - had to merge the new `commits-before` into that *same* `ignore:` mapping rather than adding
a second top-level `ignore:` key (YAML doesn't merge duplicate keys; the second one would have
silently discarded the first, losing the `commits-before` setting entirely with no error). Worth
checking for on every repo, not just this one.

Highest pre-existing tag: `4.6.2`/`v4.6.2` at `2026-03-18T15:03:19Z`. Padded to **2 full days**
(`2026-03-20T00:00:00`), not 1 — per the Gleif migration's finding that `GitVersion.Tool 5.9.0`
appears to parse `commits-before` as local machine time, and a 1-day margin can silently fail to
exclude the tag (no error, it just keeps incrementing off the old version).

Verified with the pinned `GitVersion.Tool 5.9.0` (installed to a scratch tool-path, not the
globally-installed version, which fails on this repo's `pull-request: tag: pr` config with a
schema-mismatch error — same issue Dataverse.V2 hit): resolves to `MajorMinorPatch: "1.0.0"`,
confirmed correct.

---

## Step 7 — Push and confirm CI

Status: **Pending**

---

## Checklist

- [x] `azure-pipelines.yml` — switched to `crawler.build.jobs.yml` with `multiVersionCluedInTargets` (4.7.0, 4.8.0, 5.0.0-beta.*); `useGitVersionDotNetTool: true` set explicitly
- [x] `NuGet.config` — renamed from `Nuget.config`; feeds confirmed sufficient for 4.7.0/4.8.0 (no `public` feed needed)
- [x] `Directory.Build.props` — honours `CluedInMultiVersionTargetFramework`; `DefineConstants`/`_CluedInPackageSuffix` derived; `LangVersion` pinned to 13.0
- [x] `Packages.props` — `_CluedIn` guarded; test-package versions split by `CLUEDIN_V50`; `CluedIn.Testing.Base` switched to suffixed package ID
- [x] Test project — conditional xunit v2/v3 + AutoFixture selection; `executeIntegrationTests` enabled (was previously compiled-but-not-run; new template would otherwise stop compiling it at all)
- [x] Source — `#if CLUEDIN_V50` guards for the RestSharp 106↔114 break (constructor, `Method`, response types in both `CompanyHouseClient.cs` and `CompanyHouseExternalSearchProvider.cs`); `Parameter.Value`/`HeaderParameter.Value` normalized with `.ToString()` instead of a guard
- [x] `src/` and the integration test — verified locally for all three legs via real `dotnet build` **and** `dotnet test` (not just build)
- [x] `GitVersion.yml` — merged `commits-before` into the existing `ignore:` block (duplicate-key trap); `next-version: 1.0`; `commits-before: 2026-03-20T00:00:00` (2-day padding); verified `MajorMinorPatch: 1.0.0` with the pinned GitVersion.Tool 5.9.0
- [ ] Push branch and confirm the actual Azure DevOps pipeline run is green end-to-end (all three legs + integration tests + `Multi-version: publish`)
