# Design decisions

Each decision below is recorded from the implementation and corroborating artefacts (tests, workflows, root documents). Rationale is stated only where evidence exists; inferred rationale is labelled.

## D1 — Synchronous blocking loop instead of an event pump

**Decision:** `MenuManager.Start` blocks on `while (IsRunning) TakeCommand()`.

**Evidence:** `MenuManager.Start`; no `Task`, `async`, or threading primitive in production code.

**Consequences:** simple mental model, deterministic ordering, no console re-entrancy; hostile to asynchronous hosts and to testability (no seam).

**Inferred rationale:** a console application has one interactive thread; asynchronous dispatch would add complexity without benefit.

## D2 — Concrete public types with internal interfaces

**Decision:** `Menu` and `MenuManager` are public and declared against `internal` interfaces `IMenu`/`IMenuManager`.

**Evidence:** `internal interface IMenu`, `public class Menu : IDisposable, IMenu`; no production code accepts the interface as a parameter; no test uses them.

**Consequences:** interfaces serve as member contracts and compile-time completeness checks only; consumers cannot substitute implementations.

## D3 — Reflection-based menu construction

**Decision:** `Activator.CreateInstance(menuType, parameters)`.

**Evidence:** `MenuManager.OpenMenu(Type, params object[])`; tests assert `MissingMethodException` for mismatched arguments.

**Consequences:** menus need runtime-discoverable constructors; construction failures surface as activator exceptions; no factory abstraction.

## D4 — Exact, case-sensitive command names in a `Dictionary`

**Decision:** `Dictionary<string, Command>` keyed by `command.Name` with default ordinal comparer.

**Evidence:** `Menu.Commands`, `Menu.AddCommand`, `Commands.TryGetValue`; `MenuCommandRegistrationTests` casing tests.

**Consequences:** no aliases or case folding; duplicate names rejected by `Dictionary.Add`; help ordering follows enumeration order.

## D5 — Built-in commands registered in the constructor

**Decision:** `exit` and `help` are added inside `Menu()` rather than by the manager.

**Evidence:** `Menu()` constructor; duplicate-registration tests for both names.

**Consequences:** every menu, including child and root, has them; a derived type cannot omit them; `help` always includes itself in the listing.

## D6 — Statistics are opt-in and printed by the manager

**Decision:** `AreStatisticsEnabled` defaults to `false`; the manager prints after execution.

**Evidence:** `MenuManager.AreStatisticsEnabled`, `TakeCommand`; `MenuPrinter.PrintCommandResults` is reachable only from the manager.

**Consequences:** zero output cost by default; commands cannot inject their own result rendering.

## D7 — Cancellation captured, other exceptions rethrown

**Decision:** `Command.Execute` catches only `InputCancellationException`.

**Evidence:** `Command.cs` catch clause; `CommandResult.Status` mapping; unit tests assert other startup exceptions escape unwrapped.

**Consequences:** user cancellation is distinguishable in statistics; host failures are never silently swallowed. Side effect: `CommandStatus.Failure` is unreachable.

## D8 — Dispose pattern with an inert finaliser

**Decision:** `Menu` implements `IDisposable`, a protected virtual `Dispose(bool)`, and `~Menu()`.

**Evidence:** `Menu.cs`; `ProtectedDisposalMenu` helper exists to expose `Dispose(bool)`.

**Consequences:** derived types can hook disposal; the finaliser performs no work because `Dispose(false)` returns early, so undisposed menus never raise `Disposed`.

## D9 — Manager owns menu lifetime; `Menu.Exit` delegates to the singleton

**Decision:** closing is centralised in `MenuManager.CloseMenu`; `Menu.Exit` calls `MenuManager.Instance`, not a stored reference.

**Evidence:** `Menu.Exit`; `MenuManager.CloseMenu` removes before disposing.

**Consequences:** disposal ordering and registry consistency are guaranteed for singleton-managed menus; manually constructed managers bypass `exit` semantics.

## D10 — Static console coupling

**Decision:** `NuciConsole` statics are called directly from `MenuManager` and `MenuPrinter`, with no interface.

**Evidence:** all console call sites; absence of any I/O abstraction; unit tests deliberately avoid the loop.

**Consequences:** the library is untestable past construction without process-wide console redirection; the architecture test whitelists only `NuciCLI` and `NuciExtensions`.

## D11 — Architecture tests as policy enforcement

**Decision:** structure is verified by tests, not only by review: one type per file, explicit block namespaces, build properties, no cycles, reference whitelist.

**Evidence:** `SourceFileConventionTests`, `ProductionArchitectureTests`.

**Consequences:** structural changes require test updates; conventions cannot regress silently.

## D12 — Package targeting `net10.0` with `LangVersion latest`

**Decision:** single target framework, latest language version, nullable enabled, implicit usings disabled.

**Evidence:** `NuciCLI.Menus.csproj` plus the `.csproj` assertions in `SourceFileConventionTests`.

**Consequences:** primary constructors, collection expressions (`Commands = []`), and `Lock` are available; consumers must be on .NET 10.

## D13 — OIDC trusted publishing for NuGet

**Decision:** the NuGet release workflow exchanges an OIDC token for a short-lived API key.

**Evidence:** `nuget-release.yml` uses `NuGet/login@v1` with `id-token: write` and `secrets.NUGET_USER`.

**Consequences:** no long-lived NuGet API key stored in repository secrets; requires the trusted publisher to be configured on nuget.org.