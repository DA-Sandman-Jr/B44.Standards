> **Auto-generated from `CLAUDE.md`** — edit the sibling `CLAUDE.md` instead. Direct changes are overwritten by B44.Standards on the next synchronized build.

# B44.Standards — Development Guidelines

<!-- B44 ORGANIZATION GUIDANCE: START -->
## B44 Organization Guidance

- `AGENTS.md` files are auto-generated on build; see the generated header for the source file to edit.
- Before editing or reviewing a file, read and follow every applicable `AGENTS.md` from the repository root through that file's directory. Nearer instructions override broader instructions.
- Analyzer severities live in the `B44.Standards` packaged globalconfig, never in a repository `.editorconfig`. Repository editorconfigs own style and whitespace only; tune analyzer policy upstream in the package.
- Public server/function and endpoint-owning projects set `<B44SecuritySensitive>true</B44SecuritySensitive>` in `Directory.Build.props`; B44.Standards then enables the complete SDK Security category at a target-level-pinned rule set.
- Fix shared behavior in the B44 package that owns it; do not fork or paste a local copy into a consumer repository.
- Use compatibility-bounded floating versions for internal B44 packages in every consumer, including production: pre-1.0 packages use `0.<minor>.*`, while stable packages use `<major>.*`. Package owners bump the excluded boundary for breaking changes, and consumers cross that boundary manually. Never use an unbounded `*`. Enforcement-expanding Standards changes bump the minor version and never enter an existing patch float.
- Treat roughly 350 physical lines as a review warning for production source files. New production files should normally stay at or below 500 lines; files above 650 lines require a clear cohesion-based reason.
- Existing oversized files must not grow unless the same change performs a real extraction and leaves the file smaller. Coordinators coordinate; do not evade the limit with cosmetic partial classes, one-method services, generic utility dumping grounds, or needless factories.
- `B44.Standards` fails the build on drift it can decide mechanically: an engine assembly or source generator reaching an engine-free project, a banned-symbol boundary whose analyzer is missing (which leaves the ban list inert), a `*.Tests` project that would discover no tests, a production reference to a test project, an unbounded `*` float on an internal B44 package, and — where the repository opts in — committed build debris, analyzer suppressions past its budget, and warnings that no longer fail the build. Each check names the property that turns it off; raise a budget or add an exemption in `Directory.Build.props` in the same change that needs it, so the decision is visible in review rather than silent.
- Before automated analyzer fixes, baseline measurement, scripted bulk text rewrites, or consuming a freshly published package, read `.b44/B44.Tooling.md`.
- Godot writes a `.uid` file beside every script as a stable identifier. Commit all of them and never add `*.uid` to `.gitignore`: the project still works locally without them, but references break as soon as it is cloned onto another machine, including a CI runner doing a fresh checkout. Godot generates them for every C# script under the project directory, including engine-free `Core` and test projects it never loads; that is expected and those files are committed too.
- Each repository keeps a root `BACKLOG.md` for agreed-but-not-started work and known defects, with defects in their own section so they stay distinct from planned work. It is authored by hand, never generated and never gated by the build — an empty file written to satisfy a check is worse than no file. Cross-repository programs live once in `B44.Common`'s backlog; a consumer's backlog links to the program and holds only its own share of the work, never a restatement that can drift.
- Isolation is by repository, not by folder. Engine- or framework-coupled adapters live in their own repository and package so engine-free build guards remain literal and release cadences stay independent.
- Keep licensing boundaries explicit. Source governed by terms different from a repository's `LICENSE` belongs behind a separately documented repository/package boundary with its provenance and required notices intact.
<!-- B44 ORGANIZATION GUIDANCE: END -->

This repository owns the policy and bootstrap packages used across B44 Labs
projects. Changes here can alter what fails in every consumer build, so treat
compatibility, diagnostics, and release sequencing as product behavior.

## Hard rules

- Analyzer severities live in `B44.Standards/config/*.globalconfig`, not in
  repository `.editorconfig` files.
- Enforcement-expanding changes require a new pre-1.0 minor version. Patches
  may clarify documentation or fix behavior without widening enforcement.
- Measure analyzer upgrades against every active consumer before publishing.
- `B44.Standards` and `B44.Templates` publish together from one `v*` tag.
- Keep template defaults aligned with the latest published compatibility
  boundary for both `B44.Standards` and `B44.Common`.
- The reusable workflow and template CI reference form one contract; validate
  them together before release.
- Never raise a source-size baseline to accommodate growth. Extract a cohesive
  owner or ask the repository maintainer about a genuine exception.

## Layout

- `B44.Standards/` — build-transitive props/targets, analyzer configuration,
  and canonical organization/game/tooling guidance.
- `B44.Standards.AgentGuidance.Tests/` — build-only integration fixture for
  managed guidance and generated `AGENTS.md` files.
- `B44.Standards.Ratchet.Tests/` — build-only integration fixture for baseline
  generation, exclusions, growth detection, and malformed input.
- `B44.Standards.Guardrails.Tests/` — build-only integration fixture for the
  engine boundary, repository hygiene, and the suppression budget.
- `templates/` — `B44.Templates` plus retrofit snippets.

## Guardrails

Every guard ships as a task with a `FailOnViolation` parameter and a count
output, so its fixture can assert both halves: that it **detects** the
violation, and that it **fails the build** on it. A guard proven only to detect
is a report. When a task must fail under `ContinueOnError`, return `false`
explicitly — a downgraded error is no longer an error by the time MSBuild
computes the task result, and the gate would silently report success.

Adding a check means measuring it against every active consumer first. The
portfolio evidence for the current set is recorded in `BACKLOG.md`.

## Verification

```bash
dotnet build B44.Standards.sln --no-restore
dotnet test B44.Standards.sln --no-build
```

The fixtures assert during `dotnet build`, not `dotnet test`: the solution
carries no xunit project, so a green `dotnet test` here proves nothing on its
own.

After changing an analyzer version, rebuild every consumer with `-t:Rebuild`;
an incremental build may legitimately consider application outputs current
without running the updated analyzer set.
