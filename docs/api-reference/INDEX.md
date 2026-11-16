# API reference index

`NuciCLI.Menus` is a class library, not a service. There are no HTTP endpoints, controllers, routes, or authentication schemes. This reference documents the public .NET surface.

## Global facts

| Aspect | Value |
|--------|-------|
| Assembly | `NuciCLI.Menus` |
| Namespace | `NuciCLI.Menus` (all types) |
| Target framework | `net10.0` |
| Package version documented | 1.5.7 |
| Authentication | Not applicable |
| Versioning | NuGet semantic versions derived from GitHub release tags |
| Rate limiting | Not applicable |

## Public types

| Type | Kind | Purpose | Detail |
|------|------|---------|--------|
| `Menu` | public class, `IDisposable` | Base class for host menus; owns commands and presentation settings | [`./menu.md`](./menu.md) |
| `MenuManager` | public class | Singleton orchestrator: registry, navigation, input loop, lifecycle events | [`./menu-manager.md`](./menu-manager.md) |

## Internal types (not part of the package contract)

| Type | Kind | Purpose |
|------|------|---------|
| `IMenu` | internal interface | Member contract for `Menu` |
| `IMenuManager` | internal interface | Member contract for `MenuManager` |
| `Command` | internal sealed class | Name, description, action, execution timing |
| `CommandResult` | internal sealed class | Timing, exception, derived status |
| `CommandStatus` | internal enum | `Success`, `Failure`, `Cancelled` |
| `MenuPrinter` | internal static class | All console rendering |

## Public member summary

### `Menu`

| Member | Signature | Access |
|--------|-----------|--------|
| Constructor | `Menu()` | public |
| Constructor | `Menu(string title)` | public |
| `Id` | `string { get; set; }` | public |
| `ParentId` | `string? { get; set; }` | public |
| `Title` | `string { get; set; }` | public |
| `TitleDecoration` | `string { get; set; }` | public |
| `Prompt` | `string { get; set; }` | public |
| `TitleColour` | `NuciConsoleColour { get; set; }` | public |
| `TitleDecorationColour` | `NuciConsoleColour { get; set; }` | public |
| `PromptColour` | `NuciConsoleColour { get; set; }` | public |
| `IsDisposed` | `bool { get; }` | public |
| `Created` | `event EventHandler?` | public |
| `Disposed` | `event EventHandler?` | public |
| `AddCommand` | `void AddCommand(string name, string description, Action action)` | public |
| `Exit` | `void Exit()` | public |
| `Dispose` | `void Dispose()` | public |
| `Dispose(bool)` | `protected virtual void Dispose(bool disposing)` | protected |
| `Commands` | `Dictionary<string, Command> { get; }` | internal |

### `MenuManager`

| Member | Signature | Access |
|--------|-----------|--------|
| `Instance` | `static MenuManager Instance { get; }` | public static |
| `ActiveMenuId` | `string? { get; }` | public |
| `IsRunning` | `bool { get; }` | public |
| `AreStatisticsEnabled` | `bool { get; set; }` | public |
| `Starting` | `EventHandler?` (field) | public |
| `Started` | `EventHandler?` (field) | public |
| `Stopped` | `EventHandler?` (field) | public |
| `ActiveMenuChanged` | `EventHandler?` (field) | public |
| `Start<TMenu>()` | `void` | public |
| `Start<TMenu>(params object[])` | `void` | public |
| `OpenMenu<TMenu>()` | `void` | public |
| `OpenMenu<TMenu>(params object[])` | `void` | public |
| `OpenMenu(Type)` | `void` | public |
| `OpenMenu(Type, params object[])` | `void` | public |
| `CloseMenu()` | `void` | public |
| `CloseMenu(string)` | `void` | public |
| `SwitchToMenu(string)` | `void` | public |

## Related documentation

- Flows: [`../flows/`](../flows/)
- Components: [`../components/`](../components/)
- Data model: [`../data-model.md`](../data-model.md)
- Invariants: [`../invariants.md`](../invariants.md)
- Usage examples: [`../api-usage-examples.md`](../api-usage-examples.md)