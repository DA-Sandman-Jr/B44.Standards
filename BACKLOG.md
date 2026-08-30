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
  not tracked is debris; a live one stays committed, and a file with no sidecar
  is never reported. Decidable from the tracked file list alone, with no engine
  knowledge.
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
- **Missing Godot `.uid` sidecars.** Not a defect, and no longer checked
  anywhere. Only Godot allocates a UID, so an absent sidecar means the editor
  has not run over that script yet — a normal state that no build or CI job can
  resolve, and one a check could only turn into workflow debt: a red build whose
  single remedy is opening the editor, or a fabricated value that looks
  authoritative and resolves to nothing. B44.Godot's
  `reusable-godot-uid-check.yml` enforced this from 2026-08-26 and was **deleted
  on 2026-08-29** without ever being adopted by a game. The half that stayed is
  the half that needs no engine knowledge: orphaned sidecars, which shipped here
  and found seven in WhispersOfTheEarth on its first real run.

  Content validation of a `.uid` was considered and rejected in the same pass.
  Everything beyond the orphan check — an empty or malformed file, a duplicated
  UID value — needs the file's bytes rather than the tracked path list, plus a
  claim about Godot's UID grammar and uniqueness rules that this repository
  cannot verify and that Godot may change. Orphan detection is where the
  mechanically decidable part ends.

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

2. **Explicit public API manifests.** **Evaluated 2026-08-30 — not adopted.**
   See "Public API manifests: measured and declined" below.
3. **Ambient determinism restrictions.** **Evaluated 2026-08-30 — completed the
   existing ban list, declined the rest.** See "Ambient determinism: what the
   boundary already holds" below.
4. **Deterministic string and culture semantics.** **Evaluated 2026-08-30 — the
   rules were already enforced; pinned them so they stay that way.** See
   "String and culture semantics: already enforced, now unconditional" below.
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

**Status:** **Done** on 2026-08-26; `Continuity` followed on 2026-08-29 once
the MA0002 decision below was made. All twelve consumers are on the `0.12.*`
line and green; what follows is the record of what the rollout found.

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

### An anchorless repository-wide check now fails instead of passing green

**Status:** **Done** on 2026-08-30, in 0.13.0. Found while enabling the opt-in
guardrails in `Continuity`.

The ratchet, repository hygiene, and the suppression budget each run once per
build by comparing an anchor project against the project being built. That is
how "once" is achieved, and it is also how the check disappears: if the anchor
is empty, the comparison matches no project and the target never runs. A
repository that enabled hygiene and a budget got a green build that had checked
nothing, with no message saying so.

`Continuity` was in exactly that state. It sets `B44HygieneProject` from
`B44RatchetProject`, which is what the default does, but it does not run the
ratchet — so the anchor was empty and both checks were inert. It surfaced only
because the budget was seeded one under the real count on purpose and nothing
failed.

Now `B44H003` (hygiene or budget, no anchor), `B44H004` (anchor names a file
that does not exist), `B44R002` and `B44R003` (the same two for the ratchet).
Reported from every project that sees the opt-in, because with no anchor there
is no single project to report from. Off with `B44VerifyHygieneAnchor=false` or
`B44VerifyRatchetAnchor=false`.

Not decidable, and left alone: an anchor naming a real project that is simply
not in the build. No single project's evaluation can see the rest of the build.

Measured before release: all twelve consumers already set a valid anchor, so
nothing turns red. `Continuity`, `B44.Common` and `BookshelfReader` — the three
anchor shapes in use — were rebuilt against the candidate with zero new
diagnostics.

### Keep MA0002 enforced in tests

**Status:** **Done** on 2026-08-29. Raised 2026-08-26 during the 0.12.0
consumer rollout.

Standards 0.10.x moved Meziantou.Analyzer from 3.0.123 to 3.0.138, which
widened MA0002 to xunit's `Assert.Contains(string, IEnumerable<string>)`
overloads. Eleven of the twelve consumers never noticed, because their test
suites do not use that pattern. `Continuity` did: crossing from `0.6.*` to
`0.12.*` raised MA0002 at 25 call sites across nine test files, and it was the
only repository that could not adopt 0.12.0.

