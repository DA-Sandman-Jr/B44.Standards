# B44.Standards Backlog

Agreed work that has not yet shipped. Completed work is removed after its next
published release. Cross-repository programs remain single-sourced in
[`B44.Common`'s backlog](https://github.com/DA-Sandman-Jr/B44.Common/blob/main/BACKLOG.md).

## Planned Work

### 1. Publish the first independent release

**Status:** Blocked on external configuration.

Configure NuGet Trusted Publishing for `B44.Standards` and `B44.Templates`
against this repository's `release.yml`, add the repository `NUGET_USER`
secret, and publish `v0.10.1`. Inspect both package archives before pushing the
tag.

### 2. Move reusable CI consumers

**Status:** In progress. The generated template is pinned; consumer repositories remain.

Pin TicTacHoe, Time Machine Clicker, Whispers, and B44.Godot to an immutable
commit of `.github/workflows/reusable-dotnet-ci.yml` in this repository. Update
the template's `ciRef` default to the same reviewed commit.

### 3. Complete the source-repository cutover

**Status:** Planned after package and workflow validation.

Once the packages restore successfully and one consumer passes on the new CI
pin, remove the extracted Standards, template, fixture, and reusable-workflow
sources from B44.Common. B44.Common then consumes the published Standards
package like every other repository and releases independently.

## Known Defects

### B44.Templates 0.10.0 uses stale package defaults

The published template defaults `B44.Common` and `B44.Standards` to `0.8.*`.
This repository corrects both to `0.10.*`; the fix ships in `0.10.1`.
