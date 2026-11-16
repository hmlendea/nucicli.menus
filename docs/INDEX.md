# Documentation Index

## Repository summary

`NuciCLI.Menus` is a `.NET 10.0` class library published as a NuGet package that provides a synchronous, event-driven menu system for interactive console applications built on `NuciCLI`. The repository contains the library, a unit-test project (`NUnit` + `Moq`), and an architecture-test project (`ArchUnitNET` + Roslyn syntax analysis), plus GitHub Actions workflows for build, security scanning, NuGet packaging, and release publishing.

Scope of this documentation tree: the library's public contracts, internal mechanics, execution flows, lifecycle semantics, error behaviour, and verification strategy. Application-level architecture of consuming host programs is out of scope.

## Root document map

| Document | Purpose |
|----------|---------|
| [`ARCHITECTURE.md`](../ARCHITECTURE.md) | Canonical architecture description: context, components, runtime flow, data architecture, contracts, constraints, extension points. |
| [`README.md`](../README.md) | Capabilities, usage, installation, compatibility, development, and workflow summary. |
| [`SECURITY.md`](../SECURITY.md) | Vulnerability reporting, scope, disclosure policy, automated scanning. |
| [`PRIVACY.md`](../PRIVACY.md) | Data handling: the library processes no personal data and performs no external transmission. |
| [`CONTRIBUTING.md`](../CONTRIBUTING.md) | Contribution workflow, branch policy, test and documentation expectations. |
| [`CODE_OF_CONDUCT.md`](../CODE_OF_CONDUCT.md) | Community conduct standards. |
| [`LICENSE`](../LICENSE) | GNU General Public License v3.0 or later. |

There is no `ROADMAP.md` in this repository; planned work is tracked through GitHub Releases and issues.

## Navigation

### Start here

- [`quick-start.md`](./quick-start.md) — build, run, and consume the library in minutes.
- [`repository-overview.md`](./repository-overview.md) — purpose, scope, entry points, and what the library deliberately does not do.
- [`repository-structure.md`](./repository-structure.md) — project and source layout.

### Deep dive (component ownership)

- [`components/public-api.md`](./components/public-api.md) — `Menu`, `MenuManager` public surface and contracts.
- [`components/command-execution.md`](./components/command-execution.md) — `Command`, `CommandResult`, `CommandStatus`.
- [`components/presentation.md`](./components/presentation.md) — `MenuPrinter` rendering rules.
- [`components/internal-contracts.md`](./components/internal-contracts.md) — `IMenu`, `IMenuManager`.
- [`components/test-projects.md`](./components/test-projects.md) — unit and architecture test projects.

### Flows (debugging)

- [`flows/startup-and-menu-opening.md`](./flows/startup-and-menu-opening.md) — `Start<TMenu>` through the first rendered prompt.
- [`flows/command-dispatch.md`](./flows/command-dispatch.md) — input read, resolution, execution, statistics.
- [`flows/menu-navigation-and-shutdown.md`](./flows/menu-navigation-and-shutdown.md) — `OpenMenu`, `CloseMenu`, root shutdown.

### Behaviour

- [`behaviour/menu-navigation.md`](./behaviour/menu-navigation.md) — observable navigation semantics.
- [`behaviour/command-presentation.md`](./behaviour/command-presentation.md) — help output, unknown commands, statistics.
- [`behaviour/menu-lifecycle.md`](./behaviour/menu-lifecycle.md) — creation, disposal, and event ordering.

### Cross-cutting references

- [`architecture.md`](./architecture.md) — layered elaboration of the root architecture document.
- [`data-model.md`](./data-model.md) — in-memory objects, keys, and ownership.
- [`state-and-persistence.md`](./state-and-persistence.md) — state ownership; explicit absence of persistence.
- [`error-handling.md`](./error-handling.md) — exception taxonomy and propagation rules.
- [`concurrency-and-scheduling.md`](./concurrency-and-scheduling.md) — singleton locking and synchronous execution.
- [`invariants.md`](./invariants.md) — contracts that must not be broken.
- [`configuration.md`](./configuration.md) — absence of configuration; the `AreStatisticsEnabled` switch and menu properties.
- [`logging.md`](./logging.md) — absence of logging infrastructure; console output as the observability channel.
- [`security.md`](./security.md) — threat model and mitigations.
- [`integrations.md`](./integrations.md) — `NuciCLI` and `NuciExtensions` boundaries.
- [`dependencies.md`](./dependencies.md) — runtime, test, and architecture-test packages.
- [`build-and-deployment.md`](./build-and-deployment.md) — local build and the four GitHub Actions workflows.
- [`api-usage-examples.md`](./api-usage-examples.md) — concrete host-application usage.
- [`api-reference/INDEX.md`](./api-reference/INDEX.md) — public type and member reference (library has no HTTP API).
- [`testing.md`](./testing.md) — test strategy, fixtures, and coverage boundaries.
- [`troubleshooting.md`](./troubleshooting.md) — diagnosed failure modes and resolutions.
- [`faq.md`](./faq.md) — frequently raised questions.
- [`change-guide.md`](./change-guide.md) — how to modify areas safely.
- [`design-decisions.md`](./design-decisions.md) — rationale for implemented choices.
- [`ambiguities-and-open-questions.md`](./ambiguities-and-open-questions.md) — observed gaps and unverified assumptions.
- [`documentation-maintenance.md`](./documentation-maintenance.md) — ownership and refresh procedure for this tree.

## Maintenance metadata

| Field | Value |
|-------|-------|
| Last reviewed | 2026-11-16 |
| Version documented | `NuciCLI.Menus` 1.5.8 (`NuciCLI` 3.0.4, `NuciExtensions` 6.0.3) |
| Coverage | Production library, unit tests, architecture tests, CI workflows |
| Known gaps | No coverage of real console I/O; `MenuPrinter` and `MenuManager.TakeCommand` are not exercised by tests |

Also see the companion root [`README.md`](../README.md) for usage and installation, and [`ARCHITECTURE.md`](../ARCHITECTURE.md) for the canonical architecture description.