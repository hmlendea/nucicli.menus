# Logging

## Summary

The library contains **no logging framework, no log levels, no structured fields, and no sinks**. Its only observability channel is console output produced through `NuciConsole` by `MenuManager` and `MenuPrinter`.

## What is written to the console

| Writer | Message | Colour | Condition |
|--------|---------|--------|-----------|
| `MenuPrinter.PrintTitle` | decoration + title + reversed decoration | decoration colour, title colour | every menu header |
| `MenuPrinter.PrintCommandList` | `name.padded description` rows | default | menu header, `help` |
| `MenuPrinter.PrintMenuHeader` | blank line | default | after rows |
| `MenuManager.OpenMenu` | blank line | default | opening a child while a menu is active |
| `MenuManager.TakeCommand` | `Unknown command` | red | unknown command name |
| `MenuManager.TakeCommand` | blank line | default | after each executed command |
| `MenuPrinter.PrintCommandResults` | status/duration line, optional `Error message: …` | green/red/yellow, red for error | statistics enabled |
| `Menu.HandleHelp` | command rows | default | `help` command |
| Command actions | whatever the host writes | host-controlled | each invocation |

## What is *not* recorded

| Concern | Status |
|---------|--------|
| Timestamps | Only inside statistics output, as a duration; no absolute timestamps are printed |
| Correlation or trace ids | None |
| Component or category prefixes | None |
| Severity levels | None; "Unknown command" and error messages are rendered inline, not classified |
| Stack traces | None; only `Exception.Message` in the failure statistics branch |
| Destinations other than the console | None |
| Retention, rotation, sampling | Not applicable |
| Redaction or masking | None; host command output and exception messages are emitted verbatim |
| Audit trail | None |
| Runtime level changes | Not applicable |

## Implications

1. Diagnosing a host problem requires console scrollback; the library leaves no artefact.
2. Because exception messages can reach the console through `PrintCommandResults`, hosts must avoid putting sensitive data in exception messages.
3. There is no seam for injecting a logger. Adding one is a breaking structural change (new dependency) and would violate the architecture test's allowed-reference list (see [`components/test-projects.md`](./components/test-projects.md)).
4. The console is the only output surface, and it is a static NuciCLI API, so output cannot be captured by unit tests without redirecting `Console`.

See [`behaviour/command-presentation.md`](./behaviour/command-presentation.md) for exact rendered text.