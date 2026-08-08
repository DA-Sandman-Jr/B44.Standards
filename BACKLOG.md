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

**Not** a candidate: the acceptance-gate trait and manifest scheme in
`B44.GameSystems`. That exists to report against a gated delivery plan and
would be noise in a repository without one.

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

## Known Defects

No known defects are currently queued in this repository.
