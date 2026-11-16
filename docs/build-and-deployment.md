# Build and deployment

## Local build

```
NuciCLI.Menus.slnx
├── NuciCLI.Menus/NuciCLI.Menus.csproj              (packable library)
├── NuciCLI.Menus.UnitTests/…csproj                 (NUnit + Moq)
└── NuciCLI.Menus.ArchitectureTests/…csproj         (ArchUnitNET + Roslyn)
```

```bash
dotnet restore
dotnet build --no-restore
dotnet test --no-build --verbosity normal
```

No `release.sh`, `Dockerfile`, or Compose file exists.

## Packaging

`dotnet pack -c Release --output ./nupkg -p:Version=<version>` produces `NuciCLI.Menus.<version>.nupkg`. The package version is normally injected from the release tag, not taken from the `<Version>` property (currently `1.5.7`).

Package identity: `GPL-3.0-or-later`, `IsPackable=true`, `GeneratePackageOnBuild=false`.

## Continuous integration workflows

### `.github/workflows/dotnet.yml` — `.NET`

| Aspect | Detail |
|--------|--------|
| Trigger | `push` and `pull_request` targeting `master` |
| Runner | `ubuntu-latest` |
| Steps | checkout → `actions/setup-dotnet` (10.0.x) → `dotnet restore` → `dotnet build --no-restore` → `dotnet test --no-build --verbosity normal` |
| Effect | Builds and tests all three projects |

Action versions pinned here are older (`actions/checkout@v2`, `actions/setup-dotnet@v1`) than in the other workflows.

### `.github/workflows/security.yml` — `Security`

| Aspect | Detail |
|--------|--------|
| Trigger | `push` and `pull_request` to `master`; weekly schedule `0 0 * * 0` |
| Permissions | `security-events: write`, `contents: read` |
| Steps | checkout@v7 → setup-dotnet@v5 → restore → `dotnet list package --vulnerable --include-transitive` → CodeQL init (csharp) → build → CodeQL analyze |
| Effect | Publishes CodeQL results to GitHub Code Scanning; the vulnerability list step reports to the job log but does not fail the build on findings |

### `.github/workflows/github-release.yml` — `GitHub Release`

| Aspect | Detail |
|--------|--------|
| Trigger | `release: published` |
| Permissions | `contents: write` |
| Steps | checkout@v6 (`fetch-depth: 0`) → setup-dotnet@v5 → derive version from tag (`v` prefix stripped) → restore → `dotnet pack` → SHA256 of the `.nupkg` → `gh release upload --clobber` → append checksum to release notes via `actions/github-script@v7` |
| Effect | Attaches the `.nupkg` to the GitHub release and appends the checksum to its body |

### `.github/workflows/nuget-release.yml` — `NuGet.org Release`

| Aspect | Detail |
|--------|--------|
| Trigger | `release: published` |
| Permissions | `contents: read`, `id-token: write` |
| Steps | checkout@v4 → setup-dotnet@v4 → derive version → restore → `dotnet pack` → `NuGet/login@v1` (OIDC → temporary API key using `secrets.NUGET_USER`) → `dotnet nuget push … --skip-duplicate` |
| Effect | Publishes `NuciCLI.Menus` to nuget.org using trusted publishing; no long-lived API key secret is stored |

## Release procedure

1. Create and publish a GitHub release with a `v`-prefixed tag.
2. The two release workflows run in parallel and independently pack the project.
3. `GITHUB_TOKEN` and the OIDC token are the only credentials involved.

Both release workflows derive the same version string from `github.ref_name`, so a tag such as `v1.5.8` yields package version `1.5.8`. A tag with extra prefixes or suffixes would flow through unvalidated into the package version.

## Verifying a build locally

| Check | Command |
|-------|---------|
| Compilation | `dotnet build --no-restore` |
| Unit and architecture tests | `dotnet test --no-build` |
| Package contents | `dotnet pack -c Release` then inspect `bin/Release/*.nupkg` |
| Vulnerability audit | `dotnet list package --vulnerable --include-transitive` |