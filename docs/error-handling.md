# Error handling

## Taxonomy

| Exception | Origin | Where handled | Effect |
|-----------|--------|---------------|--------|
| `InputCancellationException` (NuciCLI) | `NuciConsole.ReadLine` | `MenuManager.TakeCommand` catch | `CursorY -= 1`, command skipped, loop continues |
| `InputCancellationException` (NuciCLI) | A command action | `Command.Execute` catch | Converted to `CommandResult` with `Status == Cancelled` |
| `KeyNotFoundException` | Unknown menu id | Not handled | Propagates to host |
| `ArgumentNullException` | `CloseMenu(null)`, `CloseMenu()` with no active menu, `Dictionary.TryGetValue(null)` for a `null` command line | Not handled | Propagates to host; terminates the loop |
| `NullReferenceException` | `SwitchToMenu(null)`, `null` command action invocation, `null` `menuType`, `PrintTitle` on a `null` title | Not handled | Propagates to host |
| `ArgumentException` | Duplicate command name or duplicate menu id | Not handled | Propagates to the registrar (constructor or command action) |
| `MissingMethodException` | `Activator.CreateInstance` with no matching constructor | Not handled | Propagates; nothing registered |
| `InvalidOperationException` | `PrintCommandList` with an empty command dictionary | Not handled | Propagates |
| Any host exception from a command action (non-cancellation) | Command action | Not handled (`Command.Execute` only catches cancellation) | Escapes `TakeCommand` and `Start`; `Stopped` not raised |
| `TargetInvocationException` | Not produced by `Activator.CreateInstance` for constructor failures — activator rethrows the original |
| Console exceptions (`IOException`, etc.) | `NuciConsole` | Not handled | Propagates |

## Design position

The library implements **no** exception translation, retry, fallback, or boundary handler. Every failure other than input cancellation surfaces to the host unchanged. The documented rationale (implied by the tests and root `ARCHITECTURE.md`) is that the host owns process-level recovery.

## Verified test evidence

| Test | Confirms |
|------|----------|
| `MenuManagerLifecycleTests.GivenInvalidConstructorArguments_WhenStarting_ThenOnlyTheStartingEventIsRaised` | Startup failure does not partially initialise |
| `GivenAThrowingStartingHandler_WhenStartingWithoutArguments_ThenTheOriginalExceptionEscapes` | No wrapping of handler exceptions |
| `MenuManagerTests.GivenAnUnknownIdentifier_WhenClosingAMenu_ThenNoStateOrEventsAreChanged` | `KeyNotFoundException`, no side effects |
| `GivenNoActiveMenu_WhenClosingIt_ThenAnArgumentNullExceptionIsThrown` | `ArgumentNullException` |
| `GivenANullIdentifier_WhenClosingAMenu_ThenAnArgumentNullExceptionIsThrown` | `ArgumentNullException` |
| `GivenANullIdentifier_WhenSwitchingMenus_ThenNoStateOrEventsAreChanged` | `NullReferenceException` |
| `GivenAnUnconstructibleType_WhenOpeningAMenu_ThenNoStateOrEventsAreChanged` | Exception, no partial state |
| `…WhenOpeningAMenuByType/Generically_ThenAMissingMethodExceptionIsThrown` | `MissingMethodException` |
| `MenuTests.GivenACommandNameAlreadyRegistered_…ThenAnArgumentExceptionIsThrown` | Duplicate rejection |
| `MenuCommandRegistrationTests.GivenANullName_…ThenTheActionIsNotExecuted` | `ArgumentNullException` |

## Commentary on gaps

1. `CommandStatus.Failure` is currently unreachable because `Command.Execute` rethrows everything except `InputCancellationException`. Either the printer's failure branch is precautionary or the catch is intended to be broadened; the code does not indicate which.
2. `Menu.AddCommand` permits a `null` action, deferring the failure to invocation time. This is asserted deliberately by `GivenANullAction_WhenRegisteringACommand_ThenItsNameIsReserved`.
3. A `null` return from `NuciConsole.ReadLine` would throw from the dictionary lookup rather than being treated as an empty command. Whether `ReadLine` can return `null` is a `NuciCLI` contract detail that this repository does not document.
4. Statistics printing failures (console errors) abort the loop even though the command succeeded.
5. No exception is ever logged; the only failure signal is the console output of the command itself plus, for cancellation, the `Cancelled` status.

## Host integration guidance

| Goal | Approach |
|------|----------|
| Keep the loop alive after a throwing command | Wrap the action body in a `try/catch` inside the action; the library will not catch it |
| Report command failure in statistics | Not possible today: catch inside the action and print manually |
| Distinguish user cancellation | Catch `InputCancellationException` inside the action and rethrow to obtain `Cancelled` status |
| Recover from a bad menu id | Validate ids before calling `CloseMenu`/`SwitchToMenu`; the manager does not |