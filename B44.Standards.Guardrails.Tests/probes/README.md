# Guardrail probe projects

Small projects that `B44.Standards.Guardrails.Tests` drives out of process,
asserting each outcome. Nothing here is in the solution.

Most are deliberately non-conforming project files for a check that is an
MSBuild `<Target>` rather than a task. The fixture invokes that one shipped
target, which needs evaluation only, so those probes are never restored or
built — the only way to exercise such a check without a real violation in a
real project. Do not "fix" them; each one is a negative test.

The rest are compiled, because what they prove only exists in a real compile:
the determinism ban, the culture and static-state severities, and the
release-export optimization default. `ExportHost` is also run, to read the
optimization its build actually emitted into `ExportCore` and into itself.
