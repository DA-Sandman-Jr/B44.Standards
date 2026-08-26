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

Measured before adoption: every consumer already uses bounded internal package
floats; no production project references a test project; no consumer would fail
the always-on checks. Two consumers carry committed debris that the opt-in
hygiene check reports when they enable it, which is the check paying for itself
rather than a migration cost. Two consumers do not yet run warnings-as-errors,
which is why that check is opt-in rather than always on.

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

**Status:** **Planned** since 2026-08-25, after 0.12.0 publishes.

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
- Set `B44EngineFree` on each engine-free test project, replacing the pasted
  `PreventGodotDependencies` target the pre-template repositories carry. One
  repository needs design work before it can adopt this rather than a
  configuration change: `WhispersOfTheEarth.Tests` references the Godot
  presentation project directly, so `GodotSharp` is already in its test
  reference graph and that suite cannot run on an engine-free runner today.
  Moving the tested logic into `Whispers.Core` is the fix; enabling the flag
  first would only turn the build red.

## Known defects

No known defects are currently queued in this repository.
