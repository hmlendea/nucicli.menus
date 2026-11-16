# Invariants

Invariants below are verified against the implementation and, where indicated, by tests. Any change that violates one is a behavioural break even if no test fails.

## Constructor invariants

| ID | Invariant | Evidence |
|----|-----------|----------|
| I1 | Every constructed `Menu` has `exit` and `help` registered before its constructor body completes | `Menu()` body; `MenuCommandRegistrationTests` duplicate registration of both names |
| I2 | `Menu.Id` is a non-empty GUID string unless a consumer overwrites it | `MenuTests.GivenAConstructedMenu_WhenReadingItsIdentifier_ThenItIsNotEmpty` |
| I3 | Defaults are `TitleDecoration = "-==< "`, `Prompt = "> "`, colours Green/Yellow/White, `IsDisposed == false` | `MenuTests.…ThenTheExpectedDefaultsAreReturned` |
| I4 | `Menu.Created` is raised synchronously within the constructor, before a derived body runs | `Menu.cs` ordering |

## Registration invariants

| ID | Invariant | Evidence |
|----|-----------|----------|
| I5 | Command names are exact and case-sensitive; duplicates raise `ArgumentException` | `Menu.AddCommand`; `MenuCommandRegistrationTests` casing and duplicate tests |
| I6 | Registering a command never invokes its action | `MenuCommandRegistrationTests.…ThenTheActionIsNotExecuted` |
| I7 | A `null` action still reserves the name | `GivenANullAction_WhenRegisteringACommand_ThenItsNameIsReserved` |

## Lifecycle invariants

| ID | Invariant | Evidence |
|----|-----------|----------|
| I8 | `Dispose(bool)` sets `IsDisposed` at most once and raises `Disposed` at most once | `MenuTests` double-dispose test |
| I9 | Disposal clears `Commands` and never invokes command actions | `MenuCommandRegistrationTests.…ThenActionsAreNotExecuted` |
| I10 | `Starting` precedes menu construction; `Started` follows `IsRunning = true`; `Stopped` follows normal loop exit | `MenuManager.Start`; lifecycle tests for the failure side |
| I11 | A `Starting` handler that throws prevents construction, registration, and all later events | `MenuManagerLifecycleTests` |

## Navigation invariants

| ID | Invariant | Evidence |
|----|-----------|----------|
| I12 | Opening a menu while another is active assigns `ParentId = ActiveMenuId` | `MenuManager.OpenMenu` |
| I13 | Closing a menu disposes it and re-activates its parent when `ParentId` is non-empty | `CloseMenu` |
| I14 | Closing a menu with no parent clears `IsRunning` and therefore ends the loop | `CloseMenu` |
| I15 | `SwitchToMenu` to the identical id is a no-op (no header reprint, no event) | `SwitchToMenu` early return |
| I16 | Menu construction failure leaves the registry, `ActiveMenuId`, and events untouched | `MenuManagerTests` unconstructible-type and `MissingMethodException` tests |

## Execution invariants

| ID | Invariant | Evidence |
|----|-----------|----------|
| I17 | Command resolution is exact-match against the active menu's dictionary only | `TakeCommand` |
| I18 | `InputCancellationException` from input never terminates the loop | `TakeCommand` catch |
| I19 | `InputCancellationException` from an action yields `CommandStatus.Cancelled` | `Command.Execute` + `CommandResult.Status` |
| I20 | Statistics are printed only when `AreStatisticsEnabled` is true, and only after the action completes | `TakeCommand` |
| I21 | All other action exceptions escape to the host | `Command.Execute` catch filter |

## Structural invariants (enforced by architecture tests)

| ID | Invariant | Evidence |
|----|-----------|----------|
| I22 | Exactly one top-level type per production file, named after the file | `SourceFileConventionTests` |
| I23 | Exactly one explicit block namespace per file; no file-scoped namespaces | `SourceFileConventionTests` |
| I24 | Production `.csproj` keeps `net10.0`, `ImplicitUsings disable`, `LangVersion latest`, `Nullable enable` | `SourceFileConventionTests` |
| I25 | Production assembly references only `System.Runtime`, `NuciCLI`, `NuciExtensions` | `ProductionArchitectureTests` |
| I26 | Production sources contain no dependency cycles | `ProductionArchitectureTests` |
| I27 | `Menu` and `MenuManager` are public types of the production assembly | `ProductionArchitectureTests` |

## Non-invariants (commonly assumed but false)

| Assumption | Reality |
|------------|---------|
| "Dictionary order is guaranteed" | Enumeration order is insertion order in practice but not contractually guaranteed; the help listing depends on it visually. |
| "A closed menu's children are also closed" | Not implemented; children keep stale `ParentId` values. |
| "The manager is reusable after `Start` returns" | Stale `ActiveMenuId` makes a second `Start` behave incorrectly. |
| "Command failure is reflected in statistics" | `Failure` is unreachable; exceptions escape instead. |
| "`Menu.Exit` closes the menu on the creating manager" | It always resolves the static singleton. |