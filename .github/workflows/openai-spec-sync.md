---
description: "Monthly snapshot of the OpenAI REST specification, with an analysis of what changed"
labels: ["automation", "openai-spec"]

on:
  schedule:
    - cron: "0 3 2 * *"
      timezone: "America/Los_Angeles"
  workflow_dispatch:
    inputs:
      source_ref:
        description: "Upstream openai/openai-openapi ref, branch, or SHA to snapshot (defaults to main)"
        required: false
        default: ""
        type: string
      force:
        description: "Process the specification even when its content is unchanged"
        required: false
        default: false
        type: boolean
      dry_run:
        description: "Write the results to a temporary directory and do not open a pull request"
        required: false
        default: false
        type: boolean

engine: copilot
runs-on: ubuntu-latest
timeout-minutes: 30

permissions:
  contents: read
  copilot-requests: write

network:
  allowed:
    - defaults
    - github

tools:
  bash: ["cat", "ls", "head", "tail", "wc", "grep"]

steps:
  - name: Setup .NET
    uses: actions/setup-dotnet@c2fa09f4bde5ebb9d1777cf28262a3eb3db3ced7 # v5.2.0
    with:
      # Automatically read .NET SDK version from global.json to stay in sync
      global-json-file: global.json

  - name: Sync the OpenAI specification
    id: sync
    shell: pwsh
    env:
      GH_TOKEN: ${{ secrets.GITHUB_TOKEN }}
      SOURCE_REF: ${{ inputs.source_ref }}
      FORCE: ${{ inputs.force }}
      DRY_RUN: ${{ inputs.dry_run }}
    run: |
      $syncArgs = @()

      if ($env:SOURCE_REF) { $syncArgs += @("-SourceRef", $env:SOURCE_REF) }
      if ($env:FORCE -eq "true") { $syncArgs += "-Force" }
      if ($env:DRY_RUN -eq "true") { $syncArgs += "-DryRun" }

      ./scripts/Sync-OpenAISpec.ps1 @syncArgs

  - name: Record whether there is anything to analyze
    id: gate
    shell: bash
    env:
      CHANGED: ${{ steps.sync.outputs.changed }}
      DRY_RUN: ${{ inputs.dry_run }}
    run: |
      if [ "$CHANGED" = "true" ] && [ "$DRY_RUN" != "true" ]; then
        printf 'proceed\n' > "${RUNNER_TEMP}/spec-sync-status.txt"
      else
        printf 'nothing-to-do\n' > "${RUNNER_TEMP}/spec-sync-status.txt"
      fi

      cat "${RUNNER_TEMP}/spec-sync-status.txt"

  - name: Publish the dry run results
    if: steps.sync.outputs.changed == 'true' && inputs.dry_run == true
    uses: actions/upload-artifact@ea165f8d65b6e75b540449e92b4886f43607fa02 # v4.6.2
    with:
      name: openai-spec-sync-dry-run
      path: ${{ steps.sync.outputs.output-path }}
      retention-days: 14

  - name: Skip the analysis when there is nothing to report
    if: steps.sync.outputs.changed != 'true' || inputs.dry_run == true
    shell: bash
    run: |
      echo '{"type":"noop","message":"The OpenAI specification snapshot did not change, or this was a dry run. No pull request is needed."}' >> "$GH_AW_SAFE_OUTPUTS"

safe-outputs:
  create-pull-request:
    title-prefix: "[spec] "
    labels: [automation, openai-spec]
    draft: false
    base-branch: main
    if-no-changes: "ignore"
    max-patch-files: 400
    max-patch-size: 10240
---

# Monthly OpenAI Specification Sync

A deterministic step has already run before you. It downloaded the latest OpenAI REST API
specification, split it into per-feature specifications, validated that each one stands alone,
generated a structural diff report, and only then moved last month's snapshot into
`specification/openai/previous/` and published the new one into `specification/openai/current/`.
Every file it produced is already in the working tree. Your job is to explain what changed.

## First, check whether there is anything to do

Before reading anything else, run:

```
cat ${RUNNER_TEMP}/spec-sync-status.txt
```

If it says `nothing-to-do`, then the snapshot did not change this month, or this was a dry run.
Emit a `noop` safe output saying so and stop. Do not read the report, do not write a summary, and do
not create a pull request. A month where upstream was still is a successful month, not a problem to
investigate.

Only continue past this point if it says `proceed`.

Note that "nothing changed" here means the *generated snapshot* did not change, which is not quite
the same as upstream being untouched. An upstream edit that lands entirely in a path the split
excludes, or in documentation keys that get stripped, produces byte-identical output and correctly
results in no pull request.

## How the pull request gets made

You do not stage files, create a branch, or push. When you finish, the safe-outputs step collects
whatever changed in the working tree, creates the branch, and opens the pull request with the body
you write. You have no write access to the repository, and the workflow gives you a read-only shell,
so the pull request is the only thing you produce.

There is a second guard behind that one. The pull request step is configured to do nothing when the
working tree has no changes, so even a run that reaches you by mistake cannot open an empty or
timestamp-only pull request.

That means the deterministic step is the sole author of everything under `specification/openai/`.
Do not regenerate, reformat, or hand-edit any of it. Those files are the month-over-month comparison
baseline and must stay byte-for-byte as the tool emitted them. If something looks wrong with them,
say so in the pull request body rather than fixing it.

## What to read

1. `specification/openai/current/diff-report.md` — the structural diff between last month's
   snapshot and this one.

2. `specification/openai/current/.spec-metadata.json` — the provenance of this snapshot: the source
   URL, the content hash, and the upstream commit when one was resolved.

3. `.github/skills/ingesting-spec/` — the repository's existing process for ingesting specification
   changes into TypeSpec. Use its area names and vocabulary so the report reads the way maintainers
   already work.

## What to write

Open a single ready-for-review pull request whose body covers the following.

- A short summary of the sync: which source was snapshotted, the upstream commit or content hash,
  and the overall shape of the change.

- The most significant changes, grouped by feature area. Lead with anything that affects the public
  surface: new or removed operations, new or removed schemas, changed required properties, changed
  enum values, and changed types. Skip pure description edits unless they change meaning.

- A confidence assessment for each significant change, rated HIGH, MEDIUM, or LOW, describing how
  clear the TypeSpec update would be. HIGH means the change is mechanical. LOW means it needs a
  human decision.

- Any operation the tool could not assign to a feature area. These show up in the `UNASSIGNED paths`
  section near the top of the diff report and usually mean upstream added a path or tag the split
  does not know about. That section labels each one `new`, `unchanged`, or `resolved`. Lead with the
  new ones, since those are the ones needing a decision; mention the unchanged ones only briefly, as
  they are known gaps that have already been reviewed. Propose where a path belongs, but do not
  change the split configuration yourself.

- A link to `specification/openai/current/diff-report.md` for the full detail.

Keep the body scannable. A maintainer should be able to read it in a couple of minutes and know
whether this month needs their attention.
