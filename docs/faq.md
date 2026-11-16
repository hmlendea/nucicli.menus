# FAQ

## Does this library provide a command-line argument parser?

No. Commands are zero-argument `Action` delegates, and the typed input is matched exactly against command names. Argument handling belongs to `NuciCLI.Arguments` or the host.

## Can a command receive user input?

Yes, by calling `NuciConsole.ReadLine` inside its action. Cancellation produces `CommandStatus.Cancelled` and the loop continues.

## Why does my command's exception terminate the session?

`Command.Execute` catches only `InputCancellationException`. Every other exception escapes `MenuManager.TakeCommand` and `Start`. Wrap the action body in a `try/catch`.

## Why is the statistics status never "Failed"?

Because nothing constructs a `CommandResult` with a non-cancellation exception. The `Failure` branch exists in `MenuPrinter` but is unreachable through the current `Execute()` implementation.

## Why is `Created` raised before my constructor body runs?

The base `Menu()` constructor raises it at its end. With `Menu(string title)`, `Title` is consequently still unset when `Created` fires. Register commands in the derived constructor instead of relying on `Created`.

## Can I customise the output formatting?

Not without editing `MenuPrinter`, which is `internal static`. Formatting is hard-coded (column width offset of four, status words, two-decimal durations).

## Can I mock the console for tests?

No. `NuciConsole` is a static API of `NuciCLI`, and no abstraction is introduced. This is why the loop and printer are untested.

## Is `MenuManager` reusable after `Start` returns?

Not reliably. `ActiveMenuId` is not reset and may reference a disposed menu, causing a spurious separator line on the next open and a `KeyNotFoundException` when that menu is later closed. Create a new `MenuManager` instead.

## Why does `exit` in a child menu not exit the whole application?

`exit` closes only the active menu. The root closes the loop because it has no parent. Nesting depth corresponds to the number of `exit` invocations required.

## Can I have two menu managers?

Yes via `new MenuManager()`, but `Menu.Exit` resolves `MenuManager.Instance`, so the built-in `exit` command targets the singleton regardless of which manager created the menu.

## Are command names case-insensitive?

No. `EXIT` is an unknown command. Register separate names if both casings must work.

## Is input trimmed?

No. Leading or trailing spaces make a command unknown.

## Where is menu state persisted?

Nowhere. All state is process memory; see [`state-and-persistence.md`](./state-and-persistence.md).

## Does the library log anything?

Only console output; there is no logging framework. See [`logging.md`](./logging.md).

## Why do the architecture tests forbid new package references?

The `NuciCLI.Menus` assembly reference whitelist is `System.Runtime`, `NuciCLI`, `NuciExtensions`. Adding a dependency requires a deliberate test update, which keeps the package's footprint minimal.

## What .NET version is required?

`net10.0`. The library uses `System.Threading.Lock`, which requires .NET 9 or later, and targets .NET 10.

## Which version numbers are current?

The package project declares `NuciCLI` 3.0.3, `NuciExtensions` 6.0.3 and version 1.5.8. `ARCHITECTURE.md` and `PRIVACY.md` still cite 3.0.1/5.3.1; see [`ambiguities-and-open-questions.md`](./ambiguities-and-open-questions.md).

## Why is `ProtectedDisposalMenu` unused?

It is a test helper exposing `Dispose(bool)` as `ReleaseResources(bool)` with no current consumer. It presumably supported a removed disposal test. See [`ambiguities-and-open-questions.md`](./ambiguities-and-open-questions.md).