**Decided: MA0002 stays enforced in tests.** The overlay is not relaxed and no
site is suppressed. `B44.Tests.globalconfig` relaxes CA1707, CA1816 and CA1861
for tests because those rules ask a test suite to look like production API
surface, which is not a property tests should have. MA0002 is a different kind
of rule: it asks what a string comparison means, and a test asserting an
identity has exactly the same stake in that answer as the code producing it.
Twenty-five mechanical edits is a migration cost paid once, not a reason to
stop asking.

`Continuity` adopted `0.12.*` the same day and is green. All 25 sites turned
out to be ordinal — content-hash ids, canon ids, predicate and language names,
parsed CLI tokens, semantics versions — so each took an explicit
`StringComparer.Ordinal` and no behaviour changed. None wanted culture-sensitive
or case-insensitive semantics, which is the outcome that would have argued for
relaxing the rule instead. Every consumer is now on the `0.12.*` line.

Earlier notes here put the count at 50 diagnostics; that was one build's output
counted twice. It is 25 sites and 25 diagnostics.

This also records a gap in how that analyzer bump was measured: the hard rule
is to measure an analyzer upgrade against every active consumer before
publishing, and three consumers were on `0.6.*` and therefore untested against
3.0.138 at the time.

### String and culture semantics: already enforced, now unconditional

**Status:** **Done** on 2026-08-30, in 0.15.0 (built and measured, not released
— see the coordination note at the end).

The backlog asked whether to enforce `CA1309`/`CA1310`. They were already
enforced, at error severity, in every consumer. What was not sound was *why*
they were enforced.

**What the current policy already catches.** Compiling one construct at a time
into a real deterministic Core (`NowhereToNest.Core`), with that repository's
own settings, every ambient-culture form is a hard build error today:

| Construct | Diagnostic |
|---|---|
| `a.Equals(b)` | `MA0001` + `CA1309` |
| `string.Compare(a, b)` | `CA1309` + `CA1310` |
| `a.CompareTo(b)`, `StartsWith(string)`, `EndsWith(string)` | `CA1310` |
| `a.ToUpper()` / `ToLower()` | `CA1304` + `CA1311` |
| `int.Parse(s)`, `d.ToString()`, `string.Format(...)` | `CA1305` |
| `xs.OrderBy(x => x)`, `Array.Sort(strings)` | `MA0002` |
| `StringComparison.InvariantCulture` / `.CurrentCulture` in equality | `CA1309` |

Explicit ordinal and explicit invariant forms compile clean. `MA0001`/`MA0002`
were already pinned in `B44.globalconfig`; the `CA` rules were on because
`AnalysisMode` defaults to `Recommended`.

**The real gap was the mode, not the rules.** Rebuilding the same probe under
each mode:

| `AnalysisMode` | Culture rules |
|---|---|
| `Recommended` (the B44 default) | `CA1304 CA1305 CA1309 CA1310 CA1311` |
| `All` | the same five |
| `Default` | **none** |
| `Minimum` | **none** |

`B44.Standards.props` only *defaults* `AnalysisMode` to `Recommended`, so a
consumer that set it for an unrelated reason would silently lose the entire
family — and `B44WarningPolicy` would not object, because `B44W004` rejects only
`None` and `AllDisabledByDefault`. The severities are now pinned explicitly in
`B44.globalconfig`, at `warning`, which is exactly what they already were under
`Recommended`. Nothing is escalated; the enforcement simply stops depending on a
property nobody guards.

**Noise and suppression pressure: none, measured.** Across every active
repository there is not one `NoWarn`, `#pragma`, or `SuppressMessage` for
`CA1304`, `CA1305`, `CA1307`, `CA1309`, `CA1310`, `CA1311`, `CA1862`, `CA1866`,
`MA0001` or `MA0002`. All twelve consumers run `TreatWarningsAsErrors` **and**
`B44WarningPolicy`, so the rules have been errors for as long as they have been
on, and nobody has had to argue with one.

