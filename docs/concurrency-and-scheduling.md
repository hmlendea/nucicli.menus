# Concurrency and scheduling

## Threading model

The library is single-threaded by design. `MenuManager.Start` blocks the calling thread for the lifetime of the menu session, and every command action runs synchronously on that thread.

| Concern | Implementation |
|---------|----------------|
| Async/await | None. No `Task`, `async`, or `await` appears in production sources. |
| Background threads | None. |
| Timers/schedulers | None. |
| Parallel command execution | None. |

## The only synchronisation primitive

```csharp
private static readonly Lock syncRoot = new();
private static volatile MenuManager? instance;

public static MenuManager Instance
{
    get
    {
        if (instance is null)
        {
            lock (syncRoot)
            {
                instance ??= new MenuManager();
            }
        }

        return instance;
    }
}
```

- `System.Threading.Lock` (available from .NET 9) replaces `object`-based locks; it is not reentrant in the `Monitor` sense but provides the same scoped-lock syntax.
- Double-checked locking is sound because `instance` is `volatile`.
- Only singleton construction is protected. `menus`, `ActiveMenuId`, and `IsRunning` are **not** synchronised.

## Concurrency hazards (verified from code, not exercised by tests)

| Hazard | Mechanism |
|--------|-----------|
| Concurrent `OpenMenu` from two threads | `Dictionary.Add` on `menus` without a lock; may throw or corrupt the dictionary |
| `CloseMenu` racing with `TakeCommand` | `menus[ActiveMenuId]` can observe a removed key ⇒ `KeyNotFoundException` |
| `Menu.Commands` mutated while enumerated | `AddCommand` during `PrintCommandList` or `TryGetValue`; only possible if a command action registers commands on the active menu — sequential execution makes this safe in practice |
| Signal from another thread to stop the loop | No supported mechanism. `IsRunning` has a private setter and `CloseMenu` is the only writer; a cross-thread `CloseMenu` is the racing case above |
| `NuciConsole` thread affinity | `NuciConsole` is a static console wrapper; using the library from multiple threads simultaneously is not supported by NuciCLI's contract |

## Practical scheduling constraints

1. Long-running command actions block all input and rendering. Hosts that need background work must implement it themselves and marshal console output back to the loop thread.
2. `DateTime.Now` is read twice per command; no monotonic clock is used.
3. `GC.SuppressFinalize` is called by `Dispose()`; the finaliser exists but is effectively inert because `Dispose(false)` returns early.
4. No `CancellationToken` is accepted or produced anywhere.

## Recommended usage

| Requirement | Approach |
|-------------|----------|
| Single interactive session per process | Use `MenuManager.Instance` from the main thread only |
| Multiple independent managers | `new MenuManager()` per session, but note `Menu.Exit` always targets the singleton |
| Background processing | Start work inside the action, keep the console output on the loop thread, and expose results through commands |
| Graceful shutdown from another thread | Not supported; close the menu from within the loop |