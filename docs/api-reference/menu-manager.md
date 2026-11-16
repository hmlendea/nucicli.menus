# MenuManager

Namespace: `NuciCLI.Menus`
Assembly: `NuciCLI.Menus`
```csharp
public class MenuManager : IMenuManager
```

Base path: not applicable (class library).

## Singleton access

### `Instance`

```csharp
public static MenuManager Instance { get; }
```

Double-checked locking with a `System.Threading.Lock` and a `volatile` field. Thread-safe for first access. The constructor is public, so additional instances are possible; `Menu.Exit` always resolves this static instance.

## State properties

### `ActiveMenuId`

```csharp
public string? ActiveMenuId { get; private set; }
```

`null` before the first `SwitchToMenu`. Never reset to `null` after a session ends.

### `IsRunning`

```csharp
public bool IsRunning { get; private set; }
```

`true` between `OpenMenu` inside `Start` and closure of a menu without a parent.

### `AreStatisticsEnabled`

```csharp
public bool AreStatisticsEnabled { get; set; }   // default false
```

Enables the `PrintCommandResults` call after each executed command. Read per command.

## Events

Declared as public fields, not `event` accessors:

```csharp
public EventHandler? Starting;
public EventHandler? Started;
public EventHandler? Stopped;
public EventHandler? ActiveMenuChanged;
```

| Event | Raised when | Ordering notes |
|-------|-------------|----------------|
| `Starting` | First statement of `Start` | Before construction; `IsRunning` still `false`; `ActiveMenuId` still `null` |
| `Started` | After `IsRunning = true` | After header printed and `ActiveMenuChanged` |
| `Stopped` | After the loop terminates normally | Not raised when an exception escapes |
| `ActiveMenuChanged` | End of every effective `SwitchToMenu` | After the header print; not raised when the root closes |

## Methods

### `Start<TMenu>()` / `Start<TMenu>(params object[] parameters)`

Runs the blocking interactive loop.

| Phase | Action |
|-------|--------|
| 1 | Raise `Starting` |
| 2 | `OpenMenu<TMenu>(parameters)` |
| 3 | `IsRunning = true` |
| 4 | Raise `Started` |
| 5 | `while (IsRunning) TakeCommand()` |
| 6 | Raise `Stopped` |

| Condition | Exception |
|-----------|-----------|
| No constructor matches `parameters` | `MissingMethodException` |
| Type is not a `Menu` | Cast/activator exception |
| `Starting`/`Started` handler throws | Handler exception escapes |
| Command action throws (non-cancellation) | Exception escapes; `Stopped` skipped |
| `NuciConsole.ReadLine` returns `null` | `ArgumentNullException` from `Dictionary.TryGetValue` |

Side effects: mutates registry, active id, running flag, and console output. Returns when the root menu is closed.

### `OpenMenu<TMenu>()` / `OpenMenu<TMenu>(params object[] parameters)`

Generic convenience overloads delegating to the `Type` overload.

### `OpenMenu(Type menuType)` / `OpenMenu(Type menuType, params object[] parameters)`

Constructs and activates a menu.

| Step | Detail |
|------|--------|
| 1 | `(Menu)Activator.CreateInstance(menuType, parameters)!` |
| 2 | `menus.Add(newMenu.Id, newMenu)` |
| 3 | When a menu is active: `newMenu.ParentId = ActiveMenuId` and print a blank line |
| 4 | `SwitchToMenu(newMenu.Id)` |

| Condition | Exception |
|-----------|-----------|
| `menuType` is `null` | `NullReferenceException` |
| `menuType` is abstract, an interface, or not a `Menu` | Activator or cast exception |
| No matching constructor | `MissingMethodException` |
| Duplicate `Id` in the registry | `ArgumentException` |

Nothing is registered when construction fails.

### `CloseMenu()`

```csharp
public void CloseMenu() => CloseMenu(ActiveMenuId!);
```

Errors: `ArgumentNullException` when `ActiveMenuId` is `null`.

### `CloseMenu(string menuId)`

| Step | Detail |
|------|--------|
| 1 | Look up `menus[menuId]` |
| 2 | Capture `ParentId` |
| 3 | Remove from registry |
| 4 | `Dispose()` the menu |
| 5 | Switch to `ParentId` when non-empty, else `IsRunning = false` |

| Condition | Exception |
|-----------|-----------|
| Unknown id | `KeyNotFoundException`; no state change |
| `null` id | `ArgumentNullException` |

Closing a non-active menu is allowed and re-activates the closed menu's parent, which may differ from the previously active menu.

### `SwitchToMenu(string menuId)`

| Step | Detail |
|------|--------|
| 1 | Return when `menuId.Equals(ActiveMenuId)` |
| 2 | `ActiveMenuId = menuId` |
| 3 | `MenuPrinter.PrintMenuHeader(ActiveMenu)` |
| 4 | Raise `ActiveMenuChanged` |

| Condition | Exception |
|-----------|-----------|
| `menuId` is `null` | `NullReferenceException` |
| Unknown id | `KeyNotFoundException` after `ActiveMenuId` was already overwritten, leaving the manager pointed at a missing menu |

## Private members (behavioural reference)

| Member | Behaviour |
|--------|-----------|
| `ActiveMenu` | `menus[ActiveMenuId!]` |
| `TakeCommand()` | Read, cancellation handling, lookup, execute, optional statistics, trailing newline |

## Example

```csharp
MenuManager manager = MenuManager.Instance;
manager.AreStatisticsEnabled = true;
manager.Started += (_, _) => Console.WriteLine("Ready");
manager.Start<MainMenu>();
```

## See also

- [`../flows/startup-and-menu-opening.md`](../flows/startup-and-menu-opening.md)
- [`../flows/command-dispatch.md`](../flows/command-dispatch.md)
- [`../flows/menu-navigation-and-shutdown.md`](../flows/menu-navigation-and-shutdown.md)
- [`../components/public-api.md`](../components/public-api.md)