The reason the pressure is zero is architectural, not luck: B44 games localize
through Godot's `TranslationServer` behind an engine-free `GameText` adapter, so
player-facing text never travels through .NET culture APIs, and translation keys
are compared with `StringComparer.Ordinal` like any other token. Localized
behaviour stays expressible — these rules ask for the culture to be *named*, not
for it to be invariant — and `CulturePresentation` probes exactly that.
`B44.GameSystems.Inventory.Tests` goes further and runs authority operations
under a foreign culture through an `UnderCulture` helper, asserting the results
are identical.

**Not mechanically decidable, and deliberately left alone.** Two forms compile
clean and always will: a collection keyed by `StringComparer.CurrentCulture`,
and `ToUpper(CultureInfo.CurrentCulture)` used to normalize a key. Both require
someone to name `CurrentCulture` deliberately, and neither is separable from
legitimate presentation by any analyzer. Also out of reach, and not a culture
problem: using a string hash as durable identity. `MA0001` pushes
`a.GetHashCode()` toward an explicit `StringComparison`, but every string hash
is randomized per process, and telling a transient dictionary from a
content-addressed id is a persistence-design question, not a decidable one.

**Measured before release.** All twelve active consumers rebuilt against the
pinned config: zero `CA1304`/`CA1305`/`CA1309`/`CA1310`/`CA1311` diagnostics and
zero build failures. Nothing changes for any consumer today, because all of them
are on `Recommended` already; what changes is that none of them can lose the
family by accident.

**Released** as 0.15.0 on 2026-08-30; the documented consumer boundary and the
template defaults moved to `0.15.*` with it. No consumer has been migrated. The
agreed next step is a single deliberate portfolio migration from `0.12.*`
straight to `0.15.*`, taking the anchor guards (0.13.0), the determinism
entries (0.14.0) and this change together rather than crossing three minors one
at a time. All three were measured as zero-impact on every active consumer.

### Ambient determinism: what the boundary already holds

**Status:** **Done** on 2026-08-30, in 0.14.0 (built and measured, not yet
released — see the coordination note at the end).

The boundary was already there. `B44Deterministic` — implied by
`B44EngineFreeCore` — applies `BannedSymbols.Determinism.txt` to 26 projects,
and that list already bans `DateTime.Now`/`UtcNow`, the `DateTimeOffset`
equivalents, both `System.Random` constructors, and `Random.Shared`. The
question was not whether to build a determinism rule; it was which of the
remaining ambient sources belong in the list that exists.

**What is actually on the boundary.** Thirteen non-test occurrences of ambient
nondeterminism across all 26 projects:

| Site | API | Classification |
|---|---|---|
| `B44.Common` `SystemRandomSource` | `new Random()` ×3 | the sanctioned wrapper the ban points callers to; already banned, already suppressed with `#pragma warning disable RS0030` and paid for by the repository's suppression budget |
| `B44.Common` `StructuredGameLogger` | `Guid.NewGuid` ×2 | log correlation ids — ambient by design, never reaches authoritative state |
| `Continuity` `CanonContinuityChecker` | `Stopwatch` ×3, `GetTimestamp` ×2 | elapsed-time measurement written to an `Elapsed` diagnostic field on a result record |
| `Continuity` `PrologProcessRunner` | `Guid.NewGuid`, `GetTempFileName` | unique temp path for a subprocess — nondeterminism is the point |
| `BookshelfReader` processing service | `Guid.NewGuid`, `Stopwatch` | job identity and timing |
| `ThemedWeatherImages` ×4 | `TimeProvider.System` | composition root and three Functions entry points supplying the system clock to injected consumers |
| `Whispers.Core` `SessionSaveBuilder` | `TimeProvider.System` | a parameterless constructor defaulting to the ambient clock, inside a Core, writing `SavedAtUtc` into the save envelope |

