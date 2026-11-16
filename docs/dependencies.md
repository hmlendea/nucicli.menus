# Dependencies

## Runtime dependencies (`NuciCLI.Menus`)

| Package | Version | Purpose | Used by |
|---------|---------|---------|---------|
| `NuciCLI` | 3.0.3 | Console input/output/colour and `InputCancellationException` | `MenuManager`, `MenuPrinter`, `Command`, `CommandResult`, `Menu`, `IMenu` |
| `NuciExtensions` | 6.0.3 | `string.Reverse()` for the mirrored title decoration | `MenuPrinter.PrintTitle` |

Target framework: `net10.0`. `System.Threading.Lock` (used for the singleton) is provided by the runtime, not a package.

## Unit-test dependencies (`NuciCLI.Menus.UnitTests`)

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.NET.Test.Sdk` | 18.12.0 | Test host and runner infrastructure |
| `Moq` | 4.21.0 | Verifying `EventHandler`/`Action` invocation counts |
| `NUnit` | 5.0.0 | Test framework |
| `NUnit3TestAdapter` | 6.3.1 | VSTest adapter for NUnit |

## Architecture-test dependencies (`NuciCLI.Menus.ArchitectureTests`)

| Package | Version | Purpose |
|---------|---------|---------|
| `ArchUnitNET` | 2.4.0-alpha.1 | Dependency-cycle and path rules over project sources |
| `Microsoft.CodeAnalysis.CSharp` | 5.12.0 | Roslyn parsing for source-file convention rules |
| `Microsoft.NET.Test.Sdk` | 18.12.0 | Test host |
| `NUnit` | 5.0.0 | Test framework |
| `NUnit3TestAdapter` | 6.3.1 | VSTest adapter |

`ArchUnitNET` is a pre-release version; upgrading it may change the fluent rule API used reflectively by `CheckRuleAsync`.

## Transitive considerations

- `NuciCLI` itself depends on console facilities but is consumed only through its public static surface.
- `ArchUnitNET` and `Microsoft.CodeAnalysis.CSharp` are test-only; they never ship in the NuGet package (`IsPackable=false`).
- The architecture test forbids new production assembly references unless `allowedReferences` in `ProductionArchitectureTests` is extended.

## Version consistency observations

| Source | `NuciCLI` | `NuciExtensions` | Package version |
|--------|-----------|------------------|-----------------|
| `NuciCLI.Menus.csproj` | 3.0.3 | 6.0.3 | 1.5.8 |
| `README.md` | 3.0.3 | 6.0.3 | — |
| `ARCHITECTURE.md` | 3.0.1 | 5.3.1 | — |
| `PRIVACY.md` | 3.0.1 | 5.3.1 | — |

`NuciCLI.Menus.csproj` is authoritative for the shipped package; the two root documents are stale. See [`ambiguities-and-open-questions.md`](./ambiguities-and-open-questions.md).

## Packaging metadata

From `NuciCLI.Menus.csproj`: `PackageLicenseExpression = GPL-3.0-or-later`, tags `Console Terminal CommandLine CLI`, `GeneratePackageOnBuild=false`, `IsPackable=true`.