# Component: public API (`Menu`, `MenuManager`)

Source: `NuciCLI.Menus/Menu.cs`, `NuciCLI.Menus/MenuManager.cs`.

## `NuciCLI.Menus.Menu`

```csharp
public class Menu : IDisposable, IMenu
```

### Construction

| Constructor | Behaviour |
|-------------|-----------|
| `Menu()` | Initialises `Commands`, assigns `Id = Guid.NewGuid().ToString()`, sets default colours (`TitleColour = Green`, `TitleDecorationColour = Yellow`, `PromptColour = White`), registers `exit` and `help`, then raises `Created`. |
| `Menu(string title)` | Chains to `Menu()` and assigns `Title`. |

Important ordering detail: `Created?.Invoke(...)` runs at the end of the parameterless constructor, i.e. **before** a derived class's constructor body executes and before `Title` is set by the `Menu(string)` overload. Subscribers therefore observe a menu whose `Title` is `null` when constructed through `Menu(string title)`. `Title` is declared `= null!` precisely because it is not yet initialised at that point.

### Properties

| Property | Type | Default | Notes |
|----------|------|---------|-------|
| `Id` | `string` | new GUID string | Settable; used as the registry key. Changing it after registration orphans the menu. |
| `ParentId` | `string?` | `null` | Assigned by `MenuManager.OpenMenu` when a menu is opened while another is active. |
| `Title` | `string` | `null!` (must be set) | Rendered between decorations. |
| `TitleDecoration` | `string` | `"-==< "` | Rendered before and reversed after the title. |
| `Prompt` | `string` | `"> "` | Passed to `NuciConsole.ReadLine`. |
| `TitleColour` | `NuciConsoleColour` | `Green` | Title text colour. |
| `TitleDecorationColour` | `NuciConsoleColour` | `Yellow` | Decoration colour. |
| `PromptColour` | `NuciConsoleColour` | `White` | Prompt colour. |
| `IsDisposed` | `bool` | `false` | Set once by `Dispose(true)`. |
| `Commands` | `Dictionary<string, Command>` | empty, then `exit`/`help` | `internal`; exact, case-sensitive keys. |

### Events

- `Created` — raised in the constructor.
- `Disposed` — raised once during `Dispose(true)`, after clearing commands.

### Methods

| Method | Behaviour |
|--------|-----------|
| `AddCommand(string name, string description, Action action)` | Creates a `Command` and calls `Commands.Add`. Duplicate key ⇒ `ArgumentException` (from `Dictionary.Add`). Null `name` ⇒ `ArgumentNullException`. `action` is **not** null-checked, so a `null` action registers successfully and throws `NullReferenceException` only if the command is later executed. |
| `Exit()` | `MenuManager.Instance.CloseMenu(Id)`. Delegates to whichever manager instance is the process singleton, not necessarily the manager that created the menu. |
| `Dispose()` | `Dispose(true)` + `GC.SuppressFinalize(this)`. |
| `Dispose(bool disposing)` (protected virtual) | Returns immediately when `IsDisposed` or `!disposing`. Otherwise sets `IsDisposed = true`, clears `Commands`, raises `Disposed`. Idempotent for repeated calls. |

### Disposal semantics

- Commands are cleared, so a disposed menu's `Commands` dictionary is empty.
- No command action is invoked during disposal (verified by `MenuCommandRegistrationTests.GivenRegisteredCommands_WhenDisposingTheMenu_ThenActionsAreNotExecuted`).
- A child class overriding `Dispose(bool)` must call the base implementation to preserve `IsDisposed` and the event.

## `NuciCLI.Menus.MenuManager`

```csharp
public class MenuManager : IMenuManager
```

### Singleton

```csharp
private static readonly Lock syncRoot = new();
private static volatile MenuManager? instance;
public static MenuManager Instance { get; }   // double-checked locking
```

- Uses .NET 9+ `System.Threading.Lock`.
- Double-checked locking is safe because `instance` is `volatile`.
- The constructor is implicit and public — consumers may create additional instances (`new MenuManager()`), which the unit tests do. Those instances do not participate in `Menu.Exit`, which always resolves the static singleton.

### State

