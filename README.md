[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/funding)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/nucicli.menus)](https://github.com/hmlendea/nucicli.menus/releases/latest)
[![Build Status](https://github.com/hmlendea/nucicli.menus/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nucicli.menus/actions/workflows/dotnet.yml)
[![NuGet](https://img.shields.io/nuget/v/NuciCLI.Menus)](https://nuget.org/packages/NuciCLI.Menus)
[![License](https://img.shields.io/github/license/hmlendea/nucicli.menus)](https://github.com/hmlendea/nucicli.menus/blob/master/LICENSE)

# NuciCLI.Menus

NuciCLI.Menus is a lightweight .NET menu system for building interactive, command-driven console applications on top of NuciCLI.

## 📑 Table of Contents

- [Capabilities](#capabilities)
- [Usage](#usage)
- [Installation](#installation)
  - [Package Manager Installation](#package-manager-installation)
- [Compatibility](#compatibility)
- [Development](#development)
  - [Requirements](#requirements)
  - [Setup](#setup)
  - [Build](#build)
  - [Test](#test)
  - [Dependencies](#dependencies)
- [GitHub Actions](#github-actions)
- [Project Structure](#project-structure)
  - [Projects and Packages](#projects-and-packages)
- [Architecture](#architecture)
- [Privacy and Data](#privacy-and-data)
- [Documentation](#documentation)
- [Related Projects](#related-projects)
- [Contributing](#contributing)
- [Code of Conduct](#code-of-conduct)
- [Changelog](#changelog)
- [Security](#security)
- [Project Engagement](#project-engagement)
- [License](#license)

## ✨ Capabilities

- Define commands with descriptions and synchronous actions.
- Navigate hierarchical parent and child menus.
- Use automatic `help` and `exit` commands.
- Display optional command execution statistics.
- Subscribe to menu and manager lifecycle events.

## 🚀 Usage

```csharp
using NuciCLI.Menus;

public sealed class MainMenu : Menu
{
\tpublic MainMenu() : base("Main Menu")
\t{
\t\tAddCommand("hello", "Print a greeting", () => Console.WriteLine("Hello from NuciCLI.Menus!"));
\t}
}

MenuManager.Instance.Start<MainMenu>();
```

Type `help` to list commands or `exit` to close the active menu.

## 📦 Installation

[![Obtain it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciCLI.Menus)

### Package Manager Installation

```bash
dotnet add package NuciCLI.Menus
```

Or, via the `Package Manager Console`:
```powershell
Install-Package NuciCLI.Menus
```

## 🧩 Compatibility

| Component | Supported Versions | Notes |
|-----------|--------------------|-------|
| .NET | 10.0 | The package targets `.NET 10.0`. |

## 🛠️ Development

### Requirements

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Setup

```bash
git clone https://github.com/hmlendea/nucicli.menus.git
cd nucicli.menus
dotnet restore
```

### Build

```bash
dotnet build --no-restore
```

### Test

```bash
dotnet test --no-build --verbosity normal
```

### Dependencies

| Package | Version | Scope | Purpose |
|---------|---------|-------|---------|
| `NuciCLI` | `3.0.3` | Runtime | Provides console input, output, colours, and input cancellation. |
| `NuciExtensions` | `6.0.2` | Runtime | Provides extension utilities used by the library. |
| `Microsoft.NET.Test.Sdk` | `18.11.0` | Development | Supports test execution. |
| `Moq` | `4.21.0` | Development | Provides test doubles. |
| `NUnit` | `5.0.0` | Development | Provides the unit-test framework. |
| `NUnit3TestAdapter` | `6.3.1` | Development | Integrates NUnit with the .NET test platform. |

## ⚙️ GitHub Actions

| Workflow | Purpose | What it does |
|----------|---------|--------------|
| [ .NET ](https://github.com/hmlendea/nucicli.menus/blob/master/.github/workflows/dotnet.yml) | Validate changes | Restores dependencies, builds the solution, and runs tests on pushes and pull requests targeting `master`. |
| [ GitHub Release ](https://github.com/hmlendea/nucicli.menus/blob/master/.github/workflows/github-release.yml) | Publish GitHub release packages | Packs the NuGet package, calculates its SHA256 checksum, uploads it to the GitHub release, and updates the release notes. |
| [ NuGet.org Release ](https://github.com/hmlendea/nucicli.menus/blob/master/.github/workflows/nuget-release.yml) | Publish the NuGet package | Packs and publishes the package to NuGet.org when a release is published. |

## 🗂️ Project Structure

The solution contains the library project and its unit-test project.

### Projects and Packages

| Project | Type | Purpose |
|---------|------|---------|
| `NuciCLI.Menus/` | .NET library | Provides the menu and command APIs. |
| `NuciCLI.Menus.UnitTests/` | .NET test project | Verifies menu, command, and manager behaviour. |

## 🏗️ Architecture

NuciCLI.Menus provides a synchronous, event-driven menu manager with hierarchical in-memory menu state and command-driven console presentation.

See the [architecture documentation](./ARCHITECTURE.md) for the system context, principal components, runtime flows, ownership boundaries, dependencies, constraints, and extension points.

## 🛡️ Privacy and Data

NuciCLI.Menus runs entirely within the host application's process, does not collect, transmit, or persist personal data, and has no built-in telemetry, update checks, or external integrations.

For the detailed description of how the application handles privacy and personal data, see [PRIVACY.md](./PRIVACY.md).

## 📚 Documentation

The repository contains the following project documentation:

| Resource | Description |
|----------|-------------|
| [Architecture](./ARCHITECTURE.md) | System context, components, runtime flows, and extension points. |
| [Contributing](./CONTRIBUTING.md) | Contribution workflow and project standards. |
| [Privacy](./PRIVACY.md) | Privacy and personal-data handling. |
| [Security](./SECURITY.md) | Vulnerability reporting and security policy. |

## 🔗 Related Projects

- [NuciCLI](https://github.com/hmlendea/nucicli): core console helpers used by this library.
- [NuciCLI.Arguments](https://github.com/hmlendea/nucicli.arguments): command-line argument handling for the NuciCLI ecosystem.

## 🤝 Contributing

You are welcome to submit any suggestion, feedback, or modification to this project.

When doing so, please:
- Maintain cross-platform compatibility
- Preserve the existing public contract unless a breaking change is intentional
- Submit focused pull requests that conform to the existing code style
- Maintain your branch synchronised with `master`
- Revise the documentation when functionality changes
- Properly test all modifications, including edge cases and error conditions
- Include tests for additional or modified functionality
- Raise a new [issue](https://github.com/hmlendea/nucicli.menus/issues) for problems or suggestions

See the [contributing guidelines](./CONTRIBUTING.md) for details on how to report issues, suggest enhancements, and submit changes.

## 📜 Code of Conduct

See the [Code of Conduct](./CODE_OF_CONDUCT.md) for the standards of conduct expected in all project spaces.

## 📋 Changelog

The project uses GitHub Releases to publish versioned changes. See the [release history](https://github.com/hmlendea/nucicli.menus/releases) for published versions.

## 🔒 Security

NuciCLI.Menus follows a coordinated disclosure policy for vulnerability reporting.

For information on reporting security vulnerabilities, see [SECURITY.md](./SECURITY.md).

## 💝 Project Engagement

Discovered a problem or have a suggestion? [Open an issue](https://github.com/hmlendea/nucicli.menus/issues)!

If you find this project useful, consider [funding it](https://hmlendea.go.ro/funding) or starring ⭐️ it on GitHub!

[![Donate](https://raw.githubusercontent.com/hmlendea/readme-assets/master/donate_generic.png)](https://hmlendea.go.ro/funding)

## 📄 License

This project is being distributed under the `GNU General Public License v3.0` or later.
See [LICENSE](./LICENSE) for additional information.
