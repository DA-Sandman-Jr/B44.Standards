# Guardrail probe projects

Deliberately non-conforming project files. `B44.Standards.Guardrails.Tests`
invokes one shipped verification target against each and asserts the outcome —
the only way to exercise a check that is an MSBuild `<Target>` rather than a
task, without a real violation in a real project.

Nothing here is in the solution and nothing here is ever built or restored: the
fixture runs a single named target, which needs evaluation only. Do not "fix"
these files; each one is a negative test.