| Member | Type | Notes |
|--------|------|-------|
| `menus` | `Dictionary<string, Menu>` (private readonly) | Registry keyed by `Menu.Id`. No removal of stale entries beyond `CloseMenu`. |
| `ActiveMenuId` | `string?` | Set by `SwitchToMenu`. |
| `IsRunning` | `bool` | `true` only between `OpenMenu` and root closure inside `Start`. |
| `AreStatisticsEnabled` | `bool` | Gates `MenuPrinter.PrintCommandResults`. |
| `ActiveMenu` | `Menu` (private) | `menus[ActiveMenuId!]`; throws `ArgumentNullException` when `ActiveMenuId` is null. |

### Events

`Starting`, `Started`, `Stopped`, `ActiveMenuChanged` are declared as public **fields** of type `EventHandler?`, not `event` accessors. Consequences: consumers can invoke or replace them directly, and the `IMenuManager` interface does not declare them.

### Methods

| Method | Behaviour |
|--------|-----------|
| `Start<TMenu>()` | `Start<TMenu>([])`. |
| `Start<TMenu>(params object[] parameters)` | Raises `Starting`; `OpenMenu<TMenu>(parameters)`; `IsRunning = true`; raises `Started`; loops `while (IsRunning) TakeCommand()`; raises `Stopped`. Any exception in `Starting` or `OpenMenu` aborts before `IsRunning` is set, so `Stopped` is never raised. |
| `OpenMenu<TMenu>()` / `OpenMenu<TMenu>(params object[])` | Delegates to `OpenMenu(typeof(TMenu), parameters)`. |
| `OpenMenu(Type menuType)` / `OpenMenu(Type, params object[])` | `Activator.CreateInstance(menuType, parameters)` cast to `Menu`; added to `menus`; if a menu is already active, `ParentId = ActiveMenuId` and a blank line is printed; then `SwitchToMenu(newMenu.Id)`. |
| `CloseMenu()` | `CloseMenu(ActiveMenuId!)`; throws `ArgumentNullException` when no menu is active. |
| `CloseMenu(string menuId)` | `menus[menuId]` (⇒ `KeyNotFoundException` for unknown ids); captures `ParentId`; removes from registry; disposes; switches to parent if non-empty, otherwise sets `IsRunning = false`. |
| `SwitchToMenu(string menuId)` | No-op when `menuId` equals `ActiveMenuId` (uses `string.Equals`, so a `null` argument throws `NullReferenceException`); otherwise sets `ActiveMenuId`, prints the header, raises `ActiveMenuChanged`. |

### Construction failure modes

`Activator.CreateInstance` produces:

- `MissingMethodException` when the argument list matches no constructor (verified by tests using `("Dark Souls", …)` pairs against `Menu(string)`).
- `MissingMethodException`/`TargetInvocationException`/`ArgumentException` for non-`Menu` types, abstract types, or types without a usable constructor. Tests assert only `Throws.Exception` because the concrete type varies with the input.
- A `NullReferenceException` when `menuType` is `null` (not wrapped).

In every case the registry is untouched because the instance is added only after successful construction.

### Loop behaviour in `TakeCommand`

```
try  cmd = NuciConsole.ReadLine(ActiveMenu.Prompt, ActiveMenu.PromptColour)
catch (InputCancellationException) -> CursorY -= 1 ; return
if !Commands.TryGetValue(cmd, out command)  -> write "Unknown command" in Red ; return
result = command.Execute()
if AreStatisticsEnabled -> PrintCommandResults(result)
NuciConsole.WriteLine()
```

Notable consequences:

- `cmd` may be `null`; `Dictionary.TryGetValue(null, …)` throws `ArgumentNullException`, which escapes the loop and therefore `Start`. In practice, whether `ReadLine` can return `null` depends on `NuciCLI`.
- Command lookup is exact and case-sensitive: `EXIT` is unknown even though `exit` exists.
- After a command, an unconditional blank line is printed; a cancelled read instead decrements the cursor row by one.
- Non-`InputCancellationException` exceptions raised by the command action propagate out of `Command.Execute`, out of `TakeCommand`, and out of `Start`, so `Stopped` is not raised.
- Closing the root menu from inside a command sets `IsRunning = false`; the current `TakeCommand` still prints its trailing newline before the loop condition is re-evaluated.

See [`command-execution.md`](./command-execution.md) for the execution layer.