Every one is intentional, diagnostic, or a composition root — except the last,
which is discussed below.

**What the current checks catch.** A probe file was compiled into a real
deterministic Core (`NowhereToNest.Core`), one ambient call at a time:

| Call | Result |
|---|---|
| `DateTime.UtcNow` | rejected (`RS0030`) |
| `new Random()` | rejected (`RS0030`) |
| `DateTime.Today` | **compiled clean** |
| `Environment.TickCount64` | **compiled clean** |
| `Stopwatch.GetTimestamp` | **compiled clean** |
| `TimeProvider.System` | **compiled clean** |
| `Guid.NewGuid` | **compiled clean** |
| `Guid.CreateVersion7` | **compiled clean** |

`DateTime.Today` is the one that matters most: `DateTime.Now` is banned and
`.Today` is the same ambient clock one member over, so the ban as written told a
caller exactly how to route around it. The same is true of `Random.Shared` being
banned while `RandomNumberGenerator` was not — the ban message says "inject an
explicit random source", and the nearest unbanned RNG is one namespace away.

**Adopted — four entries, zero exception pressure.** `DateTime.Today`,
`Environment.TickCount`, `Environment.TickCount64`, and the
`RandomNumberGenerator` type. Each has **zero** uses across all 26 boundary
projects today, so nothing turns red, and no deterministic replayable
computation can legitimately read an ambient local date, an ambient monotonic
counter, or unseedable crypto entropy. This completes a rule that already ships
rather than adding a new one: no new property, no new mechanism, no framework.

**Declined, on measured evidence:**

- **`Guid.NewGuid`** — 4 boundary uses, all legitimate (log correlation ids,
  temp-file identity, job identity). Banning it buys one exception per use and
  decides nothing a reader could not already see.
- **`Stopwatch`** — 6 boundary uses. `Continuity` puts measured elapsed time in
  an `Elapsed` field on a returned record, which is a grey zone rather than a
  decidable violation: diagnostics on an authoritative result. A rule that needs
  that judgement is not mechanical.
- **`TimeProvider.System`** — 5 boundary uses, 4 of them composition roots doing
  exactly the right thing. The honest rule would be "may be read only where it
  is handed to something else", which no symbol list can express.

The fence is therefore deliberately incomplete: a caller blocked on
`Environment.TickCount` can still reach `Stopwatch.GetTimestamp`. What closes is
the accidental path — reaching for the nearest ambient number after the existing
ban fires — not a determined one.

**Observation for `WhispersOfTheEarth`, not a Standards change.**
`Whispers.Core/Saves/SessionSaveBuilder` has a parameterless constructor
defaulting to `TimeProvider.System`, used by the production `FlowCoordinator`
autoload and six test call sites, and the value it produces is written to
`SavedAtUtc` in the save envelope. That repository's own `CLAUDE.md` says Core
receives wall-clock time through an *injected* `TimeProvider`. Whether a
defaulted ambient clock satisfies that is a design question for its owner; the
injected overload and a `savedAtUtc` parameter both already exist. Recorded here
because it was found while measuring, and deliberately not changed.

**Measured before release.** All eleven active consumers were rebuilt against
the candidate list — every boundary project plus `Whispers.Core.Tests`, which is
on the boundary too — with zero `RS0030` and zero build failures.
`ThemedWeatherImages` is on `0.6.*` and outside the active set; its four
`TimeProvider.System` uses are unaffected either way, since that API was not
banned.

**Released** as 0.14.0 on 2026-08-30. The documented consumer boundary and the
template defaults moved to `0.14.*` with it. No consumer has been migrated:
every repository still floats `0.12.*` and crosses to `0.14.*` deliberately,
which is when these four entries begin applying to it.

### Public API manifests: measured and declined

**Status:** **Evaluated** on 2026-08-30, **not adopted**. Revisit only on the
trigger below.

