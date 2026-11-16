# Integrations

The library integrates with exactly two external packages, both in-process. It has no service integrations.

## `NuciCLI` 3.0.3

| Aspect | Detail |
|--------|--------|
| Boundary | Static `NuciConsole` class and `NuciConsoleColour` enumeration plus `InputCancellationException` |
| Direction | Outbound calls from the library; exceptions inbound to the library |
| Used by | `MenuManager` (input, cursor, unknown-command message), `MenuPrinter` (all rendering), `Command`/`CommandResult` (exception type) |
| Members consumed | `NuciConsole.ReadLine(string, NuciConsoleColour)`, `WriteLine()`, `WriteLine(string)`, `WriteLine(string, NuciConsoleColour)`, `Write(string, NuciConsoleColour)`, `CursorY`, `NuciConsoleColour.{Green, Yellow, White, Red, Blue, Cyan}` for defaults/tests, `InputCancellationException` |
| Configuration | None |
| Failure semantics | Console exceptions propagate; `InputCancellationException` is interpreted as user cancellation |

Verified call sites: `MenuManager.cs` lines ~139–221, `MenuPrinter.cs` lines ~24–83.

## `NuciExtensions` 6.0.2

| Aspect | Detail |
|--------|--------|
| Boundary | Extension method `Reverse()` on `string` |
| Direction | Outbound, in-process |
| Used by | `MenuPrinter.PrintTitle` only |
| Purpose | Render the mirrored right-hand title decoration |
| Failure semantics | No known failure mode; pure string manipulation |
| Note | The package is a broad utility library; only this one member is used |

Root `ARCHITECTURE.md` attributes `Reverse` to "string reversal used by title presentation", which matches the single call site.

## Internal integrations

| Boundary | Contract |
|----------|----------|
| `Menu` ↔ `MenuManager` | Manager constructs menus by reflection, assigns `ParentId`, registers by `Id`, disposes on close. `Menu.Exit` calls back into the singleton. |
| `Menu` ↔ `MenuPrinter` | `HandleHelp` passes the command dictionary for rendering; the manager passes menus and results. |
| `Command` ↔ `MenuManager` | Manager resolves commands and invokes `Execute()`; result used for optional statistics. |

## Non-integrations

No HTTP client, database driver, message broker, cache client, telemetry SDK, logging framework, filesystem watcher, or configuration provider is referenced. The architecture test enforcement makes adding one a deliberate, test-visible change.