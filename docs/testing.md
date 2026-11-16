# Testing

## Strategy

Three layers of automated verification, all NUnit-based, all run by a single `dotnet test`:

| Layer | Project | Purpose |
|-------|---------|---------|
| Behavioural unit tests | `NuciCLI.Menus.UnitTests` | Public API semantics and boundary conditions |
| Architecture/policy tests | `NuciCLI.Menus.ArchitectureTests` | Structural conventions and dependency rules |
| Compilation | `dotnet build` (CI) | Type safety, nullable analysis, warnings |

Existing approach is edge-focused: most tests assert exception types and "no side effect" outcomes rather than happy-path console output. This is a consequence of the console being a static, unmockable dependency.

## Command

```bash
dotnet test NuciCLI.Menus.slnx
```

or from CI:

```bash
dotnet restore
dotnet build --no-restore
dotnet test --no-build --verbosity normal
```

Run a single project:

```bash
dotnet test NuciCLI.Menus.UnitTests/NuciCLI.Menus.UnitTests.csproj
dotnet test NuciCLI.Menus.ArchitectureTests/NuciCLI.Menus.ArchitectureTests.csproj
```

## Unit-test inventory

| Fixture | Test count | Coverage focus |
|---------|-----------|----------------|
| `MenuTests` | 8 | Defaults, title, id, command add, duplicate rejection, disposal once, property round-trip |
| `MenuCommandRegistrationTests` | 7 (parameterised) | Built-in names, duplicate registration with varied names, casing, null name, descriptions, null action, disposal |
| `MenuManagerTests` | 12 (parameterised) | Singleton identity, initial state, unknown ids, null ids, unconstructible types, constructor mismatch |
| `MenuManagerLifecycleTests` | 5 | Startup failure ordering and exception transparency |

Fixtures create `new MenuManager()` rather than the singleton, so registry state is isolated per test.

## Test conventions

- Names: `Given<context>_When<action>_Then<outcome>`.
- `SetUp` initialises fields with `null!`; `TearDown` disposes menus.
- `Moq` mocks are restricted to `EventHandler` and `Action`.
- Assertions use NUnit constraint syntax (`Assert.That(…, Is/Throws/Has…)`).
- Parameterised data favours `TestCase` attributes with representative strings (`"Dark Souls"`, `"Age of Empires"`, `"run"`, `"EXIT"`, `"șterge"`).

## Architecture tests

| Test | Mechanism |
|------|-----------|
| No dependency cycles | `ArchUnit.ProjectFiles(csproj).InPath("**/*.cs").Should().HaveNoCycles()` invoked reflectively |
| Production scope | `BeInPath("**/*.cs")` over the project-file selector |
| Allowed references | Reflection over `Assembly.GetReferencedAssemblies()`, filtering `System.*`, subset of `{System.Runtime, NuciCLI, NuciExtensions}` |
| Expected public types | `Menu` and `MenuManager` present in the assembly |
| One type per file, name match | Roslyn `CompilationUnitSyntax` walk |
| Explicit block namespace | At most one `NamespaceDeclarationSyntax`, zero `FileScopedNamespaceDeclarationSyntax` |
| Build policy properties | String containment checks on `NuciCLI.Menus.csproj` |

## Coverage gaps

| Area | Reason not covered |
|------|--------------------|
| `MenuManager.Start` loop | Reaches `NuciConsole` statics |
| `MenuManager.TakeCommand` | Same |
| `MenuPrinter` (all methods) | Static class, writes to console |
| `Command.Execute` / `CommandResult.Status` | `internal`; reachable only through the loop or reflection |
| `OpenMenu`/`CloseMenu`/`SwitchToMenu` success paths | Require console output |
| `Menu.Created` event | Not asserted anywhere |
| `HandleHelp` | Console output |
| `ProtectedDisposalMenu` | Declared but never used by a test |

See [`ambiguities-and-open-questions.md`](./ambiguities-and-open-questions.md) for the `ProtectedDisposalMenu` observation.

## Adding tests safely

1. Match the existing naming and fixture layout.
2. Dispose any `Menu` in `TearDown`.
3. Prefer `new MenuManager()` for state isolation.
4. Do not introduce a mocking framework beyond `Moq`, and do not mock `NuciConsole` (it is static).
5. If a test needs console capture, note that `Console.SetOut` affects the process-global writer and `NuciCLI` may cache its own writer; verify before relying on it.
6. Any new production type needs its own file named after the type or `SourceFileConventionTests` fails.
7. Any new package reference to the library requires updating `allowedReferences` in `ProductionArchitectureTests`.