`Microsoft.CodeAnalysis.PublicApiAnalyzers` 4.14.0 was piloted on `B44.Common`
and measured against every reusable B44 library. It works, it catches a real
class of mistake, and it still costs more than the mistake does.

**What it catches that nothing here does.** Three mutations were applied to
`B44.Common` and built against the full current check set — analyzers,
warnings-as-errors, the ratchet, banned symbols, 50 tests:

| Mutation | Current checks | With the analyzer |
|---|---|---|
| `internal` helper made `public` | build and tests green | RS0016 |
| Parameter renamed on a public method | build and tests green | RS0016 + RS0017 |
| Optional parameter added (binary-breaking) | build and tests green | RS0016 + RS0017 |
| Public return type widened to nullable | build and tests green | RS0036 |

The blindness is structural, not a gap in the suites: a test suite exercises the
API it knows about, so *widening* is invisible to it, and a first-party consumer
only notices a *narrowing* when it happens to use the member. That is a genuine
hole.

**What it costs.** Manifest lines needed, and how much of each manifest is
compiler-synthesized record members (`Deconstruct`, `PrintMembers`, `==`,
`ToString`, `Equals`, `GetHashCode`):

| Library | Version | Entries | Synthesized |
|---|---|---|---|
| `B44.GameSystems.Shell` | 0.1.0-alpha.6 | 66 | 7 (11%) |
| `B44.Common` | 0.11.2 | 91 | 14 (15%) |
| `B44.Godot` | 0.3.2 | 112 | 22 (20%) |
| `BookshelfReader` | 3.2.0 | 349 | 47 (13%) |
| `Continuity` | 1.1.10 | 997 | 315 (32%) |
| `B44.GameSystems.OperationCore` | 0.1.0-alpha.6 | 1325 | 415 (31%) |
| `B44.GameSystems.Inventory` | 0.1.0-alpha.6 | 3446 | 1361 (40%) |

6,386 committed lines across seven libraries, a third of them members no one
wrote. Public declarations changed in 8 of `B44.Common`'s last 26 commits, 4 of
`BookshelfReader`'s last 8, and 19 of `B44.GameSystems.Inventory`'s last 30 — so
between a third and two thirds of commits to a library would carry a manifest
edit as well.

**Bootstrapping is not a one-liner from a terminal.** The supported path is the
IDE code fix. Building the 91-line `B44.Common` manifest from diagnostics took
four build-edit passes: RS0016 gives the symbol without nullability, RS0037 then
demands `#nullable enable`, RS0036 asks for annotations it only partly supplies,
and compiler-synthesized record members turn out to need the `~` oblivious
prefix, which no diagnostic prints. B44 works from terminals and agents, not
IDEs, so that path is the normal one here, not the fallback.

**Prerelease is the deciding condition.** Five of the seven libraries are pre-1.0
or alpha, where API churn is the plan rather than the hazard. `B44.Common`'s own
rule is `0.x.y` while the API churns. The shipped/unshipped split assumes a
release ritual — move `Unshipped` into `Shipped` at each publish — which is a new
step in every package's release for packages whose current job is to change
shape. Consumers are all first-party, all on compatibility-bounded floats that
this package already enforces, so a breaking change reaches them at a boundary
they cross deliberately.

**Decision: do not adopt, for any scope.** The guard is real but the mistake is
not recurring: no accidental public API change appears anywhere in the history
of the seven libraries. Adopting would buy protection against a hypothetical at
the price of 6,386 maintained lines and a manifest edit on up to two thirds of
library commits — the opposite of a boring automated failure.

**Revisit when** a package has an external consumer, or at `B44.Common` 1.0 —
whichever comes first. At that point compare against .NET Package Validation
(item 12), which diffs against the previously published package and needs no
committed manifest at all. On the evidence here it is the better-shaped tool for
what B44 actually risks, and item 12's existing condition — a meaningful
compatibility baseline — is the same trigger.

## Known defects

No known defects are currently queued in this repository.
