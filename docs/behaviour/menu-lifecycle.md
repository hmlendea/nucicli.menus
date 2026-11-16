# Behaviour: menu lifecycle

## Events and their exact emission points

| Event | Declared in | Raised at | Notes |
|-------|-------------|-----------|-------|
| `Menu.Created` | `Menu` | End of the parameterless constructor | Fires before a derived constructor body runs and before `Title` is assigned via `Menu(string)`. |
| `Menu.Disposed` | `Menu` | Inside `Dispose(true)`, after `Commands.Clear()` | Exactly once per menu. |
| `MenuManager.Starting` | `MenuManager` | First statement of `Start<TMenu>(params object[])` | Before construction, before `IsRunning`. |
| `MenuManager.Started` | `MenuManager` | After `IsRunning = true` | Not raised if startup throws. |
| `MenuManager.Stopped` | `MenuManager` | After the loop exits | Not raised if a command exception escapes. |
| `MenuManager.ActiveMenuChanged` | `MenuManager` | End of `SwitchToMenu`, after printing the header | Raised for startup, child opening, and parent restoration; not raised when the root closes. |

Because the manager's events are public fields rather than `event` declarations, they are also assignable and invocable by consumers. Subscribing with `+=` is the intended use.

## Creation ordering (critical)

```csharp
public Menu()
{
    Commands = [];
    Id = Guid.NewGuid().ToString();
    TitleColour = Green;
    TitleDecorationColour = Yellow;
    PromptColour = White;
    AddCommand("exit", "Exit this menu", Exit);
    AddCommand("help", "Prints the command list", HandleHelp);
    Created?.Invoke(this, EventArgs.Empty);
}
```

Consequences:

1. A `Created` handler cannot subscribe another command safely if it expects the built-ins to be absent — they are already present.
2. In `Menu(string title)`, `Title` is still `null` when `Created` fires. Handlers that render `Title` will output `null`-equivalent text (interpolation yields empty).
3. A derived class that needs `Created` to observe its own `AddCommand` calls must instead register commands in its constructor body or use a different hook.

## Disposal ordering

```
Dispose() -> Dispose(true) -> GC.SuppressFinalize(this)
Dispose(true):
    if IsDisposed or !disposing: return
    IsDisposed = true
    Commands.Clear()
    Disposed?.Invoke(this, Empty)
```

- Commands are cleared **before** `Disposed` fires, so a handler enumerating `menu.Commands` sees an empty dictionary.
- Disposal never invokes command actions.
- Disposing an undisposed menu twice raises `Disposed` once (asserted by tests).
- The finaliser calls `Dispose(false)`, which returns immediately; therefore collection without explicit disposal leaves `IsDisposed == false` and never raises `Disposed`.

## Manager-owned disposal

Menus are disposed only by `MenuManager.CloseMenu`. No other path disposes them:

- `Start` returning normally has already disposed the root (through `exit`).
- An exception escaping the loop leaves the active menu registered and undisposed.
- Abandoning the manager leaves all menus in memory until the process ends.

## Event ordering example — open child, then exit

| Step | Events |
|------|--------|
| `Start<Root>()` | `Root.Created` (in ctor) → `Starting` → `ActiveMenuChanged` → `Started` |
| `open` command → `OpenMenu<Child>()` | `Child.Created` (in ctor) → `ActiveMenuChanged` |
| `exit` in child | `Child.Disposed` → `ActiveMenuChanged` (parent restored) |
| `exit` in root | `Root.Disposed` → loop ends → `Stopped` |

## Test coverage of lifecycle

| Assertion | Test |
|-----------|------|
| `IsDisposed` true and one `Disposed` raise | `MenuTests.GivenAnUndisposedMenu_WhenDisposingIt_ThenItIsMarkedAsDisposedAndRaisesTheEvent` |
| Second dispose raises once | `MenuTests.GivenADisposedMenu_WhenDisposingItAgain_ThenTheEventIsRaisedOnlyOnce` |
| Only `Starting` fires when construction fails | `MenuManagerLifecycleTests.GivenInvalidConstructorArguments_WhenStarting_ThenOnlyTheStartingEventIsRaised` |
| `Starting` handler exception escapes unchanged | two `…ThenTheOriginalExceptionEscapes` tests |
| `Starting` handler observes pre-startup state | `…ThenItObservesThePreStartupState` |

Not covered by tests: any successful `Started`/`Stopped`/`ActiveMenuChanged` sequence, and `Menu.Created`.