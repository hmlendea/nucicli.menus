# API usage examples

The library exposes no HTTP API. Examples below use the public .NET surface.

## Minimal host

```csharp
using NuciCLI.Menus;

public sealed class MainMenu : Menu
{
    public MainMenu() : base("Main Menu")
    {
        AddCommand("hello", "Print a greeting", () => Console.WriteLine("Hello!"));
        AddCommand("settings", "Open settings", () => MenuManager.Instance.OpenMenu<SettingsMenu>());
    }
}

public sealed class SettingsMenu : Menu
{
    public SettingsMenu() : base("Settings")
    {
        AddCommand("toggle", "Toggle statistics", () =>
        {
            MenuManager.Instance.AreStatisticsEnabled = !MenuManager.Instance.AreStatisticsEnabled;
            Console.WriteLine($"Statistics: {MenuManager.Instance.AreStatisticsEnabled}");
        });
    }
}

MenuManager.Instance.Start<MainMenu>();
```

## Root menu with constructor arguments

```csharp
public sealed class GreetingMenu : Menu
{
    private readonly string user;

    public GreetingMenu(string user) : base($"Hello, {user}")
        => this.user = user;
}

MenuManager.Instance.Start<GreetingMenu>("player-one");
```

`Activator.CreateInstance` binds the string argument to the single-`string` constructor. Supplying two arguments produces `MissingMethodException`.

## Nested construction with parameters

```csharp
AddCommand("profile", "Open a profile", () => MenuManager.Instance.OpenMenu<ProfileMenu>(userId));
```

The child constructor must accept `string` for this to succeed; otherwise `MissingMethodException` escapes the command action and terminates the loop.

## Handling cancellation inside a command

```csharp
AddCommand("prompt", "Ask for input", () =>
{
    string? answer = NuciConsole.ReadLine("Name: ", NuciConsoleColour.White);
    Console.WriteLine($"Typed: {answer}");
});
```

If the host's `NuciConsole.ReadLine` throws `InputCancellationException`, the command yields `CommandStatus.Cancelled` and the loop continues.

## Keeping the loop alive after a failing command

```csharp
AddCommand("risky", "Run risky work", () =>
{
    try
    {
        ThrowSomething();
    }
    catch (Exception exception)
    {
        Console.WriteLine($"Failed: {exception.Message}");
    }
});
```

The library will not catch non-cancellation exceptions, so the action must handle them to keep the session alive.

## Reacting to lifecycle events

```csharp
MenuManager manager = MenuManager.Instance;

manager.Starting += (_, _) => Console.WriteLine("Starting");
manager.Started += (_, _) => Console.WriteLine("Started");
manager.ActiveMenuChanged += (_, _) => Console.WriteLine($"Active: {manager.ActiveMenuId}");
manager.Stopped += (_, _) => Console.WriteLine("Stopped");

manager.Start<MainMenu>();
```

Ordering observed: `Starting` → (`Created` of the root, if subscribed) → `ActiveMenuChanged` → `Started` → … → `Stopped`.

## Custom appearance

```csharp
public sealed class BrandedMenu : Menu
{
    public BrandedMenu()
    {
        Title = "Branded";
        TitleDecoration = "| ";
        Prompt = "branded> ";
        TitleColour = NuciConsoleColour.Cyan;
        TitleDecorationColour = NuciConsoleColour.Magenta;
        PromptColour = NuciConsoleColour.Gray;
    }
}
```

Note: the base `Menu()` constructor raises `Created` before this body runs, so a `Created` handler sees default values. Assignments made here apply before the manager prints the header.

## Navigating without `exit`

```csharp
AddCommand("home", "Jump to the main menu", () => MenuManager.Instance.SwitchToMenu(mainMenuId));
```

`SwitchToMenu` prints the target header and raises `ActiveMenuChanged`, but leaves the previous menu registered and undisposed. Prefer `CloseMenu` for stack-like navigation and reserve `SwitchToMenu` for deliberate jumps where the previous menu must stay live.

## Direct manager construction (advanced)

```csharp
MenuManager manager = new();
manager.AreStatisticsEnabled = true;

// Do not start a loop; open a menu to inspect state
manager.OpenMenu<MainMenu>();
Console.WriteLine(manager.ActiveMenuId);
manager.CloseMenu();
```

`Menu.Exit` still targets `MenuManager.Instance`, so any menu closed through the `exit` command under a non-singleton manager will attempt to close on the singleton registry.