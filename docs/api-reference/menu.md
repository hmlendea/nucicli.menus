# Menu

Namespace: `NuciCLI.Menus`
Assembly: `NuciCLI.Menus`
```csharp
public class Menu : IDisposable, IMenu
```

Base path: not applicable (class library).

## Constructors

### `Menu()`

Creates a menu with a fresh GUID `Id`, default colours and text, and the built-in `exit` and `help` commands.

| Effect | Detail |
|--------|--------|
| `Commands` | Initialised to empty, then two entries added |
| `Id` | `Guid.NewGuid().ToString()` |
| `TitleColour` | `NuciConsoleColour.Green` |
| `TitleDecorationColour` | `NuciConsoleColour.Yellow` |
| `PromptColour` | `NuciConsoleColour.White` |
| `Title` | `null!` (must be assigned) |
| `TitleDecoration` | `"-==< "` |
| `Prompt` | `"> "` |
| Events | `Created` raised at the end of the constructor |

Errors: none under normal conditions. A derived type whose constructor throws leaves the instance unregistered.

### `Menu(string title)`

Chains to `Menu()` and assigns `Title`. Raises `Created` **before** `Title` is assigned.

Errors: none.

## Properties

### `Id`

```csharp
public string Id { get; set; }
```

Registry key in `MenuManager.menus`. Reassigning after registration orphans the menu. Duplicate ids cause `ArgumentException` from `Dictionary.Add` in `OpenMenu`.

### `ParentId`

```csharp
public string? ParentId { get; set; }
```

Assigned by `MenuManager.OpenMenu` when a menu is opened while another is active. `null` for root menus.

### `Title`

```csharp
public string Title { get; set; }
```

Rendered between `TitleDecoration` and its reverse. Declared non-nullable with a `null!` initialiser because construction precedes assignment.

### `TitleDecoration`

```csharp
public string TitleDecoration { get; set; }   // default "-==< "
```

Prefix and (reversed) suffix around the title. Empty values are permitted.

### `Prompt`

```csharp
public string Prompt { get; set; }             // default "> "
```

Passed to `NuciConsole.ReadLine`.

### `TitleColour`, `TitleDecorationColour`, `PromptColour`

```csharp
public NuciConsoleColour TitleColour { get; set; }            // Green
public NuciConsoleColour TitleDecorationColour { get; set; }  // Yellow
public NuciConsoleColour PromptColour { get; set; }           // White
```

Colours from `NuciCLI`.

### `IsDisposed`

```csharp
public bool IsDisposed { get; }   // private set
```

`true` after `Dispose(true)` ran. Never reset.

## Events

### `Created`

```csharp
public event EventHandler? Created;
```

Raised inside the constructor. Sender is the menu instance; `EventArgs.Empty`.

### `Disposed`

```csharp
public event EventHandler? Disposed;
```

Raised once inside `Dispose(true)` after `Commands.Clear()`.

## Methods

### `AddCommand(string name, string description, Action action)`

Registers a command under the exact, case-sensitive `name`.

| Parameter | Type | Constraint |
|-----------|------|------------|
| `name` | `string` | Non-null; not already present |
| `description` | `string` | Any value, including `null` |
| `action` | `Action` | Not validated; `null` is accepted and fails on execution |

Errors:

| Condition | Exception |
|-----------|-----------|
| `name` is `null` | `ArgumentNullException` |
| `name` already registered (`exit`, `help`, or prior command) | `ArgumentException` |

Side effects: none beyond adding to the dictionary; the action is not invoked.

### `Exit()`

```csharp
public void Exit() => MenuManager.Instance.CloseMenu(Id);
```

Closes this menu on the process singleton manager. Returns to the parent when `ParentId` is set; otherwise stops the loop.

Errors: `KeyNotFoundException` when the singleton registry does not contain this `Id`.

### `Dispose()`

```csharp
public void Dispose()
{
    Dispose(true);
    GC.SuppressFinalize(this);
}
```

Clears commands, sets `IsDisposed`, raises `Disposed` once.

### `Dispose(bool disposing)` (protected virtual)

Overridable hook. Must call the base implementation to preserve disposal state and events. Calling with `false` (finaliser path) performs no work.

## Internal members

| Member | Note |
|--------|------|
| `Commands` | `Dictionary<string, Command>`; `exit` and `help` always present until disposal |
| `HandleHelp()` | Private; `MenuPrinter.PrintCommandList(Commands)` |

## Examples

```csharp
public sealed class SubMenu : Menu
{
    public SubMenu() : base("Sub Menu")
    {
        Prompt = "sub> ";
        AddCommand("action", "Do the thing", () => Console.WriteLine("done"));
    }
}
```

## See also

- [`../components/public-api.md`](../components/public-api.md)
- [`../behaviour/menu-lifecycle.md`](../behaviour/menu-lifecycle.md)
- [`../invariants.md`](../invariants.md)