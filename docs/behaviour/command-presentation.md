# Behaviour: command presentation

## The `help` command

Registered by every `Menu` constructor as `help` / `Prints the command list`; its action is `Menu.HandleHelp`:

```csharp
private void HandleHelp() => MenuPrinter.PrintCommandList(Commands);
```

Output characteristics:

- One row per registered command, in dictionary enumeration order.
- Each row is `name` padded to `(longest name length + 4)` followed by a single space and the description.
- The `help` row itself appears in the list (it is a registered command).
- The menu title is **not** reprinted by `help`; only the command rows.
- No leading blank line (unlike the statistics block).

Example output for the README example menu:

```
exit      Exit this menu
help      Prints the command list
hello     Print a greeting
sub       Open the sub menu
```

## Unknown commands

`MenuManager.TakeCommand` writes `Unknown command` in red and continues. No suggestions, no prefix matching, no case folding.

## Execution statistics

Enabled by `MenuManager.AreStatisticsEnabled = true` (default `false`). For every executed command the manager prints:

```
<empty line>
Command finished with status <Status> after <duration>
[Error message: <message>]      // only when Status == Failure
<empty line>
```

Then `TakeCommand` prints one more blank line.

Observable example (statistics enabled):

```
> hello
Hello!

Command finished with status Success after 0.00s

>
```

### Status words and colours

| Status | Text | Colour | Reachable now |
|--------|------|--------|---------------|
| `Success` | `Success` | Green | Yes |
| `Failure` | `Failed` | Red | No — `Command.Execute` does not produce it |
| `Cancelled` | `Cancelled` | Yellow | Yes, when the action throws `InputCancellationException` |

### Duration formatting

| Duration | Rendered |
|----------|----------|
| `< 60 s` | seconds, two decimals, suffix `s` |
| `≥ 60 s` | minutes, two decimals, suffix `m` |

Durations come from `DateTime.Now` deltas, so they include time spent writing console output inside the action.

## Disabled statistics

With `AreStatisticsEnabled == false` (the default) only the command's own output is visible, followed by one blank line.

## Interaction with navigation commands

- `exit`: the menu closes; when statistics are enabled the `Success` line and blank lines appear after the parent header has already been printed.
- `help`: the command list is followed by the statistics block when enabled, which visually separates the list from the next prompt.

## Sensitive output

Failure messages print `Exception.Message` verbatim. Host actions that include sensitive data in exception messages will therefore surface it on the console; the library performs no redaction.