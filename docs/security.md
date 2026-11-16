# Security

The root [`SECURITY.md`](../SECURITY.md) defines reporting, scope, and disclosure. This document describes the technical security model of the library.

## Attack surface

| Surface | Description | Trust boundary |
|---------|-------------|----------------|
| Console input (`NuciConsole.ReadLine`) | Text typed by the local operator | Untrusted input; used only as a dictionary key |
| Command actions | Arbitrary host delegates | Trusted code, but may throw, block, or produce sensitive output |
| Menu type names | `Type` objects passed by the host to `OpenMenu` | Trusted; reflection is used to construct them |
| Console output | Rendered strings from menus and commands | Written to the local terminal |

## Verified properties

1. **User input is never interpreted.** The command string is used solely as a `Dictionary` key. No path composition, command execution, SQL, format string, or deserialisation occurs. There is therefore no injection vector within the library.
2. **No network, filesystem, process, or registry access** exists in production code. The architecture test restricts assembly references to `System.Runtime`, `NuciCLI`, and `NuciExtensions`.
3. **No credentials, tokens, or personal data** are read, stored, or transmitted. See [`../PRIVACY.md`](../PRIVACY.md).
4. **No deserialisation** of any kind: no serializers are referenced.
5. **Reflection is limited** to `Activator.CreateInstance(menuType, parameters)` with host-supplied types; consumers control the type argument.
6. **No dynamic code generation, `Assembly.Load`, or unmanaged interop.**

## Risks and mitigations

| Risk | Mechanism | Mitigation |
|------|-----------|------------|
| Sensitive data printed to console | `PrintCommandResults` emits `Exception.Message` verbatim; commands print whatever the host writes | Hosts must not place secrets in exception messages or command output; the terminal is inherently local-visible |
| Privileged action from a mistyped command | Exact-match dispatch; no prefix matching | Commands are explicit; hosts should avoid destructive single-letter command names |
| Unbounded memory growth | `menus` grows with each `OpenMenu`; entries are removed only by `CloseMenu`; escaping exceptions leave menus registered | Ensure menus are closed; treat escaping exceptions as fatal |
| Denial of service through a blocking action | Actions run synchronously on the loop thread | Hosts must keep actions short or move blocking work off-thread |
| Reflection abuse | `OpenMenu(Type)` accepts any supplied type | Callers are trusted application code; validate before calling if types originate externally |
| Console escape sequences | Rendered strings are written raw through `NuciCLI` | Host-supplied titles/descriptions are trusted; avoid interpolating untrusted external text into command descriptions |
| Cross-thread state corruption | No locking on `menus` | Use the manager from one thread |
| Finaliser not releasing state | `Dispose(false)` returns early | Dispose menus through the manager; unclosed menus are collected only with the manager |

## Supply chain

- Runtime dependencies are `NuciCLI` 3.0.3 and `NuciExtensions` 6.0.3 (asserted in `NuciCLI.Menus.csproj`; root `ARCHITECTURE.md` and `PRIVACY.md` still cite older versions — see [`ambiguities-and-open-questions.md`](./ambiguities-and-open-questions.md)).
- `.github/workflows/security.yml` runs `dotnet list package --vulnerable --include-transitive` and CodeQL for C# on push, pull request, and a weekly schedule.
- Release workflows use OIDC (`NuGet/login@v1`) for NuGet publishing and `GITHUB_TOKEN` with `contents: write` for release asset upload.

## Not applicable

Authentication, authorisation, session management, transport security, encryption at rest, CSRF, SSRF, and rate limiting are all absent because the library has no service boundary.