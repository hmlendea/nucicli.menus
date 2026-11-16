# Documentation maintenance

## Purpose of this tree

`docs/` exists so a future agent or maintainer can work on `NuciCLI.Menus` without re-deriving its behaviour from source. It is deliberately redundant with the code in the sense that every claim names the file that proves it.

## Source-of-truth ordering

When documentation and code disagree, resolve in this order:

1. `NuciCLI.Menus/*.cs` and `NuciCLI.Menus.csproj`
2. Test projects (`NuciCLI.Menus.UnitTests`, `NuciCLI.Menus.ArchitectureTests`)
3. `.github/workflows/*.yml`
4. `docs/`
5. Root `ARCHITECTURE.md`, `README.md`, `PRIVACY.md`, `SECURITY.md`

Root documents are maintained separately and are known to lag the project file (see [`ambiguities-and-open-questions.md`](./ambiguities-and-open-questions.md#a1--documented-dependency-versions-are-stale)).

## When to update which document

| Change | Documents |
|--------|-----------|
| New or changed public member | `api-reference/menu.md`, `api-reference/menu-manager.md`, `api-reference/INDEX.md`, `components/public-api.md` |
| Changed event ordering | `behaviour/menu-lifecycle.md`, `flows/*`, `invariants.md` |
| Changed exception behaviour | `error-handling.md`, `troubleshooting.md`, `behaviour/command-presentation.md` |
| Changed rendering | `components/presentation.md`, `behaviour/command-presentation.md`, `logging.md` |
| New dependency or version bump | `dependencies.md`, `integrations.md`, `security.md`, `components/test-projects.md` (if the whitelist changes) |
| Workflow change | `build-and-deployment.md`, `security.md` |
| Test addition or removal | `testing.md`, `components/test-projects.md` |
| Structural convention change | `components/test-projects.md`, `change-guide.md` |
| Resolved item | Move from `ambiguities-and-open-questions.md` into the relevant reference, and note the resolution |
| New or removed `docs/` file | `INDEX.md`, plus the `## 📚 Documentation` section of `README.md` |

## Review cadence

No automation enforces documentation freshness. Recommended practice:

- Update `docs/` in the same pull request as the behavioural change.
- Re-verify the *Maintenance metadata* table in [`INDEX.md`](./INDEX.md) (last reviewed date, documented version, known gaps) at each release.
- Re-verify `api-reference/*` whenever `Menu.cs` or `MenuManager.cs` changes signature.

## Verification procedure for a docs refresh

```bash
# Confirm the documented facts still hold
dotnet build --no-restore
dotnet test --no-build

# Confirm package metadata claims
grep -E "TargetFramework|Version|PackageReference" NuciCLI.Menus/NuciCLI.Menus.csproj

# Confirm workflows referenced still exist
ls .github/workflows
```

Then re-read each file under `docs/components/` and `docs/flows/` against the corresponding source file.

## Ownership

Documentation is maintained by the project maintainer (`hmlendea`). Contributions follow [`../CONTRIBUTING.md`](../CONTRIBUTING.md).

## Link hygiene

- Cross-references inside `docs/` are relative (`./architecture.md`, `../flows/command-dispatch.md`).
- References to root documents use `../NAME.md`.
- Repository-relative links must resolve before being committed.

## What this tree must not contain

Per the documentation standard for this repository:

- No duplicate of root `ARCHITECTURE.md`, `SECURITY.md`, `PRIVACY.md`, or `LICENSE`.
- No roadmap content (root `ROADMAP.md` does not exist; releases and issues track planned work).
- No invented behaviour: every statement must be traceable to source, tests, or workflow files.