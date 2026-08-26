# B44.Standards

Shared build policy for B44 Labs repositories. It is published publicly for
credential-free, reproducible restores and demonstrates the standards applied
across the portfolio; it is not intended as a general-purpose framework.

Reference it with `PrivateAssets="all"` and it applies, via buildTransitive
assets:

- SDK analyzers at `AnalysisMode=Recommended`, a curated Meziantou allowlist, and
  banned-API rules — severities in a packaged global analyzer config.
- `CA1502` / `MA0051` complexity and method-length thresholds.
- NuGet vulnerability audit on restore.
- Opt-in synchronization of canonical organization/game guidance into marked
  root `CLAUDE.md` sections, plus recursive sibling `AGENTS.md` generation.

Opt-in flags:

- `<B44Deterministic>true</B44Deterministic>` — bans ambient time/randomness
  (`DateTime.Now`, `new Random()`, …); inject `TimeProvider` / an explicit random
  source instead.
- `<B44EngineFreeCore>true</B44EngineFreeCore>` — additionally bans Godot and
  Unity APIs and fails the build if a Godot or Unity assembly reaches the
  resolved reference graph (implies determinism). The engine list is flat and
  deliberately short: it names the engines B44 actually integrates, and is not
  an extensible engine-policy mechanism.
- `<B44SecuritySensitive>true</B44SecuritySensitive>` — enables every built-in
  SDK Security rule and pins the rule level to the project's target framework
  (`8.0-all` for `net8.0`, `10.0-all` for `net10.0`). Set this in
  `Directory.Build.props` for public server/function and endpoint-owning projects.

Agent guidance synchronization is off unless a repository opts in from its
root `Directory.Build.props`:

```xml
<B44AgentSyncEnabled>true</B44AgentSyncEnabled>
<B44AgentGuidanceProfile>Organization</B44AgentGuidanceProfile>
<B44AgentRepositoryRoot>$(MSBuildThisFileDirectory)</B44AgentRepositoryRoot>
<B44AgentSyncProject>$(MSBuildThisFileDirectory)src\App\App.csproj</B44AgentSyncProject>
```

Use `Game` instead of `Organization` to add the game rules. Set
`B44GameCoreProject` to the mandatory engine-free `*.Core.csproj`, make that
same project the synchronization anchor, and set `B44EngineFreeCore=true` in
its project file. The anchor makes synchronization run once; all paths remain
repository-relative.
Local builds update managed files, while
`-p:B44AgentSyncVerifyOnly=true` validates them without writing.

All B44 repositories, including released and production consumers, reference
internal packages through a compatibility-bounded float. Pre-1.0 packages use
`0.<minor>.*` (the current consumer boundary is `0.11.*`); stable packages use
`<major>.*`. Breaking changes bump the excluded minor or major boundary and
require a deliberate consumer edit. Never use an unbounded `*`. Changes that
expand Standards enforcement bump the Standards minor version rather than
entering an existing patch float.

Synchronization does not traverse common dependency, build-output, coverage,
publish, IDE, or virtual-environment directories, and it never follows directory
reparse points. Repositories can add their own generated or imported subtrees;
paths are interpreted relative to `B44AgentRepositoryRoot` and must remain
inside it:

```xml
<ItemGroup>
  <B44AgentSyncExclude Include="generated-site" />
  <B44AgentSyncExclude Include="generated-assets" />
</ItemGroup>
```

Source available for reference; all rights reserved. See `LICENSE`.
