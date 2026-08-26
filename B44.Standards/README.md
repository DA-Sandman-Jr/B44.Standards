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
- `<B44EngineFree>true</B44EngineFree>` — bans Godot and Unity APIs and fails
  the build if a Godot or Unity assembly, or an engine source generator,
  reaches the resolved reference or analyzer graph. A symbol analyzer cannot
  reject a dependency nothing has used yet, which is why the graph is checked
  directly. The engine list is flat and deliberately short: it names the
  engines B44 actually integrates, and is not an extensible engine-policy
  mechanism. Set it on the engine-free test project too — the suite has to run
  on a machine with no engine installed. A non-engine assembly that collides
  with a prefix (`Unity.Container`, say) is exempted by name with
  `<B44EngineReferenceAllow Include="Unity.Container" />`.
- `<B44EngineFreeCore>true</B44EngineFreeCore>` — the engine-free Core of a game
  or shared package: implies `B44EngineFree` and, because a Core is also the
  deterministic authority, `B44Deterministic`.
- `<B44SecuritySensitive>true</B44SecuritySensitive>` — enables every built-in
  SDK Security rule and pins the rule level to the project's target framework
  (`8.0-all` for `net8.0`, `10.0-all` for `net10.0`). Set this in
  `Directory.Build.props` for public server/function and endpoint-owning projects.

## Verification guardrails

These exist for one reason: each failure they catch otherwise produces a
**green** build. They read evidence MSBuild and git already have, and none of
them needs a manifest of the repository.

Always on, and green across every current consumer:

- **Banned-symbol guard integrity.** A project that opts into `B44EngineFree` or
  `B44Deterministic` must actually have a banned-API analyzer loaded and must
  not suppress `RS0030`. Without the analyzer the ban lists are inert, and
  nothing about the build looks wrong.
- **Test-project integrity.** A `*.Tests` project must declare a test framework
  (VSTest's `IsTestProject`, or `IsTestingPlatformApplication`), and a Testing
  Platform project must set `TestingPlatformDotnetTestSupport=true` — without it
  `dotnet test` exits 0 having run nothing. A build-only MSBuild fixture named
  `*.Tests` declares itself with `B44BuildOnlyFixture=true`.
- **Zero discovered tests is a failure.** VSTest projects without their own
  runsettings get `TreatNoTestsAsError`. It is a floor of one, not an expected
  count, so it never becomes a brittle number to update. Microsoft.Testing
  Platform already enforces the same floor.
- **Reference policy.** Production projects may not reference a `*.Tests`
  project. Internal `B44.*` packages may not use an unbounded `*` version (an
  exact pin is a warning, not an error).

Opt-in, one line each in `Directory.Build.props`:

- `<B44HygieneEnabled>true</B44HygieneEnabled>` — fails the build when git
  tracks generated output, engine/tool caches, logs, backups, merge-conflict
  leftovers, editor lock files, ad-hoc screenshots, or stray binaries and
  archives. Godot `.uid` sidecars, Godot `.import` files, Wavefront `.obj`
  models and Unity's `Packages/manifest.json` are never flagged. Exempt paths
  with `<B44HygieneAllow>tools/*.exe;native/libfoo.so</B44HygieneAllow>`, or
  turn the binary family off entirely with `B44HygieneBinaries=false`. Anchor
  and repository root default to the ratchet's.
- `<B44SuppressionBudget>N</B44SuppressionBudget>` — caps `#pragma warning
  disable` and `SuppressMessage` across production sources. Over budget fails;
  under budget warns that the budget can be lowered. Raising it is a one-line
  diff in a reviewed file, which is the entire point.
- `<B44WarningPolicy>true</B44WarningPolicy>` — asserts that diagnostics stay
  visible: warnings as errors, `Nullable=enable`, analyzers not switched off,
  no `WarningsNotAsErrors`, and no project-wide `NoWarn` outside
  `B44AllowedNoWarn` (default `CS1591`; extend with
  `<B44AllowedNoWarn>$(B44AllowedNoWarn);NU5128</B44AllowedNoWarn>`).

Every check has a named property that turns it off (`B44ReferencePolicy`,
`B44TestProjectIntegrity`, `B44TestRunSettings`, `B44VerifyBannedSymbolGuard`).
Turning one off is an edit to a reviewed file, not a runtime flag.

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
`0.<minor>.*` (the current consumer boundary is `0.12.*`); stable packages use
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
