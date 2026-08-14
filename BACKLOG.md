# B44.Standards Backlog

Agreed work that has not yet shipped. Cross-repository programs remain
single-sourced in
[`B44.Common`'s backlog](https://github.com/DA-Sandman-Jr/B44.Common/blob/main/BACKLOG.md).

## Planned Work

### S1. Promote the naming and structure conventions to organization guidance

**Status:** **Planned** since 2026-08-07.

`B44.GameSystems` established a set of conventions that currently live only in
its own `CLAUDE.md`. Most are organization-wide and belong in the managed
guidance section so every repository inherits them instead of rediscovering
them. Several were derived by reading what `B44.Common` and `B44.Godot`
already do, so promoting them is largely writing down existing practice.

Candidates for the managed **organization** section:

1. Namespace equals folder, rooted at the assembly name.
2. Tests mirror production *type names*, not production folders, and stay
   flat. A test file is `<TypeUnderTest>Tests.cs`; assembly-wide assertions
   with no single type under test live in `AssemblyBoundaryTests`. This is
   what `B44.Common` already does — four flat test files against nine
   production types across three folders.
3. Test fixtures live in a `Fixtures/` folder beside the tests they serve —
   the one sanctioned exception to organizing by feature rather than by
   technical kind.
4. One public type per file, file named for the type.
5. No project-tracker identifiers — no story, ticket, epic, or gate ids in
   type names, member names, namespaces, folders, file names, doc comments, or
   exception messages. They belong in commit messages, `CLAUDE.md`,
   `BACKLOG.md`, and test trait metadata, all of which can be removed when the
   work they track is finished.
6. Doc comments state design facts, not plans. If a comment needs a ticket to
   make sense, the design fact behind it has not been written down yet.
7. Test doubles are named for their role, per the Meszaros taxonomy: `*Stub`
   for canned data, `*Fake` for a working implementation, `*Spy`, `*Mock`.
   Name for the role the double will have once the surrounding code exists so
   it does not need renaming later.
8. Test method names are two or three underscore-separated parts,
   `Subject_Condition_ExpectedBehaviour`, where the subject may be a method or
   a scenario. Already the practice in `B44.Common` and `B44.Godot`; currently
   undocumented.

Also candidates, and not repository-specific — these are working conventions
intended to make routine changes easier to verify:

9. **Prefer a targeted edit over scripted string replacement for
   context-sensitive changes.** A regex or `.Replace` that matches nothing
   can leave the intended text unchanged without making that obvious. Scripted
   replacement is fine for mechanical repo-wide renames where a follow-up grep
   proves the result.
10. **When a type loses public members, find the assertions that died with
    them.** Removing a member can also remove the tests that expressed its
    contract without identifying which behavioral assertion needs a new home.
    Leftover fixture arguments should be removed because they can still read
    as intent.
11. **Doc comments are load-bearing and unverified.** After a behavioural
    change, sweep the absence-claims near it — "never", "no", "nothing",
    "not invoked". XML `cref`s are compiler-checked under
    `TreatWarningsAsErrors`; prose still requires review for alignment with
    the implementation.

**Not** a candidate: the acceptance-gate trait and manifest scheme in
`B44.GameSystems`. That exists to report against a gated delivery plan and
would be noise in a repository without one. The closing-read rule that governs
it is likewise initiative-specific.

Open questions for whoever picks this up:

- Which of these can be analyzer-enforced rather than left as prose? One type
  per file has an existing StyleCop rule; the rest are likely guidance only.
  Adding enforcement is an enforcement-expanding change, so it needs a new
  pre-1.0 minor version and measurement against every active consumer first.
- `B44.Common`'s `Interfaces/` folder contradicts rule 1's spirit — it groups
  by technical kind and already contains a concrete class. Fix it as part of
  adoption, or record an explicit exception.
- Existing repositories predate several of these. Decide whether adoption is
  retroactive or applies to new code only; a flag day across four repositories
  is probably not worth it.

### S2. Evaluate and confirm proposed mechanical guardrails

**Status:** **Planned for evaluation** since 2026-08-13. The only decision
recorded here is to investigate these candidates. None is approved for
implementation, sequencing, or organization-wide enforcement yet.

`B44.GameSystems` produced a mechanical-guardrails review covering dependency
boundaries, deterministic authority, public contracts, conformance, and
package distribution. Before any proposal becomes planned implementation,
confirm the underlying authority is stable, collect objective supporting
evidence, identify the correct owning repository, and measure the proposed
enforcement against every active consumer as required for an
enforcement-expanding Standards change.

Evaluate the following candidates first because the review considers them
small, objectively decidable, and potentially high-value. That assessment is
a hypothesis to verify, not an accepted priority order:

1. **Production dependency allowlists.** Confirm the intended production
   project/package graph, whether evaluated `ProjectReference` and runtime
   `PackageReference` items provide complete evidence, how test projects and
   approved exceptions are distinguished, and whether the policy belongs in
   Standards or in a GameSystems-owned boundary test.
2. **Explicit public API manifests.** Evaluate
   `Microsoft.CodeAnalysis.PublicApiAnalyzers` and
   `PublicAPI.Shipped.txt` / `PublicAPI.Unshipped.txt` for genuinely reusable
   packages. Confirm how this overlaps with package compatibility policy and
   whether rapidly changing or unreleased packages should participate.
3. **Ambient determinism restrictions.** Baseline and classify uses of
   `Guid.NewGuid`, tick counts, `Stopwatch`, entropy sources, and other
   process- or machine-derived inputs. Confirm which code is authoritative,
   which exact APIs are unsafe there, and how caller-supplied deterministic
   providers remain valid without banning ordinary equality hashing.
4. **Deterministic string and culture semantics.** Measure narrow built-in
   globalization rules such as `CA1309` and `CA1310`, then determine whether
   additional parsing/formatting rules can distinguish machine-readable
   authority from localized presentation with acceptably low noise.
5. **Environmental-effect and serialization fences.** Confirm the reusable
   authority boundary before prohibiting exact filesystem, networking,
   process, environment-state, or concrete serialization APIs. Preserve
   neutral snapshot/restoration contracts and consumer-owned adapters.
6. **Public API technology-leakage scans.** Prototype a recursive public-
   surface check covering parameters, return types, members, arrays, by-ref
   forms, and nested generics. Validate the initial forbidden technology
   families and whether an existing analyzer can replace a custom reflection
   test.

Evaluate these second-wave candidates only after the first group has been
measured in real work and has demonstrated enough value to justify more
machinery:

7. **Opt-in exhaustive conformance for designated finite outcomes.** Confirm
   that specific closed outcome enums benefit from member-by-member semantic
   cases and that a helper can require them without treating every enum as a
   conformance contract.
8. **Deterministic replay and fresh-process conformance.** Select a few
   high-value scenarios and determine whether independent runs, cultures, or
   fresh processes expose defects that banned-symbol rules cannot. Test code,
   rather than production objects, should own any canonical projection.
9. **Writable static authority-state rejection.** Inventory production
   statics, define what constitutes authority state, and confirm that an exact
   exception list can stay small and trustworthy.
10. **Foundation scheduler-escape restrictions.** Gather implementation
    evidence before considering a narrow analyzer for `Task.Run`,
    `Task.Delay`, `TaskFactory.StartNew`, threads, thread-pool work, timers, or
    `Parallel.*` inside the deterministic scheduling boundary. Do not infer a
    general organization-wide concurrency doctrine from this candidate.

Evaluate the following only when the named product boundary actually exists:

11. **Minimal-consumer package smoke tests.** When packages claim independent
    consumption, assess pack-to-local-feed tests that restore, build, and run
    a minimal documented composition using only declared dependencies.
12. **Released-package compatibility validation.** When packages have a
    meaningful compatibility baseline, evaluate official .NET Package
    Validation / ApiCompat and the required suppression and versioning policy.
13. **Namespace-to-folder enforcement.** After S1 decides the convention and
    adoption policy, identify the built-in SDK rule, clean baseline, and
    consumer impact before enabling it.
14. **Public-type-to-file enforcement.** After S1 confirms the convention,
    evaluate existing analyzer support, companion-type exceptions, and the
    migration cost across consumers.
15. **Durable save-compatibility fixtures.** Revisit only in a consumer that
    has shipped a durable save contract. Historical fixtures, malformed and
    future-version behavior, and failure atomicity belong with that
    consumer's real serializer rather than in GameSystems or Standards.

Also confirm the review's recommendation to leave the following out of the
mechanical roadmap unless substantially new evidence changes the tradeoff:

- ArchUnitNET as standard infrastructure, paid architecture/compliance or
  agent-observability tooling, and permanent multi-agent evaluator harnesses.
- Mandatory repository-wide mutation testing, generic line/branch coverage
  thresholds, public-member-to-test-reference counts, and generic story/test
  traceability infrastructure.
- Per-story changed-file allowlists, broad type-name or collection/namespace
  blacklists, Clean Architecture/DDD/design-pattern analyzers, and a custom
  architecture DSL.
- A generic persistence/serialization framework inside GameSystems or a large
  documentation-verification system without a demonstrated failure it would
  prevent.

For each candidate, record an explicit outcome: adopt, narrow, defer until a
named trigger, retain as semantic review, or reject. Adoption requires all of
the following to be confirmed:

- the B44 authority or compatibility boundary is stable and has an identified
  owner;
- the violation is objectively decidable from complete evidence and has a
  meaningful cost or blast radius;
- false positives and exception pressure are low enough to preserve trust;
- the check does not prescribe an unnecessary implementation and can run
  locally without paid infrastructure;
- analyzer/versioning implications and active-consumer baselines are known;
- acceptance cases include passing examples, failing examples, and the
  intended escape or extension mechanism.

Because this backlog is public, publish evaluation outcomes at the policy and
product-contract level. Do not include references to unrelated games or
clients, credentials or secrets, private service details, or actionable
security findings. Describe a proposed guardrail as preventive policy rather
than evidence of a current weakness. If evaluation uncovers a secret or an
exploitable issue, keep the details out of this backlog and route them through
the appropriate private remediation and disclosure process.

Stop adding mechanical rules when the remaining question is whether a concept
belongs in B44, which system owns it, whether it has earned generalization, or
whether a design abstraction is justified. Those remain semantic decisions.

## Known Defects

No known defects are currently queued in this repository.
