# Configuration

## Summary

The library has **no configuration file, no environment variables, no command-line options, and no external configuration source**. All behaviour is controlled through public properties assigned at runtime by the host application.

## Runtime-configurable behaviour

| Setting | Owner | Type | Default | Effect |
|---------|-------|------|---------|--------|
| `AreStatisticsEnabled` | `MenuManager` | `bool` | `false` | When `true`, prints status/duration after each command |
| `Title` | `Menu` | `string` | unset (`null!`) | Title text between decorations |
| `TitleDecoration` | `Menu` | `string` | `"-==< "` | Prefix, reversed as suffix |
| `Prompt` | `Menu` | `string` | `"> "` | Prompt text passed to `ReadLine` |
| `TitleColour` | `Menu` | `NuciConsoleColour` | `Green` | Title colour |
| `TitleDecorationColour` | `Menu` | `NuciConsoleColour` | `Yellow` | Decoration colour |
| `PromptColour` | `Menu` | `NuciConsoleColour` | `White` | Prompt colour |
| `Id` | `Menu` | `string` | new GUID | Registry key |
| `ParentId` | `Menu` | `string?` | `null` | Set by the manager on open |

Consumers assign these in constructors (for menu appearance) or before `Start` (for statistics):

```csharp
MenuManager manager = MenuManager.Instance;
manager.AreStatisticsEnabled = true;
manager.Start<MainMenu>();
```

## Precedence

There is exactly one source (in-code assignment), so no precedence rules exist. Constructor defaults are overwritten by any later assignment; there is no validation or coercion.

## Reload behaviour

Changes take effect on next use:

- `AreStatisticsEnabled` is read per command, so toggling it mid-session works immediately.
- Menu presentation properties are read when a header is printed, so changing them affects the next header render, not the current one.
- `Id` changes after registration orphan the menu in the registry.

## Absent facilities

- No options objects, builders, or fluent configuration.
- No secret handling, because no credentials exist.
- No profiles, environments, or per-user scope.
- No validation of property values (an empty `TitleDecoration` is allowed; a `null` `Title` renders as empty).
- No hot reload from external sources.

See [`invariants.md`](./invariants.md) for constraints on these values and [`data-model.md`](./data-model.md) for ownership.