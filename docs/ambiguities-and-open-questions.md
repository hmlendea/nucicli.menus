# Ambiguities and open questions

Items below are observed discrepancies or undocumented assumptions. Each records the evidence and the question a future contributor must answer before relying on either side.

## A1 — Documented dependency versions are stale

| Source | `NuciCLI` | `NuciExtensions` |
|--------|-----------|------------------|
| `NuciCLI.Menus/NuciCLI.Menus.csproj` (authoritative) | 3.0.4 | 6.0.3 |
| `README.md` | 3.0.4 | 6.0.3 |
| `ARCHITECTURE.md` | 3.0.1 | 5.3.1 |
| `PRIVACY.md` | 3.0.1 | 5.3.1 |

Question: should the root documents be updated to 3.0.4 / 6.0.3, and are the older statements describing an intentional support floor?

## A2 — `CommandStatus.Failure` is unreachable

`Command.Execute` rethrows all non-cancellation exceptions, so no `CommandResult` ever carries a non-cancellation exception. `MenuPrinter` implements the red `Failed` branch and `Error message:` line regardless.

Question: is the intended design to (a) catch broadly and record the exception as `Failure`, or (b) keep rethrowing and treat the failure branch as dead code that should be removed?

## A3 — `CommandResult.Exception` has no initialiser

The two-argument primary constructor leaves `Exception` null implicitly. This is legal because the property is `Exception?`, but it relies on default-initialisation of a `{ get; }` auto-property.

Question: intended, or should it be `= null;` explicitly for clarity?

## A4 — `ProtectedDisposalMenu` is unused

`NuciCLI.Menus.UnitTests/ProtectedDisposalMenu.cs` exposes `ReleaseResources(bool)` to reach `Menu.Dispose(bool)`. No test references it. Either a disposal-hook test was removed, or a test was intended. The library's own `Dispose(bool)` behaviour with `disposing == false` is therefore unverified.

Question: should a test exercising `ReleaseResources(false)` be restored?

## A5 — `Menu.Created` fires before `Title` is assigned and before derived constructor bodies

Consequences: handlers see `null` title values and cannot observe derived registrations. No test asserts `Created` at all.

Question: is the current ordering intended (constructor-time event) or should `Created` be raised later, e.g. on first activation?

## A6 — `MenuManager` retains state between `Start` invocations

`ActiveMenuId` is never reset. A second `Start` on the same instance:
- prints a spurious separator line because a stale active id exists;
- gives the new root a `ParentId` pointing at a removed menu;
- makes closing that root throw `KeyNotFoundException` instead of stopping the loop.

Question: is single-run usage an intentional constraint, or should `Start` reset `ActiveMenuId`/`IsRunning`?

## A7 — `null` return from `NuciConsole.ReadLine`

`TakeCommand` passes `cmd` directly to `Dictionary.TryGetValue`, which throws `ArgumentNullException` for `null`. Whether `ReadLine` can return `null` depends on the `NuciCLI` contract, which is not represented in this repository.

Question: should `TakeCommand` guard against `null` explicitly?

## A8 — Closing a non-active menu re-activates that menu's parent

`CloseMenu(menuId)` switches to the closed menu's parent regardless of the currently active menu, so the active menu can jump levels unexpectedly.

Question: intended (supporting programmatic cleanup) or an oversight?

## A9 — `SwitchToMenu` corrupts state for unknown ids

`ActiveMenuId` is assigned before `PrintMenuHeader` looks up the menu, so an unknown id leaves `ActiveMenuId` pointing at a nonexistent entry and throws `KeyNotFoundException`.

Question: should validation happen before assignment?

## A10 — No test covers `OpenMenu`/`CloseMenu`/`SwitchToMenu` success paths

Only failure and guard paths are asserted. Header printing, parent assignment, registry mutation, and `ActiveMenuChanged` firing are unverified.

Question: should a console seam be introduced to enable coverage, or is the current boundary accepted?

## A11 — `SwitchToMenu` equality check uses the argument

`menuId.Equals(ActiveMenuId)` throws `NullReferenceException` for a `null` argument, and is inconsistent with the rest of the codebase's `string.IsNullOrWhiteSpace` style.

Question: intended guard style?

## A12 — `MenuManager` lifecycle events are public fields

`Starting`, `Started`, `Stopped`, and `ActiveMenuChanged` are fields, so consumers can invoke or replace them. They are also absent from `IMenuManager`.

Question: intended (simple hook surface) or should they be `event` accessors and interface members?

## A13 — Help ordering depends on dictionary enumeration

`PrintCommandList` iterates `commands.Values`. .NET preserves insertion order in practice for the current implementation but does not guarantee it. No test asserts output order.

Question: should ordering become an explicit contract (e.g. by sorting) or remain incidental?

## A14 — Duration precision

`DateTime.Now` is low resolution and not monotonic; sub-millisecond commands render as `0.00s`, and clock changes can produce negative durations that are printed as negative numbers.

Question: should `Stopwatch` and clamped durations be used instead?

## A15 — Workflow action versions are inconsistent

`.github/workflows/dotnet.yml` uses `actions/checkout@v2` and `actions/setup-dotnet@v1`; other workflows use v4–v7. CodeQL uses `@v4`.

Question: are the older pins intentional, or should they be brought in line?

## A16 — Vulnerability scanning does not gate the build

`security.yml` runs `dotnet list package --vulnerable --include-transitive`, whose findings appear in the job log but do not fail the workflow. Only CodeQL results reach Code Scanning.

Question: should the audit step fail the build (`--vulnerable` returns non-zero in some CLI versions depending on flags), or should Dependabot be enabled instead?

## A17 — Architecture test resolves paths via `../../../../`

Both architecture test fixtures derive repository paths from `TestContext.CurrentContext.TestDirectory`, which breaks if the output layout changes (e.g. custom `OutputPath`, packing, or single-file publish of tests).

Question: should paths be resolved from an MSBuild-provided property instead?

## A18 — `Name`/`Description` nullability mismatch

`Menu.AddCommand` accepts `string description` and passes it to `Command`; the interface and implementation are non-nullable annotated, yet tests pass `null!` and no null check exists. `MenuPrinter` interpolates the value (rendering empty).

Question: is a null description supported (as tests imply) or should it be validated?