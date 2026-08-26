# GameName — Development Guidelines

Repository-local guidance. The managed B44 organization and game sections are
inserted and kept current by `B44.Standards` on every build — do not hand-edit
them, and do not edit `AGENTS.md` at all; it is generated from this file.

## What this game is

_One paragraph: the pitch, the platform, the state of it. Replace this._

## Layout

- `GameName.Core/` — engine-free. All rules, state, algorithms, and anything
  worth testing. Sets `B44EngineFreeCore=true`; a Godot reference here fails
  the build.
- `GameName.Tests/` — xunit.v3 over Core only. Sets `B44EngineFree=true`, so an
  engine dependency reaching the test graph fails the build. The suite must run
  on a machine with no engine installed; CI proves it by using a runner that has
  none.
- Godot project at the repository root once it exists; scene controllers stay
  thin and translate at the boundary.

## Build guardrails

Beyond the ratchet below, `B44.Standards` fails this repository's build on:

- an engine assembly or source generator reaching an engine-free project;
- a banned-symbol boundary whose analyzer is missing, which would leave the ban
  list silently inert;
- a `*.Tests` project that declares no test framework, or a Testing Platform
  project missing `TestingPlatformDotnetTestSupport` — both make `dotnet test`
  report success without running anything;
- generated output, logs, or editor debris committed to git;
- a new analyzer suppression past `B44SuppressionBudget`;
- warnings not treated as errors, or a project-wide `NoWarn` outside
  `B44AllowedNoWarn`.

Raising a budget or adding an exemption is a one-line change in this file's
sibling `Directory.Build.props` — deliberately visible in review rather than
silent.

## Ratchet

`ratchet-baseline.txt` records every production file over 350 lines. A violation
fails **the build**, not just the suite. Regenerate only in the same change that
performs a real extraction:

```bash
dotnet build GameName.Core/GameName.Core.csproj -t:B44WriteRatchetBaseline
```

An entry's trailing `# reason` comment survives regeneration. When the build
fails on the ratchet the fix is the extraction — do not split a file in a way
that leaves the code worse, and do not raise a baseline entry to fit. If a file
genuinely warrants an exception, stop and ask the repository maintainer; **an
agent must never grant itself one.**

## Tests

```bash
dotnet test
```
