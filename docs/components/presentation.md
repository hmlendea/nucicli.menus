# Component: presentation (`MenuPrinter`)

Source: `NuciCLI.Menus/MenuPrinter.cs`.

`internal static class MenuPrinter` is the sole rendering component. It writes exclusively through `NuciConsole` and formats all text inline (no localisation, no resources).

## Constants

| Constant | Value | Used by |
|----------|-------|---------|
| `CommandColumnSpacing` | `4` | `PrintCommandList` column width |
| `SuccessStatusText` | `"Success"` | `PrintStatus` |
| `FailureStatusText` | `"Failed"` | `PrintStatus` |
| `CancelledStatusText` | `"Cancelled"` | `PrintStatus` |

## `PrintMenuHeader(Menu menu)`

1. `PrintTitle(menu)`
2. `PrintCommandList(menu.Commands)`
3. `NuciConsole.WriteLine()` — blank line after the header

## `PrintTitle(Menu menu)`

```
Write(TitleDecoration, TitleDecorationColour)
Write(Title, TitleColour)
Write(TitleDecoration.Reverse(), TitleDecorationColour)
WriteLine()
```

With defaults the rendered title is `-==< Main Menu >==-`. `Reverse()` comes from `NuciExtensions`.

Edge case: `TitleDecoration` of `""` yields `Max(...)` over the command names only — but only `PrintCommandList` uses `Max`, so an empty decoration does not break title printing. A `Menu` with **no** commands does break `PrintCommandList` (see below).

## `PrintCommandList(IDictionary<string, Command> commands)`

```csharp
int commandColumnWidth = commands.Keys.Max(name => name.Length) + CommandColumnSpacing;
foreach (Command command in commands.Values)
    WriteLine($"{command.Name.PadRight(commandColumnWidth)} {command.Description}");
```

Verified behaviour and risks:

- Column width is computed **once** from the longest command name plus four.
- `commands.Keys.Max(...)` throws `InvalidOperationException` ("Sequence contains no elements") for an empty dictionary. This cannot occur for a normally constructed `Menu` (which always registers `exit` and `help`), but would occur after disposal (commands cleared) if `PrintCommandList` were reached.
- Enumeration order is `Dictionary` insertion order in practice for the current runtime, but the type contract does not guarantee ordering. Registration order is therefore stable in observed output but not a documented invariant.
- A `null` `Description` renders through interpolation as an empty string.
- Descriptions are not wrapped or truncated; long descriptions exceed terminal width.

## `PrintCommandResults(CommandResult result)`

```
WriteLine()
Write("Command finished with status ")
PrintStatus(result.Status)
Write($" after {GetHumanFriendlyDurationString(result.Duration)}")
if (Status == Failure) WriteLine($"Error message: {Exception?.Message}", Red)
WriteLine()
```

`PrintStatus` writes the status word coloured green (success), red (failed), or yellow (cancelled). Because `Command.Execute` never produces `Failure`, the failure branch is currently dead code reachable only through future changes or direct internal use.

## Duration formatting

| Condition | Format | Example |
|-----------|--------|---------|
| `< 1` minute total | `{TotalSeconds:0.00}s` | `0.42s` |
| otherwise | `{TotalMinutes:0.00}m` | `2.75m` |

Negative durations (clock adjustment during execution) render as negative numbers; there is no clamping.

## Coupling notes

- `MenuPrinter` is `internal`; consumers cannot override rendering. Replacing presentation requires editing this file.
- The printer performs no exception handling: a `NuciConsole` failure propagates to the caller (`MenuManager` or `Menu.HandleHelp`).
- `MenuPrinter` is a static class, so it cannot be mocked; no test currently exercises it (see [`../testing.md`](../testing.md)).