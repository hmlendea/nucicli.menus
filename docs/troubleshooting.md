# Troubleshooting

Every entry is derived from an observed code path or a drafting observation; no speculative issues are listed.

| Symptom | Probable cause | Resolution |
|---------|----------------|------------|
| `MissingMethodException` when calling `Start<TMenu>(...)` | Constructor arguments do not match any `TMenu` constructor | Match the constructor arity and types exactly; `Activator.CreateInstance` performs no coercion |
| `NullReferenceException` when calling `OpenMenu<TMenu>()` | Menu type cast failed (type is not a `Menu`) | Derive the menu from `Menu` |
| `ArgumentException` ("An item with the same key has already been added") on menu open | Two menus ended up with the same `Id` | Do not assign a custom `Id` that collides; let the GUID default stand |
| `ArgumentException` when adding a command | Name already registered, including built-ins `exit`/`help`, or case-identical duplicate | Rename the command; names are case-sensitive but the same casing is a duplicate |
| `KeyNotFoundException` on `CloseMenu(id)` | Id not in the registry (typo, already closed, or created by a different `MenuManager` instance) | Ensure the menu was opened by the same manager; `Menu.Exit` always uses the singleton |
| `ArgumentNullException` from `CloseMenu()` | No active menu | Check `ActiveMenuId` before closing |
| `NullReferenceException` from `SwitchToMenu(null)` | Passed `null` | Pass a registered id |
| Loop ends silently after `exit` in a child menu but the application did nothing | Expected: only the active menu closes; the parent remains | Close each level or call `CloseMenu` on the root |
| Exception from a command crashes the application | Non-`InputCancellationException` exceptions escape the loop | Wrap action bodies in `try/catch` inside the command |
| `Stopped` event never fires | An exception escaped the loop, or `Started` was never reached | Handle exceptions inside commands; verify startup succeeded |
| Statistics never appear | `AreStatisticsEnabled` left at its default `false` | Set it before `Start` |
| Statistics show `0.00s` for instant commands | `DateTime.Now` resolution and two-decimal formatting | Expected; sub-tick durations round to zero |
| Second `Start` behaves oddly (extra blank line, `KeyNotFoundException` later) | `ActiveMenuId` retained from the previous session and points at a disposed menu | Use a new `MenuManager` instance, or reset the process; the manager is not designed for reuse |
| `InvalidOperationException` ("Sequence contains no elements") during header print | `PrintCommandList` called on a menu with no commands (only possible after disposal or manual clearing) | Do not render a disposed menu |
| `help` lists commands in an unexpected order | Enumerating a `Dictionary`; insertion order is observed but not contractual | Do not rely on ordering |
| `Created` handler sees a `null` title | Raised inside `Menu()` before `Menu(string)` assigns `Title` | Read `Title` after construction, or override behaviour in the derived constructor |
| `Created` handler cannot find its own commands | Handler runs before the derived constructor body | Register commands in the derived constructor and subscribe afterwards |
| Disposal does not raise `Disposed` | The menu was garbage-collected without `Dispose()`; the finaliser path is a no-op | Dispose via `MenuManager.CloseMenu` or explicitly |
| Menu stays in memory after an exception | Registry holds a strong reference; only `CloseMenu` removes entries | Treat loop-aborting exceptions as fatal and rebuild the manager |
| Architecture tests fail after adding a `using`-referenced package | `ProductionArchitectureTests` whitelist | Add the assembly name to `allowedReferences` deliberately and document it |
| Architecture tests fail after adding a second type to a file | One-type-per-file rule | Split into a file named after the type |
| Architecture tests fail after using a file-scoped namespace | Explicit block namespace rule | Convert to `namespace X { … }` |
| `dotnet test` cannot locate the production project | Architecture tests resolve `../../../../NuciCLI.Menus/…` relative to the test output directory | Run tests from the repository layout; do not relocate the output directory |