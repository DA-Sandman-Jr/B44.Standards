# B44.Standards Backlog

Agreed work that has not yet shipped. This public backlog stays intentionally concise; detailed evaluation notes and private consumer sequencing are tracked separately until they become actionable work in this repository.

Cross-repository programs remain single-sourced in [`B44.Common`'s backlog](https://github.com/DA-Sandman-Jr/B44.Common/blob/main/BACKLOG.md).

Shipped releases and closed evaluations are not open work. They are kept, condensed, under **Closed decisions and portfolio evidence** so a settled question is not re-proposed and so the guardrail measurement record `CLAUDE.md` points at stays findable.

## Planned work

### Promote naming and structure conventions to shared guidance

**Status:** **Planned** since 2026-08-07.

Evaluate a small set of conventions for namespaces, source-file organization, tests, test doubles, documentation, and review practices. Before adopting one, decide whether it belongs in prose or analyzer policy, measure any new enforcement against active consumers, and define whether adoption is prospective or retroactive.

### Evaluate the remaining mechanical guardrails

**Status:** **Planned for evaluation** since 2026-08-13. The first implementation pass shipped in 0.12.0, and the 2026-08-30 evaluations closed items 2, 3, and 4; what follows is the remainder.

A proposal becomes planned implementation only after its authority is stable, evidence is objective, the owning repository is clear, and enforcement impact has been measured across active consumers. Numbering follows the original review.

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

Adoption still requires all of the following to be confirmed: a stable B44 authority or compatibility boundary with an identified owner; an objectively decidable violation with meaningful cost; false positives and exception pressure low enough to preserve trust; no prescribed implementation and no paid infrastructure; known analyzer/versioning implications and active-consumer baselines; and acceptance cases covering passing examples, failing examples, and the intended escape mechanism.

Because this backlog is public, publish evaluation outcomes at the policy and product-contract level. Describe a proposed guardrail as preventive policy rather than evidence of a current weakness. If evaluation uncovers a secret or an exploitable issue, keep the details out of this backlog and route them through the appropriate private remediation and disclosure process.

Stop adding mechanical rules when the remaining question is whether a concept belongs in B44, which system owns it, whether it has earned generalization, or whether a design abstraction is justified. Those remain semantic decisions.

## Known defects

No known defects are currently queued in this repository.

## Closed decisions and portfolio evidence

Not open work. Kept so settled questions stay settled, and so the guardrail evidence referenced from `CLAUDE.md` remains findable. Full rationale and measurements are in each change's commit and release notes.

### Shipped, 0.12.0 through 0.15.1

- **0.12.0 / 0.12.1 — the first mechanical guardrail pass.** Engine boundary widened and generalized (`GodotPlugins`, Godot source generators, `Unity.*`, and any project opting into `B44EngineFree` rather than only a Core); banned-symbol guard integrity; test-infrastructure integrity; reference policy (no production reference to a test project, no unbounded `*` float on an internal `B44.*` package); opt-in repository hygiene, suppression budget, and warning policy; repository-owned `B44BannedSymbols` lists; orphaned `.uid`/`.import` sidecar detection; and `B44T003` for consumer runsettings that drop the zero-tests floor. Each guard ships with a build-only fixture asserting both detection and build failure. The hand-copied Godot guard target in the game template was retired in favour of the generalized check.
- **0.13.0 — an anchorless repository-wide check now fails instead of passing green.** The ratchet, hygiene, and suppression budget each run once per build by comparing an anchor project against the project being built, so an empty anchor silently disabled them and produced a green build that had checked nothing. `B44H003`/`B44H004` and `B44R002`/`B44R003` now report from every project that sees the opt-in; `B44VerifyHygieneAnchor=false` and `B44VerifyRatchetAnchor=false` turn them off. An anchor naming a real project that is simply not in the build stays undecidable and is left alone.
- **0.14.0 — the determinism ban list completed.** Added `DateTime.Today`, `Environment.TickCount`, `Environment.TickCount64`, and the `RandomNumberGenerator` type to the existing `BannedSymbols.Determinism.txt`, closing the accidental route around the `DateTime.Now` and `Random` bans already in place. No new property, mechanism, or framework.
- **0.15.0 — string and culture severities pinned.** `CA1304`, `CA1305`, `CA1309`, `CA1310`, and `CA1311` are now pinned explicitly in `B44.globalconfig` at `warning` — exactly what `AnalysisMode=Recommended` already gave them — so a consumer that sets `AnalysisMode` for an unrelated reason can no longer lose the whole family silently.
- **0.15.1 — extraction policy reconciled.** Shared guidance no longer makes a second consumer a precondition for extracting a bounded reusable capability; generalized infrastructure keeps the higher bar of multiple independent real consumers. Documentation only, no enforcement change, so it rode inside every consumer's existing `0.15.*` float.

**Portfolio evidence.** Every check above was measured against every active consumer before adoption: bounded internal package floats everywhere; no production project referencing a test project; zero `RS0030` across all 26 boundary projects on the determinism additions; zero `CA1304`/`CA1305`/`CA1309`/`CA1310`/`CA1311` diagnostics and zero build failures on the pinned culture severities; and zero new diagnostics on the anchor guards across the three anchor shapes in use. Two consumers carried committed debris that the opt-in hygiene check reported when they enabled it, which is the check paying for itself rather than a migration cost. Two consumers did not yet run warnings-as-errors, which is why that check is opt-in rather than always on.

### Consumer rollout — complete

The 0.12.0 retrofit finished on 2026-08-26, with `Continuity` following on 2026-08-29 once the MA0002 decision below was made. The portfolio then crossed from `0.12.*` straight to `0.15.*` in one deliberate migration, taking the anchor guards, the determinism entries, and the pinned culture severities together rather than crossing three minors one at a time. **All twelve active consumers now float `0.15.*`**, with hygiene, suppression budgets, and the warning policy enabled and seeded per repository. `ThemedWeatherImages` remains on `0.6.*` and outside the active set.

### Evaluated and declined

- **Public API manifests (item 2).** `Microsoft.CodeAnalysis.PublicApiAnalyzers` 4.14.0 was piloted on `B44.Common` and measured across seven libraries: 6,386 committed manifest lines, roughly a third of them compiler-synthesized record members, and a manifest edit on between a third and two thirds of library commits. It catches a real class of mistake — API *widening* is structurally invisible to a test suite — but no accidental public API change appears anywhere in those libraries' history, five of the seven are pre-1.0 where churn is the plan, and bootstrapping depends on an IDE code fix that B44's terminal and agent workflow does not use. **Revisit when** a package gains an external consumer, or at `B44.Common` 1.0 — whichever comes first — and compare against .NET Package Validation (item 12), which diffs against the previously published package and needs no committed manifest.
- **Ambient determinism, beyond the four adopted entries (item 3).** `Guid.NewGuid` (4 boundary uses: log correlation ids, temp-file identity, job identity), `Stopwatch` (6 uses, including measured elapsed time on an authoritative result record, which is a judgement call rather than a decidable violation), and `TimeProvider.System` (5 uses, 4 of them composition roots doing exactly the right thing). The fence is therefore deliberately incomplete: it closes the accidental path — reaching for the nearest ambient number after an existing ban fires — not a determined one.
- **Deterministic string and culture semantics, beyond pinning (item 4).** A collection keyed by `StringComparer.CurrentCulture`, and `ToUpper(CultureInfo.CurrentCulture)` used to normalize a key, both compile clean and always will; each requires someone to name `CurrentCulture` deliberately, and neither is separable from legitimate presentation by any analyzer. Using a string hash as durable identity is out of reach too, and is a persistence-design question rather than a culture one.
- **Rejected in the 0.12.0 pass.** Exact whole-repository manifests and content hashing; a repository-wide `.editorconfig`-style style policy attached to the warning checks; per-file suppression baselines; `.obj`, `packages/`, and `Library/` in the hygiene rules; enforcing `TreatWarningsAsErrors` for every consumer; proving prose guidance complete; automated duplicate-code promotion; universal mutation analysis; and a generalized guardrail framework.
- **Standing "confirmed out".** ArchUnitNET as standard infrastructure, paid architecture or agent-observability tooling, permanent multi-agent evaluator harnesses, repository-wide mutation testing, generic coverage thresholds, per-story changed-file allowlists, design-pattern analyzers, and a custom architecture DSL stay out of the mechanical roadmap unless substantially new evidence changes the tradeoff.

### Deliberately left repository-specific

- **Public and mutation API surface.** A helper made public with one internal caller, and mutation APIs left public against an intended boundary, are real and worth catching — but the ownership rule is known only inside the repository that holds it. Where the rule can be written as an exact member list, `B44BannedSymbols` enforces it; where it cannot, it belongs in that repository's own architecture tests. Standards does not cap API size, does not internalize APIs by caller count, and does not infer boundaries.
- **Missing Godot `.uid` sidecars.** Not a defect, and no longer checked anywhere: only Godot allocates a UID, so an absent sidecar means the editor has not run over that script yet. B44.Godot's `reusable-godot-uid-check.yml` enforced it from 2026-08-26 and was deleted on 2026-08-29 without ever being adopted by a game. The half that needs no engine knowledge — orphaned sidecars — shipped here instead and found seven in `WhispersOfTheEarth` on its first real run. Content validation of a `.uid` was considered and rejected in the same pass.
- **`Whispers.Core/Saves/SessionSaveBuilder`.** Its parameterless constructor defaults to `TimeProvider.System`, and that value is written to `SavedAtUtc` in the save envelope, while that repository's own `CLAUDE.md` says Core receives wall-clock time through an *injected* `TimeProvider`. Found while measuring 0.14.0; the injected overload and a `savedAtUtc` parameter both already exist. A design question for its owner, not a Standards change.

### MA0002 stays enforced in tests

**Decided 2026-08-29.** Meziantou.Analyzer 3.0.138 widened MA0002 to xunit's `Assert.Contains(string, IEnumerable<string>)` overloads, which surfaced at 25 sites across nine `Continuity` test files and made it the only repository that could not adopt 0.12.0. The overlay is not relaxed and no site is suppressed: `B44.Tests.globalconfig` relaxes rules that ask a test suite to look like production API surface, and MA0002 is not one of them — it asks what a string comparison *means*, and a test asserting an identity has the same stake in that answer as the code producing it. All 25 sites turned out to be ordinal and took an explicit `StringComparer.Ordinal` with no behaviour change.

This also recorded a gap in how that analyzer bump was measured: the hard rule is to measure an analyzer upgrade against every active consumer before publishing, and three consumers were on `0.6.*` and therefore untested against 3.0.138 at the time.
