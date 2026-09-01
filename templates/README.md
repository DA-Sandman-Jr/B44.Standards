# B44 repository bootstrap templates

## Use the template

```bash
dotnet new install B44.Templates
dotnet new b44game -n MyGame
```

That produces a repository that builds, tests, and passes its own gates
immediately: engine-free Core with `B44EngineFreeCore=true`, an equally
engine-free xunit.v3 test project (`B44EngineFree=true`), `B44.Standards`
policy, agent-guidance synchronization, the source-size ratchet, repository
hygiene, a zero suppression budget, warnings-as-errors, a `BACKLOG.md`, an
`.editorconfig` seeded with the portfolio's style baseline, and CI wired to the
shared reusable workflow. A placeholder type and test exist only
to prove the wiring — delete them with your first real commit.

The template parameter defaults and the `B44.Standards` version publish from
the same `v*` tag, so a scaffolded repository always gets a `B44.Standards`
float that actually carries the guardrails its `Directory.Build.props` enables.

Parameters (`dotnet new b44game --help` lists them):

| Parameter | Default | Purpose |
|---|---|---|
| `--standardsVersion` | `0.15.*` | Compatibility-bounded float for `B44.Standards` |
| `--commonVersion` | `0.11.*` | Compatibility-bounded float for `B44.Common` |
| `--targetFramework` | `net8.0` | Core and test target framework |
| `--ciRef` | `9c6faefab05253a4fa1d16b971d37cc8c80bf750` | Reviewed commit for the shared reusable CI workflow |

**Why a separate `B44.Templates` package rather than shipping inside
`B44.Standards`:** the consumption models differ completely. `B44.Standards` is
a `PackageReference` every project restores on every build; templates are
installed once per machine with `dotnet new install`. Bundling them would make
every game download template content it never uses.

**Do not add anything to the template that changes when policy changes.** The
template seeds a repository once; ongoing policy flows from `B44.Standards`.
Anything that would need editing here when a rule changes belongs in the
package instead.

## Loose snippets

Superseded by the template above for new repositories. Kept for retrofitting an
existing repo that predates it.

| File | Goes to | Notes |
|---|---|---|
| `Directory.Build.props` | repo root | Select the `Game` guidance profile, repository-relative sync anchor, mandatory engine-free Core project, and compatibility-bounded B44.Standards version |
| `format.yml` | `.github/workflows/` | dotnet-format gate |
| `build-test.yml` | `.github/workflows/` | Build + test gate; replace `GAME` placeholders; B44 packages restore directly from nuget.org |
| `nuget.config` | repo root | Optional deterministic nuget.org-only package source; no credentials |
| `CLAUDE.skeleton.md` | new root `CLAUDE.md` | Repository-local starter only; B44.Standards inserts and maintains the canonical managed sections |
| `TestProject.godot-guard.snippet.xml` | test csproj | One property (`B44EngineFree`) that hands the engine guard to B44.Standards 0.12.0+; delete any pasted `PreventGodotDependencies` target when applying it |
