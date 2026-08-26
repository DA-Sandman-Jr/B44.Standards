# B44.Standards Backlog

Agreed work that has not yet shipped. This public backlog stays intentionally concise; detailed evaluation notes and private consumer sequencing are tracked separately until they become actionable work in this repository.

Cross-repository programs remain single-sourced in [`B44.Common`'s backlog](https://github.com/DA-Sandman-Jr/B44.Common/blob/main/BACKLOG.md).

## Planned work

### Promote naming and structure conventions to shared guidance

**Status:** **Planned** since 2026-08-07.

Evaluate a small set of conventions for namespaces, source-file organization, tests, test doubles, documentation, and review practices. Before adopting one, decide whether it belongs in prose or analyzer policy, measure any new enforcement against active consumers, and define whether adoption is prospective or retroactive.

### Evaluate proposed mechanical guardrails

**Status:** **Partially delivered** in 0.12.0 (2026-08-25); the remainder stays **planned for evaluation** since 2026-08-13.

Investigate guardrails around dependency boundaries, deterministic authority, public contracts, package compatibility, and conformance testing. A proposal becomes planned implementation only after its authority is stable, evidence is objective, the owning repository is clear, and enforcement impact has been measured across active consumers.

#### Adopted in 0.12.0

The first implementation pass took the candidates whose evidence MSBuild or git
already holds, and whose measured impact across the twelve active consumers is
zero or a small, real defect. Each guard is covered by a build-only fixture that
asserts both detection and build failure.

- **Engine boundary, widened and generalized.** The resolved-reference check now
  also covers `GodotPlugins`, Godot source generators, and Unity package modules
  (`Unity.*`), and applies to any project opting into `B44EngineFree`, not only
  a Core. The hand-copied Godot guard target in the game template is retired in
  favour of it.
- **Banned-symbol guard integrity.** A project that declares an engine-free or
  deterministic boundary must actually load a banned-API analyzer and must not
  suppress `RS0030`. The ban lists are otherwise inert with nothing looking
  wrong.
- **Test-infrastructure integrity.** A `*.Tests` project must declare a test
  framework; a Testing Platform project must set
  `TestingPlatformDotnetTestSupport`; VSTest projects get `TreatNoTestsAsError`.
  Deliberately a floor of one discovered test rather than an expected count.
- **Reference policy.** No production reference to a test project; no unbounded
  `*` float on an internal `B44.*` package (an exact pin warns).
- **Repository hygiene** (opt-in). Committed build output, caches, logs,
  backups, merge leftovers, editor lock files, ad-hoc screenshots, and stray
  binaries fail the build. Godot `.uid` and `.import` sidecars, Wavefront
  `.obj` models, and Unity's `Packages/manifest.json` are explicitly never
  flagged.
- **Suppression budget** (opt-in). One integer per repository, over which new
  `#pragma warning disable` / `SuppressMessage` occurrences fail the build.
- **Warning policy** (opt-in). Warnings-as-errors, nullable, analyzers enabled,
  and a bounded project-wide `NoWarn`.
- **Repository-owned banned-symbol lists.** `B44BannedSymbols` registers a
  repository's own list through the mechanism B44.Standards already uses for
  its engine and determinism boundaries, so an architectural rule that can be
  written as an exact set of members is enforced instead of reviewed forever.
  The build rejects a list that does not exist, and one whose name the analyzer
  will never match — both of which are otherwise silently inert.
- **Orphaned sidecars.** A tracked `.uid` or `.import` whose principal file is
  not tracked is debris; every live one must stay committed. Decidable from the
  tracked file list alone, with no engine knowledge.
- **Consumer runsettings that drop the zero-tests floor** (`B44T003`, a
  warning).

Measured before adoption: every consumer already uses bounded internal package
floats; no production project references a test project; no consumer would fail
the always-on checks. Two consumers carry committed debris that the opt-in
hygiene check reports when they enable it, which is the check paying for itself
rather than a migration cost. Two consumers do not yet run warnings-as-errors,
which is why that check is opt-in rather than always on.

#### Deliberately left repository-specific

- **Public and mutation API surface.** A helper made public with one internal
  caller, and mutation APIs left public against an intended boundary, are real
  and worth catching — but the ownership rule is known only inside the
  repository that holds it. Where the rule can be written as an exact member
  list, `B44BannedSymbols` now enforces it; where it cannot, it belongs in that
  repository's own architecture tests. Standards does not cap API size, does
  not internalize APIs by caller count, and does not infer boundaries.
- **Missing Godot `.uid` sidecars.** A new `.cs` committed before the editor
  regenerated its sidecar is a real, recurring failure, but detecting it needs
  the Godot project root and the knowledge that generation requires opening the
  editor — engine-specific, and UIDs must never be fabricated outside Godot to
  satisfy a check. **Delivered in B44.Godot** on 2026-08-26 as
  `reusable-godot-uid-check.yml`, which reports tracked scripts with no
  committed sidecar and never writes one. The general half — orphaned sidecars,
  which need no engine knowledge — shipped here instead, and found seven in
  WhispersOfTheEarth on its first real run.

#### Rejected in this pass

- **Exact whole-repository manifests and content hashing.** Maintenance is
  proportional to churn while the defect rate is not, and the failure mode is a
  stale manifest nobody trusts.
- **A repository-wide `.editorconfig`-style style policy attached to the
  warning checks.** Diagnostics staying visible is the goal; formatting already
  has an owner in `dotnet format`.
- **Per-file suppression baselines.** A single repository integer is cheaper,
  survives file moves, and produces the same review signal.
- **`.obj`, `packages/`, and `Library/` in the hygiene rules.** Measurement
  found real tracked files behind two of them (Unity's `Packages/manifest.json`;
  Wavefront models are plausible in every game repository), and the third cannot
  be distinguished from an ordinary source directory by name alone.
- **Enforcing `TreatWarningsAsErrors` for every consumer.** Two active
  consumers would turn red on upgrade with no defect behind it; that is a flag
  day, not a guardrail.
- **Proving prose guidance complete.** Generated mirrors are verified against
  their source; asserting that hand-authored documentation describes every type
  or matches every count is not mechanically decidable, and a check that
  approximated it would be worse than none.
- **Automated duplicate-code promotion.** A UI scaling function appearing in
  two games is an extraction candidate for a person to judge. Whether two
  near-identical functions are the same concept is a design decision;
  organization guidance now says where to record the candidate.
- **Universal mutation analysis.** The engine-reads-but-does-not-mutate rule is
  enforceable as an exact member list. Inferring mutation boundaries in general
  is not, and would produce a rule nobody could trust.
- **A generalized guardrail framework.** Every check added here reuses a
  mechanism the portfolio already runs — banned symbols, the ratchet's anchor
  pattern, MSBuild items, `git ls-files` — rather than introducing a new
  abstraction over them.

#### Still to evaluate

Sequenced after the first pass has been lived with. Numbering follows the
original review.

2. **Explicit public API manifests.** `Microsoft.CodeAnalysis.PublicApiAnalyzers`
   with `PublicAPI.Shipped.txt` / `PublicAPI.Unshipped.txt` for genuinely
   reusable packages. Confirm the overlap with package compatibility policy and
   whether unreleased packages should participate.
3. **Ambient determinism restrictions.** Baseline and classify `Guid.NewGuid`,
   tick counts, `Stopwatch`, and entropy sources before banning any of them.
4. **Deterministic string and culture semantics.** Measure `CA1309` / `CA1310`,
   then decide whether machine-readable authority can be told from localized
   presentation with acceptably low noise.
5. **Environmental-effect and serialization fences.** Confirm the reusable
   authority boundary before prohibiting filesystem, networking, process, or
   concrete serialization APIs.
6. **Public API technology-leakage scans.** Prototype a recursive public-surface
   check and validate whether an existing analyzer replaces a custom test.
7. **Opt-in exhaustive conformance for designated finite outcomes.**
8. **Deterministic replay and fresh-process conformance.**
9. **Writable static authority-state rejection.**
10. **Foundation scheduler-escape restrictions.** Gather implementation evidence
    before considering an analyzer for `Task.Run`, timers, or `Parallel.*`
    inside the deterministic scheduling boundary.
11. **Minimal-consumer package smoke tests.** Only once packages claim
    independent consumption.
12. **Released-package compatibility validation** (.NET Package Validation /
    ApiCompat). Only once packages have a meaningful compatibility baseline.
13. **Namespace-to-folder enforcement.** After the naming-convention item above
    decides the convention and adoption policy.
14. **Public-type-to-file enforcement.** Same dependency.
15. **Durable save-compatibility fixtures.** Belongs with the consumer that
    ships a durable save contract, not here.

Also confirmed: leave ArchUnitNET as standard infrastructure, paid architecture
or agent-observability tooling, permanent multi-agent evaluator harnesses,
repository-wide mutation testing, generic coverage thresholds, per-story
changed-file allowlists, design-pattern analyzers, and a custom architecture DSL
out of the mechanical roadmap unless substantially new evidence changes the
tradeoff.

Adoption still requires all of the following to be confirmed: a stable B44
authority or compatibility boundary with an identified owner; an objectively
decidable violation with meaningful cost; false positives and exception pressure
low enough to preserve trust; no prescribed implementation and no paid
infrastructure; known analyzer/versioning implications and active-consumer
baselines; and acceptance cases covering passing examples, failing examples, and
the intended escape mechanism.

Because this backlog is public, publish evaluation outcomes at the policy and
product-contract level. Describe a proposed guardrail as preventive policy
rather than evidence of a current weakness. If evaluation uncovers a secret or
an exploitable issue, keep the details out of this backlog and route them
through the appropriate private remediation and disclosure process.

Stop adding mechanical rules when the remaining question is whether a concept
belongs in B44, which system owns it, whether it has earned generalization, or
whether a design abstraction is justified. Those remain semantic decisions.

### Retrofit the 0.12.0 guardrails into existing consumers

**Status:** **Done** on 2026-08-26, except `Continuity`, which is blocked on the
MA0002 decision above. Eleven of twelve consumers are on `0.12.*` and green;
what follows is the record of what the rollout found.

The always-on checks arrive with the float boundary and need no consumer work.
The opt-in ones do, one line each per repository, and two repositories have real
findings waiting for them:

- Enable `B44HygieneEnabled` per repository. `WhispersOfTheEarth` has a
  committed `.trx`, three `.log` files, and a `.user` file; `Continuity` has a
  committed `.user` file. Remove them with `git rm --cached` and extend the
  repository's `.gitignore` in the same change.
- Enable `B44SuppressionBudget` per repository, seeded at each repository's
  current count rather than at zero, so the ratchet starts where the code
  actually is.
- Enable `B44WarningPolicy` where warnings already fail the build. `TicTacHoe`
  and `BeforeForeverAfter` need `TreatWarningsAsErrors` first; treat that as
  their own scheduled work, not a Standards flag day.
- `WhispersOfTheEarth.Tests` supplies its own runsettings for platform pinning,
  so it keeps that file and never gains the zero-discovered-tests floor. Adding
  `<TreatNoTestsAsError>true</TreatNoTestsAsError>` to
  `WhispersOfTheEarth.Tests.runsettings` closes it; `B44T003` warns until then.
- `B44.GameSystems` currently fails `-p:B44AgentSyncVerifyOnly=true` with a
  stale generated `AGENTS.md`. That is the existing synchronization check doing
  its job, not new enforcement; regenerate it in that repository.
- Set `B44EngineFree` on each engine-free test project, replacing the pasted
  `PreventGodotDependencies` target the pre-template repositories carry. One
  repository needs design work before it can adopt this rather than a
  configuration change: `WhispersOfTheEarth.Tests` references the Godot
  presentation project directly, so `GodotSharp` is already in its test
  reference graph and that suite cannot run on an engine-free runner today.
  Moving the tested logic into `Whispers.Core` is the fix; enabling the flag
  first would only turn the build red.

### Decide whether MA0002 belongs in the test severity overlay

**Status:** **Planned** since 2026-08-26, found during the 0.12.0 consumer rollout.

Standards 0.10.x moved Meziantou.Analyzer from 3.0.123 to 3.0.138, which
widened MA0002 to xunit's `Assert.Contains(string, IEnumerable<string>)`
overloads. Eleven of the twelve consumers never noticed, because their test
suites do not use that pattern. `Continuity` does: crossing from `0.6.*` to
`0.12.*` raises 50 MA0002 diagnostics across 25 call sites in nine test files,
and it is the only repository that could not adopt 0.12.0.

Every one of those call sites is an assertion comparing string literals, where
the default comparer is already ordinal. The fix at each site is mechanical and
behaviour-neutral, which is exactly what makes 25 of them the wrong answer: the
question is whether MA0002 is telling a test suite something worth knowing.
`B44.Tests.globalconfig` already relaxes CA1707, CA1816 and CA1861 for tests
on that reasoning.

Decide one of: relax MA0002 in the test overlay (a patch, since it narrows
enforcement); leave it and accept the migration in the one repository that
hits it; or narrow it some other way. Whichever way it goes, `Continuity`
adopts 0.12.0 immediately afterwards — its two real defects are already fixed
and merged, and it is green on `0.6.*`.

This also records a gap in how that analyzer bump was measured: the hard rule
is to measure an analyzer upgrade against every active consumer before
publishing, and three consumers were on `0.6.*` and therefore untested against
3.0.138 at the time.

## Known defects

No known defects are currently queued in this repository.
