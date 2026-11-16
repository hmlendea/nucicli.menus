# State and persistence

## Summary

`NuciCLI.Menus` holds only process-local transient state and implements no persistence of any kind. This is a verified absence, not an omission: no file, database, cache, registry, environment-variable, or serialisation dependency exists in the library.

## Verified evidence for the absence of persistence

| Check | Result |
|-------|--------|
| Package references in `NuciCLI.Menus.csproj` | `NuciCLI`, `NuciExtensions` only |
| `System.IO` usage in production sources | none |
| Types with serialisation attributes | none |
| Configuration loading (`IConfiguration`, JSON readers) | none |
| Allowed assembly references enforced by architecture test | `System.Runtime`, `NuciCLI`, `NuciExtensions` |

## State inventory and lifetime

| State | Owner | Created | Cleared | Survives process exit |
|-------|-------|---------|---------|-----------------------|
| `Menu.Id` | `Menu` | Constructor (`Guid.NewGuid()`) | Never (per instance) | No |
| `Menu.ParentId` | `MenuManager.OpenMenu` | Menu open | With instance | No |
| `Menu.Commands` | `Menu` | Constructor and `AddCommand` | `Dispose(true)` | No |
| `Menu` presentation properties | `Menu` | Constructor / consumer assignment | With instance | No |
| `Menu.IsDisposed` | `Menu` | `Dispose(true)` | Never reset | No |
| `MenuManager.menus` | `MenuManager` | `OpenMenu` | `CloseMenu(menuId)` | No |
| `MenuManager.ActiveMenuId` | `MenuManager` | `SwitchToMenu` | Never reset to `null` | No |
| `MenuManager.IsRunning` | `MenuManager` | `Start` | `CloseMenu` on root | No |
| `MenuManager.AreStatisticsEnabled` | `MenuManager` | consumer assignment | Never reset | No |
| `CommandResult` | `CommandManager.TakeCommand` (local) | Each execution | Garbage collection | No |
| Singleton `instance` | `MenuManager` | First `Instance` access | Process end | No |

## Consequences for hosts

1. Any host-level persistence (settings, history, user profiles) must be implemented by the host and stored outside the library.
2. Because `ActiveMenuId` and `IsRunning` are not reset after `Start` returns, a second `Start` on the same instance inherits stale state (see [`flows/menu-navigation-and-shutdown.md`](./flows/menu-navigation-and-shutdown.md)).
3. Menus left registered by an aborting exception are never collected while the manager lives, because the dictionary holds strong references.

## Non-existent facilities

- No transactions, snapshots, or consistency model.
- No migrations or schema evolution.
- No caches, invalidation, or TTLs.
- No concurrency control beyond singleton initialisation (see [`concurrency-and-scheduling.md`](./concurrency-and-scheduling.md)).