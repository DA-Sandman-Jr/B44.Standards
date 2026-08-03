# B44.Standards

B44.Standards is the shared engineering-policy layer for B44 Labs projects.
It packages analyzers, deterministic-build rules, architecture checks, and
repository guidance so every consumer receives the same reviewed standards
without copying configuration between repositories.

The repository publishes two packages to [nuget.org](https://www.nuget.org/):

- `B44.Standards` — build-transitive analyzers, severity configuration,
  engine-isolation checks, source-size ratcheting, and opt-in guidance sync.
- `B44.Templates` — a `dotnet new` template that scaffolds a complete B44 game
  repository with an engine-free Core project, tests, CI, and policy wiring.

## Engineering goals

- Keep engine-independent code mechanically isolated from Godot assemblies.
- Make deterministic time and randomness explicit dependencies.
- Expand analyzer enforcement through deliberate version boundaries.
- Prevent large source files from growing while allowing genuine extraction.
- Keep organization and game guidance synchronized without overwriting
  repository-specific instructions.
- Provide reproducible, credential-free package restores and CI validation.

## Using B44.Standards

Reference the package privately so policy assets affect the build without
becoming a runtime dependency:

```xml
<PackageReference Include="B44.Standards" Version="0.10.*" PrivateAssets="all" />
```

The package is conservative by default. Repositories opt into stronger
profiles through MSBuild properties such as `B44Deterministic`,
`B44EngineFreeCore`, `B44SecuritySensitive`, `B44RatchetEnabled`, and
`B44AgentSyncEnabled`. See [the package README](B44.Standards/README.md) for
configuration details.

## Creating a game repository

```bash
dotnet new install B44.Templates
dotnet new b44game -n MyGame
```

See [the template documentation](templates/README.md) for parameters and the
generated layout.

## Repository layout

- `B44.Standards/` — package assets and canonical guidance.
- `B44.Standards.AgentGuidance.Tests/` — build fixture for guidance sync.
- `B44.Standards.Ratchet.Tests/` — build fixture for source-size enforcement.
- `templates/B44.Templates/` — the installable project-template package.
- `.github/workflows/reusable-dotnet-ci.yml` — reusable engine-free .NET CI.

## Verification

```bash
dotnet restore B44.Standards.sln
dotnet build B44.Standards.sln --no-restore
dotnet test B44.Standards.sln --no-build
```

The build fixtures exercise the exact MSBuild assets shipped in the package.
The repository also applies its own policy locally before publishing it.

## Versioning and release

Both packages share a version because policy and bootstrap defaults change
together. Pre-1.0 consumers use `0.<minor>.*`; changes that expand enforcement
or break a contract move to a new minor boundary. A `v*` tag runs the release
workflow and publishes through NuGet Trusted Publishing with OIDC.

## License

Source available for reference; all rights reserved. See [LICENSE](LICENSE).
