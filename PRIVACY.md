# Privacy and Personal Data

`NuciCLI.Menus` is a .NET library for building interactive, command-driven console applications. It runs entirely within the host application's process, does not collect, transmit, or persist personal data, and has no built-in telemetry, update checks, or external integrations.

## 📑 Table of Contents

- [What This Document Covers](#what-this-document-covers)
- [Self-Hosted Deployments](#self-hosted-deployments)
- [Data We Handle](#data-we-handle)
  - [Data Provided to the Application](#data-provided-to-the-application)
  - [Data Generated or Collected by the Application](#data-generated-or-collected-by-the-application)
  - [Data Received from Integrations](#data-received-from-integrations)
- [Processing and Use](#processing-and-use)
- [Storage, Retention, and Deletion](#storage-retention-and-deletion)
- [External Processing and Integrations](#external-processing-and-integrations)
- [Data Protection and Security](#data-protection-and-security)
- [Document Changes](#document-changes)
- [Contact](#contact)

## 🔎 What This Document Covers

This document describes how `NuciCLI.Menus` at https://github.com/hmlendea/nucicli.menus handles personal data. It covers the library behaviour and verified integrations described below. Where the software is self-hosted, the instance operator may have separate responsibilities described below.

## 🏠 Self-Hosted Deployments

`NuciCLI.Menus` is a library consumed by a host console application. The library itself does not deploy, host, or operate any service. The host application operator controls the process, console, configuration, local storage, logs, backups, access controls, retention, and any request handling. The library sends no data to project maintainers or external services. It has no telemetry, update checks, crash reports, email, authentication, reverse-proxy, object-storage, or monitoring integrations.

## 📥 Data We Handle

### Data Provided to the Application

The library does not request, collect, or process personal data from users, administrators, or connected systems. Command names, descriptions, and host-provided `Action` delegates are application-defined and not treated as personal data by the library.

### Data Generated or Collected by the Application

The library generates no personal data. It creates transient in-memory objects for menu identifiers, command registrations, and command execution timing (`CommandResult`). These objects contain no personal data and exist only for the duration of the menu session.

### Data Received from Integrations

The library has no built-in integrations and receives no data from third parties.

## 🧭 Processing and Use

The library processes the transient in-memory state described above for these verified functions:
- Menu hierarchy management — menu identifiers and parent-child relationships
- Command registration and lookup — command names and descriptions provided by the host
- Command execution timing — start time, end time, duration, and cancellation status
- Console presentation — menu titles, prompts, command lists, and optional statistics output

## 🗄️ Storage, Retention, and Deletion

All library state is stored in process memory:
- Menu registry: `Dictionary<string, Menu>` in `MenuManager`, cleared when menus are closed and disposed
- Command registry: `Dictionary<string, Command>` in each `Menu`, cleared during menu disposal
- Command results: `CommandResult` instances created per execution and discarded after optional printing

No data is persisted to disk, databases, caches, backups, or configured storage services. When a menu is closed, it is removed from the registry and disposed, clearing its commands. When the root menu is closed, the manager loop ends and all transient state is released. The host application operator controls the process lifetime and any application-level data.

## 🔗 External Processing and Integrations

The library has no built-in external data transfers. It depends on:
- `NuciCLI` 3.0.1 — console input, output, colours, and input cancellation (in-process)
- `NuciExtensions` 5.3.1 — string reversal for title decoration (in-process)
- .NET 10.0 runtime — reflection, collections, events, disposal, and timing (in-process)

No data is sent to these dependencies beyond in-process method calls.

| Service or integration | Purpose | Data involved | Configuration or documentation |
|-----------------------|---------|---------------|--------------------------------|
| `NuciCLI` | Console I/O and cancellation | In-process console interaction | https://github.com/hmlendea/nucicli |
| `NuciExtensions` | String reversal for title rendering | In-process string manipulation | https://github.com/hmlendea/nuciextensions |
| .NET 10.0 runtime | Execution platform | In-process runtime services | https://dotnet.microsoft.com |

## 🛡️ Data Protection and Security

The library implements no data protection mechanisms because it handles no personal data. It uses standard .NET in-process memory management. The host application operator is responsible for process security, console access, secrets management, and any application-level data protection. The original maintainers are not responsible for changes, modifications, or privacy issues introduced by downstream forks.

## 🔄 Document Changes

Update this document when library data flows, storage, integrations, or deployment responsibilities change. The current version is published at https://github.com/hmlendea/nucicli.menus/blob/master/PRIVACY.md.

## 📬 Contact

For questions about library data handling, contact the project maintainers at https://github.com/hmlendea/nucicli.menus. For a host application, contact the application operator. Do not send passwords, access tokens, or other secrets.