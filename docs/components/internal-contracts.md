# Component: internal contracts (`IMenu`, `IMenuManager`)

Source: `NuciCLI.Menus/IMenu.cs`, `NuciCLI.Menus/IMenuManager.cs`.

Both interfaces are `internal`. They are declared in the base list of their implementations and are verified indirectly by the architecture tests (which require one type per file and the public types `Menu` and `MenuManager`).

## `IMenu`

| Member | Implemented by |
|--------|----------------|
| `string Id { get; set; }` | `Menu.Id` |
| `string? ParentId { get; set; }` | `Menu.ParentId` |
| `NuciConsoleColour TitleColour { get; set; }` | `Menu.TitleColour` |
| `NuciConsoleColour TitleDecorationColour { get; set; }` | `Menu.TitleDecorationColour` |
| `NuciConsoleColour PromptColour { get; set; }` | `Menu.PromptColour` |
| `string Title { get; set; }` | `Menu.Title` |
| `string TitleDecoration { get; set; }` | `Menu.TitleDecoration` |
| `string Prompt { get; set; }` | `Menu.Prompt` |
| `bool IsDisposed { get; }` | `Menu.IsDisposed` |
| `event EventHandler? Created` | `Menu.Created` |
| `event EventHandler? Disposed` | `Menu.Disposed` |
| `void Exit()` | `Menu.Exit` |
| `void AddCommand(string, string, Action)` | `Menu.AddCommand` |

`IMenu` does **not** declare `Dispose()`, `Commands`, or the dispose pattern, even though `Menu` implements `IDisposable`. Consumers working through the concrete type are unaffected (the interface is internal and unreachable outside the assembly).

## `IMenuManager`

| Member | Implemented by |
|--------|----------------|
| `string? ActiveMenuId { get; }` | `MenuManager.ActiveMenuId` |
| `bool IsRunning { get; }` | `MenuManager.IsRunning` |
| `bool AreStatisticsEnabled { get; set; }` | `MenuManager.AreStatisticsEnabled` |
| `void Start<TMenu>()` | `MenuManager.Start<TMenu>()` |
| `void Start<TMenu>(params object[])` | `MenuManager.Start<TMenu>(params object[])` |
| `void OpenMenu<TMenu>()` / `OpenMenu<TMenu>(params object[])` | Generic overloads |
| `void OpenMenu(Type)` / `OpenMenu(Type, params object[])` | Reflection overloads |
| `void CloseMenu()` / `CloseMenu(string)` | Close overloads |
| `void SwitchToMenu(string)` | Switch |

`IMenuManager` does **not** declare `Instance` (static), the lifecycle events (`Starting`, `Started`, `Stopped`, `ActiveMenuChanged`), or `ActiveMenu`. Those are therefore reachable only through the concrete type, which is what all consumers use.

## Why the interfaces exist

Evidence: both are `internal`, no other production code accepts an `IMenu`/`IMenuManager` parameter, and no test uses them. They currently document the member contract per type and provide a compile-time guarantee that the implementation covers every declared member. They are not an extension seam.

Implication for modification: adding a member to either interface requires updating exactly one implementation (`Menu` or `MenuManager`) and cannot break external consumers, because the interfaces are not part of the package's public surface.