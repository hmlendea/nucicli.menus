# Component: test projects

Two test projects exist. Both target `net10.0`, disable implicit usings, enable nullable, and set `IsPackable=false`, `IsTestProject=true`, `ExcludeFromCodeCoverage=true`.

## `NuciCLI.Menus.UnitTests`

References the production project directly. Packages: `Microsoft.NET.Test.Sdk` 18.12.0, `Moq` 4.21.0, `NUnit` 5.0.0, `NUnit3TestAdapter` 6.3.1.

| File | Fixture | Verified behaviour |
|------|---------|--------------------|
| `MenuTests.cs` | `MenuTests` | Title assignment, defaults (`"-==< "`, `"> "`, Green/Yellow/White, not disposed), non-empty `Id`, adding a command, duplicate-name rejection, disposal marks `IsDisposed` and raises `Disposed` once, repeated dispose raises once, all settable properties round-trip. |
| `MenuCommandRegistrationTests.cs` | `MenuCommandRegistrationTests` | Built-in `exit`/`help` names cannot be re-registered; arbitrary names including empty/whitespace/unicode are accepted once and rejected twice; distinct casing accepted (`EXIT`, `Help`, `Run`); null name ⇒ `ArgumentNullException`; any description (including null) accepted; null action still reserves the name; disposal does not invoke actions. |
| `MenuManagerTests.cs` | `MenuManagerTests` | `Instance` is a stable singleton; a fresh manager has no active menu and is not running; closing unknown/empty/whitespace id ⇒ `KeyNotFoundException` with no state change; `CloseMenu()` with no active menu ⇒ `ArgumentNullException`; `CloseMenu(null)` ⇒ `ArgumentNullException`; `SwitchToMenu(null)` ⇒ `NullReferenceException`; unconstructible types ⇒ exception with no state change; incompatible constructor arguments ⇒ `MissingMethodException` (both generic and reflection overloads). |
| `MenuManagerLifecycleTests.cs` | `MenuManagerLifecycleTests` | Startup failure paths: invalid constructor arguments raise only `Starting`; a throwing `Starting` handler's original exception escapes unwrapped; a `Starting` handler observes `IsRunning == false` and `ActiveMenuId == null`. |
| `ProtectedDisposalMenu.cs` | — | Helper subclass exposing `Dispose(bool)` as `ReleaseResources(bool)`. |

### Test conventions observed

- One assertion concept per test; names follow `Given…_When…_Then…`.
- `Moq` is used only for `EventHandler` and `Action` to assert invocation counts; no production type is mocked (none is mockable: statics and internal types).
- Tests use `new MenuManager()` rather than the singleton, which keeps registry state isolated per test.
- `MenuTests` and `MenuCommandRegistrationTests` dispose in `TearDown`.
- Test fixture data (`"Dark Souls"`, `"Age of Empires"`) is used for constructor-argument scenarios.

### Coverage boundaries

Not exercised: `MenuManager.Start` loop body, `TakeCommand`, `MenuPrinter` (all methods), `Command.Execute`, `CommandResult.Status`, `OpenMenu` success paths, `SwitchToMenu` success path, `ActiveMenuChanged` on real transitions, `Menu.Exit`, `HandleHelp`.

Reason: all of these reach `NuciConsole` statics, which are not abstracted, so no seam exists for substitution.

## `NuciCLI.Menus.ArchitectureTests`

References the production project. Packages: `ArchUnitNET` 2.4.0-alpha.1, `Microsoft.CodeAnalysis.CSharp` 5.12.0, `Microsoft.NET.Test.Sdk` 18.12.0, `NUnit` 5.0.0, `NUnit3TestAdapter` 6.3.1.

| File | Test | Rule enforced |
|------|------|---------------|
| `ProductionArchitectureTests.cs` | `GivenProductionProject_WhenCheckingForCycles_ThenNoCyclesExist` | `ArchUnit.ProjectFiles(NuciCLI.Menus.csproj).InPath("**/*.cs").Should().HaveNoCycles()` |
| | `…AllFilesAreUnderTheProductionProject` | The project-file selector resolves only production sources (no tests, no generated code). |
| | `…OnlyDocumentedDependenciesAreReferenced` | Referenced assemblies excluding `System.*` must be a subset of `{ System.Runtime, NuciCLI, NuciExtensions }`. |
| | `…AllExpectedPublicComponentsExist` | `Menu` and `MenuManager` types are loaded from the production assembly. |
| `SourceFileConventionTests.cs` | `…SourcesAreAvailable` | Production `.cs` files are discoverable excluding `bin`/`obj`. |
| | `…EachFileHasOneMatchingType` | Exactly one top-level type per file; identifier equals filename. |
| | `…RequiredPoliciesAreEnabled` | `.csproj` contains `net10.0`, `ImplicitUsings disable`, `LangVersion latest`, `Nullable enable`. |
| | `…ExplicitBlockNamespacesAreUsed` | Exactly one `namespace X { }` block per file; no file-scoped namespaces. |

### Implementation notes

- Project paths are resolved relative to `TestContext.CurrentContext.TestDirectory` via `../../../../`, so moving test binaries changes the resolution.
- The ArchUnit rules are invoked through reflection (`CheckAsync`) because the fluent rule object's static type is not exposed by the alpha package; `CheckRuleAsync` asserts the presence of `CheckAsync` and a `Result` property.
- `HaveNoCycles` and `BeInPath` violations produce empty collections on success.

### Consequences for modification

| Change | Blocked by |
|--------|-----------|
| Adding a second public type to an existing file | `SourceFileConventionTests` |
| Using a file-scoped namespace | `SourceFileConventionTests` |
| Adding any new package reference to the library | `ProductionArchitectureTests` assembly-reference subset check (unless the test list is updated) |
| Enabling implicit usings or removing nullable | `SourceFileConventionTests` |
| Introducing a namespace/file dependency cycle | `ProductionArchitectureTests` |

Note the assembly-reference check asserts `Is.SubsetOf(allowedReferences)`, so it fails when a **new** reference appears; it does not fail when a listed reference stops being used. The list must be extended deliberately when adding a dependency.