[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/fund.html) [![Build Status](https://github.com/hmlendea/nucicli.menus/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nucicli.menus/actions/workflows/dotnet.yml) [![Latest GitHub release](https://img.shields.io/github/v/release/hmlendea/nucicli.menus)](https://github.com/hmlendea/nucicli.menus/releases/latest)

# NuciCLI.Menus

## About

`NuciCLI.Menus` is a lightweight menu system for building interactive, command-driven console applications on top of [NuciCLI](https://github.com/hmlendea/nucicli).

It provides:

- Structured menus with automatic `help` and `exit` commands
- Hierarchical navigation (parent/child menus)
- Command registration with descriptions
- Optional command execution statistics
- Simple lifecycle events for manager state changes

## Contents

- [Installation](#installation)
- [Quick Start](#quick-start)
- [How It Works](#how-it-works)
- [Public API Overview](#public-api-overview)
- [Command Execution Statistics](#command-execution-statistics)
- [Menu Navigation](#menu-navigation)
- [Best Practices](#best-practices)
- [Related Packages](#related-packages)
- [License](#license)

## Installation

[![Get it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciCLI.Menus)

### .NET CLI

```bash
dotnet add package NuciCLI.Menus
```

### Package Manager

```powershell
Install-Package NuciCLI.Menus
```

## Quick Start

The library is built around two core types:

- `Menu`: defines commands and visual/menu behavior
- `MenuManager`: opens menus, reads user input, and executes commands

Example:

```csharp
using NuciCLI.Menus;

public sealed class MainMenu : Menu
{
	public MainMenu() : base("Main Menu")
	{
		AddCommand("hello", "Print a greeting", () =>
		{
			NuciConsole.WriteLine("Hello from NuciCLI.Menus!");
		});

		AddCommand("settings", "Open settings menu", () =>
		{
			MenuManager.Instance.OpenMenu<SettingsMenu>();
		});
	}
}

public sealed class SettingsMenu : Menu
{
	public SettingsMenu() : base("Settings")
	{
		AddCommand("show", "Show current settings", () =>
		{
			NuciConsole.WriteLine("No settings configured yet.");
		});
	}
}

internal static class Program
{
	private static void Main()
	{
		MenuManager.Instance.Start<MainMenu>();
	}
}
```

Run the app and type:

- `help` to list available commands in the current menu
- `settings` to navigate to the child menu
- `exit` to go back (or stop the app when at the root menu)

## How It Works

When a menu is created, two commands are added automatically:

- `help`: prints all commands of the active menu
- `exit`: closes the active menu

`MenuManager` manages the active menu loop:

1. Prints the menu header and command list
2. Reads user input using the active menu prompt
3. Resolves the command name and executes its action
4. Prints optional execution statistics (if enabled)
5. Repeats until the root menu is closed

If a command is unknown, the user gets an `Unknown command` message.

## Public API Overview

### `Menu`

Main customization points:

- `Title`: visible menu title
- `TitleDecoration`: decoration text around title (default: `-==< `)
- `Prompt`: command input prompt (default: `> `)
- `TitleColour`, `TitleDecorationColour`, `PromptColour`: console colors

Main methods:

- `AddCommand(string name, string description, Action action)`
- `Exit()`

Events:

- `Created`
- `Disposed`

### `MenuManager`

Singleton access:

- `MenuManager.Instance`

Core methods:

- `Start<TMenu>()`
- `Start<TMenu>(params object[] parameters)`
- `OpenMenu<TMenu>()`
- `OpenMenu<TMenu>(params object[] parameters)`
- `OpenMenu(Type menuType, params object[] parameters)`
- `CloseMenu()` / `CloseMenu(string menuId)`
- `SwitchToMenu(string menuId)`

State and configuration:

- `ActiveMenuId`
- `IsRunning`
- `AreStatisticsEnabled`

Events:

- `Starting`
- `Started`
- `Stopped`
- `ActiveMenuChanged`

## Command Execution Statistics

Set `MenuManager.Instance.AreStatisticsEnabled = true` to print command execution results:

- Status: `Success`, `Failed`, or `Cancelled`
- Duration in seconds/minutes
- Error message for failed commands

`InputCancellationException` is treated as `Cancelled` when thrown during command execution.

## Menu Navigation

Opening a menu from another menu creates a parent-child relation internally.

- `OpenMenu<TChild>()` moves focus to the child menu
- `exit` in the child menu returns to the parent menu
- `exit` in the root menu stops the manager loop

This makes it straightforward to model multi-level CLI applications.

## Best Practices

- Keep command names short, lower-case, and verb-oriented (`list`, `create`, `sync`)
- Use command descriptions as concise help text
- Organize features into focused submenus
- Enable statistics in development and troubleshooting workflows
- Handle expected input cancellation in command actions where relevant

## Related Packages

The NuciCLI ecosystem is split into focused packages:

- [NuciCLI](https://github.com/hmlendea/nucicli) for core console helpers
- [NuciCLI.Arguments](https://github.com/hmlendea/nucicli.arguments) for command-line argument handling
- [NuciCLI.Menus](https://github.com/hmlendea/nucicli.menus) for interactive terminal menus

## Target Framework

The current package targets `.NET 10.0`.

## License

This project is licensed under the `GNU General Public License v3.0` or later. See [LICENSE](./LICENSE) for details.
