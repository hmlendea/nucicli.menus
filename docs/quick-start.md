# Quick start

## Prerequisites

- .NET 10.0 SDK.

## Build and test the repository

```bash
dotnet restore
dotnet build --no-restore
dotnet test --no-build --verbosity normal
```

`dotnet test` runs all three projects: unit tests, architecture tests, and their combination inside the solution.

## Consume the package

```bash
dotnet add package NuciCLI.Menus
```

## Minimal host application

```csharp
using NuciCLI.Menus;

public sealed class MainMenu : Menu
{
    public MainMenu() : base("Main Menu")
    {
        AddCommand("hello", "Print a greeting", () => Console.WriteLine("Hello!"));
        AddCommand("sub", "Open the sub menu", () => MenuManager.Instance.OpenMenu<SubMenu>());
    }
}

public sealed class SubMenu : Menu
{
    public SubMenu() : base("Sub Menu")
    {
        AddCommand("back", "Return to the main menu", () => MenuManager.Instance.CloseMenu());
    }
}

MenuManager.Instance.Start<MainMenu>();
```

Behaviour of the example:

- `MenuManager.Instance.Start<MainMenu>()` prints the header and then blocks reading commands.
- `help` lists every registered command including the built-ins.
- `exit` on the root menu ends the loop; `exit` on a child returns to its parent.
- `sub` opens `SubMenu` with `ParentId` set to the root menu identifier.

## Enabling execution statistics

```csharp
MenuManager manager = MenuManager.Instance;
manager.AreStatisticsEnabled = true;
manager.Start<MainMenu>();
```

Every command then prints `Command finished with status <Status> after <duration>`; failures additionally print the exception message.

## Next steps

- API surface: [`api-reference/INDEX.md`](./api-reference/INDEX.md)
- Execution trace: [`flows/command-dispatch.md`](./flows/command-dispatch.md)
- Safe modification guidance: [`change-guide.md`](./change-guide.md)