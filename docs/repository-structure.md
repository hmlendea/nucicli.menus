# Repository structure

## Directory layout

```
nucicli.menus/
├── .github/
│   ├── FUNDING.yml
│   └── workflows/
│       ├── dotnet.yml              # build + test on push/PR to master
│       ├── security.yml            # vulnerability audit + CodeQL
│       ├── github-release.yml      # pack, checksum, upload to GitHub release
│       └── nuget-release.yml       # publish to NuGet.org (OIDC trusted publishing)
├── NuciCLI.Menus/                  # production library (packable)
│   ├── Command.cs
│   ├── CommandResult.cs
│   ├── CommandStatus.cs
│   ├── IMenu.cs
│   ├── IMenuManager.cs
│   ├── Menu.cs
│   ├── MenuManager.cs
│   ├── MenuPrinter.cs
│   └── NuciCLI.Menus.csproj
├── NuciCLI.Menus.UnitTests/        # NUnit + Moq behaviour tests
├── NuciCLI.Menus.ArchitectureTests/ # ArchUnitNET + Roslyn policy tests
├── NuciCLI.Menus.slnx              # solution (three projects)
├── ARCHITECTURE.md
├── CONTRIBUTING.md
├── CODE_OF_CONDUCT.md
├── PRIVACY.md
├── SECURITY.md
├── README.md
└── LICENSE
```

`bin/` and `obj/` appear at repository root and inside each project; they are build artefacts and are not source.

## Production source files

| File | Declared type | Accessibility | Responsibility |
|------|---------------|---------------|----------------|
| `Menu.cs` | `class Menu : IDisposable, IMenu` | `public` | Menu identity, presentation settings, command dictionary, `exit`/`help` registration, disposal. |
| `MenuManager.cs` | `class MenuManager : IMenuManager` | `public` | Singleton, menu registry, opening/closing/switching, input loop, lifecycle events. |
| `IMenu.cs` | `interface IMenu` | `internal` | Internal contract implemented by `Menu`. |
| `IMenuManager.cs` | `interface IMenuManager` | `internal` | Internal contract implemented by `MenuManager`. |
| `Command.cs` | `sealed class Command` (primary constructor) | `internal sealed` | Holds `Name`, `Description`, `Action`; `Execute()` timing and cancellation capture. |
| `CommandResult.cs` | `sealed class CommandResult` (primary constructor) | `internal sealed` | Start/end time, `Duration`, `Exception`, derived `Status`. |
| `CommandStatus.cs` | `enum CommandStatus` | `internal` | `Success`, `Failure`, `Cancelled`. |
| `MenuPrinter.cs` | `static class MenuPrinter` | `internal static` | Header, title, command list, and statistics rendering. |

Naming rule enforced by `SourceFileConventionTests`: exactly one top-level type per file, and its identifier must equal the filename without extension. Adding a new type therefore requires a new file named after that type.

## Test project contents

| Project | Files | Focus |
|---------|-------|-------|
| `NuciCLI.Menus.UnitTests` | `MenuTests.cs`, `MenuCommandRegistrationTests.cs`, `MenuManagerTests.cs`, `MenuManagerLifecycleTests.cs`, `ProtectedDisposalMenu.cs` | Behaviour and boundary conditions of the public API. |
| `NuciCLI.Menus.ArchitectureTests` | `ProductionArchitectureTests.cs`, `SourceFileConventionTests.cs` | Structural and dependency policies. |

`ProtectedDisposalMenu.cs` is a test helper exposing `Menu.Dispose(bool)`; it is currently not referenced by any test (see [`ambiguities-and-open-questions.md`](./ambiguities-and-open-questions.md)).

## Solution definition

`NuciCLI.Menus.slnx` is an XML solution file listing the three projects. `dotnet build`/`dotnet test` against the solution file builds and tests all three.

## Related documentation

- Component-level detail: [`components/`](./components/)
- Elaborated architecture: [`architecture.md`](./architecture.md)