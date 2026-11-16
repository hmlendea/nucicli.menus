# Change guide

Purpose: let a future contributor modify the library without breaking contracts that tests or documents rely on.

## Before any change

1. Run `dotnet test` to establish the baseline.
2. Check [`invariants.md`](./invariants.md) for the invariant your change touches.
3. Confirm the change does not add a production package reference; that requires an explicit `ProductionArchitectureTests` update (see [`components/test-projects.md`](./components/test-projects.md)).

## Adding a public member to `Menu`

| Step | Detail |
|------|--------|
| 1 | Add to `IMenu` if it belongs to the member contract (internal interface). |
| 2 | Implement in `Menu`; keep the dispose pattern intact. |
| 3 | If it stores state, document its clearing behaviour in `Dispose(bool)`. |
| 4 | Update [`api-reference/menu.md`](./api-reference/menu.md) and the public member summary in [`api-reference/INDEX.md`](./api-reference/INDEX.md). |

Do not change the constructor ordering: `Created` is raised inside `Menu()`, before derived bodies.

## Adding a menu-manager capability

| Step | Detail |
|------|--------|
| 1 | Add to `IMenuManager` when it is an instance member. |
| 2 | Implement in `MenuManager`; reuse `SwitchToMenu`, not a direct `ActiveMenuId` assignment, so the header and event stay in sync. |
| 3 | Verify the closed-menu case; only `CloseMenu` clears `IsRunning`. |
| 4 | Update [`api-reference/menu-manager.md`](./api-reference/menu-manager.md), the flows under [`flows/`](./flows/), and [`invariants.md`](./invariants.md). |

## Changing rendering

Rendering lives in `MenuPrinter` (internal static). Constraints:

- Keep the column formula if `help` alignment is part of expected output.
- Duration formatting is asserted only in documentation; changing it is a visible behaviour change.
- Any change to status words affects statistics output; `Success`/`Failed`/`Cancelled` are documented in [`behaviour/command-presentation.md`](./behaviour/command-presentation.md).

## Changing error behaviour

| Intent | Action | Risk |
|--------|--------|------|
| Keep the loop alive for all command failures | Catch broadly inside `Command.Execute` and populate `Exception`, making `Failure` reachable | Changes invariants I19/I21 and the documented host guidance |
| Convert unknown commands into an exception | Remove the `TryGetValue` guard | Breaks documented behaviour and tests do not cover it |
| Handle `null` input explicitly | Guard before `TryGetValue` | Requires knowing `NuciCLI`'s `ReadLine` contract |

Any of these changes requires updating [`error-handling.md`](./error-handling.md) and [`behaviour/command-presentation.md`](./behaviour/command-presentation.md).

## Changing registration rules

`Menu.AddCommand` delegates to `Dictionary.Add`. Introducing validation (non-empty name, null action rejection) breaks these verified test expectations:

- `GivenAnUnusedName_WhenRegisteringItTwice_…` includes `""`, `" "`, and `"two words"` as accepted names.
- `GivenANullAction_WhenRegisteringACommand_ThenItsNameIsReserved` requires `null` actions to be accepted.

Update `MenuCommandRegistrationTests` deliberately and record the compatibility decision.

## Adding a production type

- New file, named exactly after the type.
- Exactly one top-level type per file.
- Explicit block namespace: `namespace NuciCLI.Menus { … }`.
- No file-scoped namespace.

These are enforced by `SourceFileConventionTests`.

## Adding a dependency

1. Add the `PackageReference` to `NuciCLI.Menus.csproj`.
2. Add the assembly name to `allowedReferences` in `ProductionArchitectureTests`.
3. Update [`dependencies.md`](./dependencies.md), [`integrations.md`](./integrations.md), [`security.md`](./security.md), and the root `ARCHITECTURE.md` external-dependencies table.

## Adding tests

Follow [`testing.md`](./testing.md). For any behavioural change, add a test that fails before the change and passes after. Where the change affects console output, either introduce a seam first or state explicitly in the change description that the path remains untested.

## Documentation synchronisation checklist

| Changed artefact | Documents to update |
|------------------|---------------------|
| Public API | `docs/api-reference/*`, `docs/components/public-api.md`, `docs/invariants.md`, `README.md` usage snippet if affected |
| Behaviour or ordering | `docs/behaviour/*`, `docs/flows/*`, `docs/invariants.md` |
| Exception semantics | `docs/error-handling.md`, `docs/troubleshooting.md`, `docs/behaviour/command-presentation.md` |
| Dependencies or versions | `docs/dependencies.md`, `docs/integrations.md`, root `ARCHITECTURE.md`, root `PRIVACY.md` |
| Build/workflow files | `docs/build-and-deployment.md`, `README.md` GitHub Actions table |
| Test structure | `docs/testing.md`, `docs/components/test-projects.md` |
| New/removed docs file | `docs/INDEX.md`, and the README `## 📚 Documentation` section |

## Release steps

1. Merge to `master`; the `.NET` workflow builds and tests.
2. Publish a GitHub release with a `v`-prefixed tag.
3. `GitHub Release` and `NuGet.org Release` workflows pack, checksum, upload, and publish.
4. Verify: release asset present with SHA256 in the body, package visible on nuget.org.

See [`build-and-deployment.md`](./build-and-deployment.md).