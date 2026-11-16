# Behaviour: menu navigation

This document describes what a user of a host application observably experiences when navigating menus.

## Built-in navigation command

`exit` is registered by every `Menu` constructor with description `Exit this menu`. Its action is `Menu.Exit`:

```csharp
public void Exit() => MenuManager.Instance.CloseMenu(Id);
```

Observable effect:

| Active menu | What the user sees |
|-------------|--------------------|
| Root menu (no `ParentId`) | The prompt loop ends; `Stopped` fires; `Start` returns to the host |
| Child menu | The child's header disappears; the parent header (title plus its command list) is printed; the prompt returns |

Because the closing menu is disposed, a reopened menu is always a fresh instance: any state the host stored on the previous instance is gone unless the host kept it elsewhere.

## Opening a child menu

Hosts open children from a command action:

```csharp
AddCommand("settings", "Open settings", () => MenuManager.Instance.OpenMenu<SettingsMenu>());
```

Observable effect: a blank line, then the new menu's header, then the new prompt. The previous prompt line stays on screen as scrollback.

Nesting depth is unlimited; each level adds one `ParentId` link and one registry entry.

## Returning without `exit`

`MenuManager.Instance.CloseMenu()` closes the active menu and is equivalent to `exit` for that menu. `SwitchToMenu(id)` jumps directly to any registered menu id, which can skip levels; the target's header is printed and the previous menu remains registered (not disposed) unless it is later closed.

## Ordering guarantees visible to users

1. The header is always printed **before** the corresponding prompt.
2. `ActiveMenuChanged` fires after the header is printed.
3. A child menu's parent header is reprinted each time it becomes active again.
4. Opening a menu from the root prints a leading blank line; opening the root itself does not.

## Failure behaviour visible to users

| Situation | What the user sees |
|-----------|--------------------|
| Unknown command | `Unknown command` in red, then a new prompt |
| Command that throws | Whatever the command printed, then the exception surfaces to the host (typically a crash or host-level handler); no further prompt |
| `InputCancellationException` while reading | The prompt line is overwritten (cursor row decremented) and the loop continues |
| Command registered with a `null` action | Exception on invocation, escaping the loop |
| `exit` invoked from a nested command two levels deep | Only the active menu closes; the grandparent is not reached |

## Non-goals

- No breadcrumb trail, prompt prefix, or "back" command beyond `exit`.
- No history, completion, arrow-key navigation, or search.
- No confirmation prompt before closing.

See [`../flows/menu-navigation-and-shutdown.md`](../flows/menu-navigation-and-shutdown.md) for the mechanism.