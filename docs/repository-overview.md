# Repository overview

## Purpose

`NuciCLI.Menus` supplies a hierarchical, command-driven interactive menu system for console applications that already depend on `NuciCLI`. It exists so that a host console application does not need to re-implement prompt reading, command lookup, menu-to-menu navigation, help listing, and execution statistics printing.

## What the library owns

| Concern | Implementation |
|---------|----------------|
| Menu identity and parent link | `Menu.Id`, `Menu.ParentId` |
| Menu presentation settings | `Menu.Title`, `TitleDecoration`, `Prompt`, three `NuciConsoleColour` properties |
| Command registration and lookup | `Menu.Commands` (`Dictionary<string, Command>`), `Menu.AddCommand` |
| Menu registry and transitions | `MenuManager.menus`, `OpenMenu`, `CloseMenu`, `SwitchToMenu` |
| Synchronous input loop | `MenuManager.Start<TMenu>`, `MenuManager.TakeCommand` |
| Lifecycle notifications | `Menu.Created`, `Menu.Disposed`, `MenuManager.Starting`, `Started`, `Stopped`, `ActiveMenuChanged` |
| Execution timing and status | `Command.Execute`, `CommandResult`, `CommandStatus` |
| Console rendering | `MenuPrinter` |
| Built-in commands | `exit`, `help` registered in the `Menu` constructor |

## What the library deliberately does not own

- Persistence of any kind (no files, database, cache, or user settings).
- Networking, authentication, authorisation, or process supervision.
- Logging, telemetry, diagnostics, or crash reporting.
- Command argument parsing. Commands are zero-argument `Action` delegates; tokenisation is the host's responsibility.
- Asynchronous or parallel command execution.
- Abstract console abstraction: `NuciConsole` static calls are used directly.
- Menu visual layout beyond title, decoration, prompt, and command table.

## Entry points

| Entry point | Signature | Purpose |
|-------------|-----------|---------|
| `NuciCLI.Menus.MenuManager.Instance` | `static MenuManager` | Process-wide manager used by `Menu.Exit`. |
| `NuciCLI.Menus.MenuManager.Start<TMenu>()` | `void` | Runs the blocking menu loop with a root menu. |
| `NuciCLI.Menus.MenuManager.Start<TMenu>(params object[])` | `void` | Same, with constructor arguments. |
| `NuciCLI.Menus.Menu` (constructors) | `Menu()`, `Menu(string title)` | Base class for host menu types. |
| `NuciCLI.Menus.MenuManager.OpenMenu` | four overloads | Opens a child menu without running a loop. |

## Consumers

Only the host console application. The library has no server, endpoint, or scheduled process, so there is no runtime topology beyond "one library inside one process".

## Verification summary

- `NuciCLI.Menus.UnitTests` exercises menu defaults, disposal, command registration boundaries, manager state guards, and startup event ordering.
- `NuciCLI.Menus.ArchitectureTests` enforces source-file conventions (one type per file, explicit block namespaces, filename/type match), build-policy properties, absence of dependency cycles, and the allowed assembly reference set.

See [`testing.md`](./testing.md) for detail.