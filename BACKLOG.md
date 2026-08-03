# B44.Standards Backlog

Agreed work that has not yet shipped. Cross-repository programs remain
single-sourced in
[`B44.Common`'s backlog](https://github.com/DA-Sandman-Jr/B44.Common/blob/main/BACKLOG.md).

## Planned Work

### 1. Finish reusable CI consumer migration

**Status:** **In progress.** The reusable workflow is live at an immutable
commit in this repository. The generated template uses that pin, and the
B44.Godot canary passed and merged in
[PR 17](https://github.com/DA-Sandman-Jr/B44.Godot/pull/17).

Move the remaining workflow calls in TicTacHoe, Time Machine Clicker, and
Whispers from `B44.Common` to the reviewed `B44.Standards` commit. Each change
must pass its repository's pull-request CI before merge.

## Known Defects

No known defects are currently queued in this repository.
