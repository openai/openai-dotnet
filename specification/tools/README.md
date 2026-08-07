# OpenAI Specification Tooling

This folder holds the tooling that produces the OpenAI REST API specification snapshots under
`specification/openai/`:

| Item | What it is |
|---|---|
| `OpenAI.SpecProcessor/` | The console tool that downloads, sanitizes, splits, validates, and compares the specification. |
| `OpenAI.SpecProcessor.Tests/` | Its test suite, including the determinism and publication-gate regressions. |
| `openai-spec-processor.json` | Processor configuration: the source URL, the feature area map, and the exclusion list. |
| `taxonomy-exceptions.json` | Declared, deliberate divergences between the feature area map and the TypeSpec folders. |

The orchestrator that drives the tool, `scripts/Sync-OpenAISpec.ps1`, is shared by local runs and
the `Monthly OpenAI Specification Sync` workflow, so both behave the same way.

This is maintainer tooling. Everything below assumes commands are run from the repository root.

## Taking a snapshot

The `specification/openai/` folder holds a snapshot of the OpenAI REST API specification, split into
per-feature specifications, along with a report describing what changed since the previous snapshot.
The `Monthly OpenAI Specification Sync` workflow refreshes it on the second of each month and opens a
pull request when upstream has changed.

To take a snapshot yourself, whether to test the tooling or to get an advance look at a change that
has not shipped yet:

```powershell
# Preview the current upstream specification without modifying the committed snapshot
./scripts/Sync-OpenAISpec.ps1 -DryRun

# Preview a specific upstream branch, tag, or commit
./scripts/Sync-OpenAISpec.ps1 -SourceRef "some-feature-branch" -DryRun

# Refresh the committed snapshot
./scripts/Sync-OpenAISpec.ps1
```

A dry run writes to a temporary directory and reports where, leaving `specification/openai/` alone.
The staged output of a *successful* dry run is left in place on purpose, because that output is the
whole point of the run. Any failed run cleans its staging directory up, dry or not, so a broken
experiment cannot pile up in temp or be mistaken later for a valid snapshot.

The same workflow can also be started from the Actions tab with the `source_ref`, `force`, and
`dry_run` inputs, which map to the parameters above.

Do not hand-edit the files under `specification/openai/`. They are the baseline the next month's
comparison is made against, and the tool must be the only thing that writes them.

## What has to be true before a snapshot is published

Every path that can publish, the monthly run, a local refresh, a forced run, and a run with
`--no-diff`, goes through the same gate: the processor's exit code. Processing writes to a staging
directory first, and `current/` and `previous/` are only touched after the processor has exited
zero. A non-zero exit stops the run before rotation, so both committed snapshots are left exactly as
they were.

Three things make the exit code non-zero, and each of them is a case where publishing would be worse
than failing:

- **A feature specification that is not self-contained.** A dangling `$ref` means the split dropped
  something it should have carried, and being self-contained is the one promise the split exists to
  make. This used to print `ISSUES` and exit zero, which meant the broken file was published and
  became the baseline.

- **A split that produced no feature areas.** A document can parse cleanly and still not be the
  specification, which is what a source URL quietly serving the wrong file looks like. Publishing it
  replaces a good baseline with an empty one.

- **Any failure during download, sanitization, parsing, splitting, validation, report generation, or
  metadata writing.** The staging directory is discarded and the committed snapshots never move.

## How the feature areas are decided

The snapshot is split into the same feature areas the SDK is organized around, which are the folders
under `specification/base/typespec`. The processor's own map lives in `FeatureAreaConfig`, and the two
are expected to agree.

Neither one is subordinate to the other. The folders are what the code is organized around; the area
map is what the snapshot and the change report speak in. A test asserts they match, so a rename or a
regrouping on either side surfaces as a reviewed change rather than a quiet divergence.

Deliberate divergence is allowed, but it has to be declared in
`specification/tools/taxonomy-exceptions.json` with a reason. Two entries are there today:
`completions`, which is excluded from the snapshot by decision, and `streaming`, which holds shared
constructs rather than a REST surface of its own. Adding an entry is how you say "this one is on
purpose"; a second test checks that every declared exception still names something real, so the list
cannot rot into silently suppressing a check nobody decided to suppress.

## What decides whether a run does anything

Two things, and either one moving is a reason to run:

- **The source content.** The SHA-256 of the downloaded specification, compared against
  `sourceContentHash` in `specification/openai/current/.spec-metadata.json`. An upstream commit that
  touches other files moves the commit SHA without moving the specification, and that should not
  produce a pull request.

- **The processing behavior.** A fingerprint of everything about the tool that could change the
  output from identical input: the feature map, the exclusion rules, the sanitizer version, and the
  comparison scope version, recorded as `processingIdentity`. Comparing source bytes alone answers
  "did upstream change" while quietly assuming the answer to "would we produce the same thing
  anyway". That assumption fails the month someone edits the feature map: upstream is still, the run
  skips, and the snapshot goes on describing itself with a taxonomy the tool no longer uses.

Most of the fingerprint is derived rather than declared, because a number someone has to remember to
bump is a number that will eventually be wrong. Adding a feature area or editing an exclusion changes
it on its own. Two hand-maintained constants cover what cannot be derived,
`ProcessingIdentity.CurrentBehaviorVersion` for the split and diff logic and `SpecSanitizer.Version`
for the source repairs. Bump those when a change would produce different output from the same input.

Run `dotnet run --project specification/tools/OpenAI.SpecProcessor -- identity` to see the current
fingerprint and its components.

The snapshot metadata carries a `schemaVersion`. If it is missing, malformed, or newer than the
tooling understands, the run stops rather than guessing, because a hash that cannot be read is not
the same as a hash that does not match, and treating it as a mismatch would rotate a good baseline
out of the way on the strength of a corrupt file. A metadata file that is simply absent is the
ordinary first-run case and is not an error.

Metadata written before `schemaVersion` existed reads back as legacy rather than as corrupt, and
enum values recorded as numbers by an older writer are still read correctly. That matters because
the baseline has to outlive the exact build that produced it: a snapshot readable only by its own
writer would strand the comparison the first time the tooling changed. The persisted numeric values
are fixed and must not be renumbered or reordered, since an old file would keep deserializing
successfully and mean something different.

## What decides whether a pull request is opened

Passing the check above starts a run. It does not commit to publishing one, because those are
different questions and conflating them is how a diagnostic run turns into noise.

After processing, the staged output is compared against the committed snapshot: every generated
feature file, and the metadata with the run timestamp removed. If they match, nothing is rotated,
nothing is published, and the workflow reports no change. That covers two cases at once. A forced
run, which bypasses the input check on purpose, cannot open a pull request whose entire content is a
moved timestamp. And an upstream edit landing entirely in an excluded path or a stripped
documentation key produces byte-identical output and correctly results in no pull request, even
though the source hash moved.

The workflow enforces the same thing at its own boundary. The agent is told to check a status marker
first and stop without reading anything if there is nothing to report, and the pull request step is
configured to do nothing when the working tree is unchanged. So a pull request requires the generated
files themselves to have moved.

## How a gap in the feature map is reported

A path upstream publishes that no feature area claims appears in no feature specification, so it
would otherwise be invisible in a report organized by feature. Each one is called out above the
summary and labelled against the previous snapshot:

- **new**: the previous snapshot had no such gap and this one does. This is the case that needs a
  decision.

- **unchanged**: a gap that was already known. It stays visible so it is not forgotten, without
  reading as a fresh regression every month.

- **resolved**: a gap the previous snapshot had and this one does not, because an area now claims
  the path.

Only `new` and `unchanged` entries are recorded in the metadata, so a resolved path is reported in
the month it was resolved and then leaves the baseline. It is said once rather than repeated
forever. The consequence, which is the right one, is that a path falling out of the feature map a
second time reads as `new` again: a routing regression is genuinely new information, not a
continuation of a gap that was already closed.

Today one path is deliberately unassigned, `/content_provenance_checks`. It is new upstream, the SDK
has no folder for it, and leaving it visible is what will prompt the decision about whether to carry
it.

## What is allowed to change on a forced rerun

`-Force` reprocesses identical content. The contract below covers every field in
`.spec-metadata.json`, not only the ones a single run happened to move, because a partial account of
it is the kind of thing that quietly stops being true.

| Field | Changes on a forced rerun of identical content | Why |
|---|---|---|
| `version` | No | Read from the source document's `info` block. |
| `schemaVersion` | No | Only moves when the metadata shape itself is revised, which is a code change. |
| `source` | No | The pinned URL, which contains the resolved commit. Same ref, same commit, same URL. |
| `downloadedCommitSha` | No | Same ref resolves to the same commit while upstream is still. |
| `sourceContentHash` | No | It is the hash of identical bytes. If this moves, the content moved and the run is not a rerun. |
| `processedAt` | **Yes** | The wall-clock time the run happened, which is the point of recording it. |
| `featureCount` | No | Determined by the feature map and the source. |
| `unassignedPaths` | No | Including each `status`, which is computed against the previous snapshot rather than the clock. |
| `excludedPaths` | No | Read from the checked-in exclusion list. |
| `diffScope` | No | A constant of the build; it moves when the comparison is changed, which is a code change. |
| `processingIdentity` | No | A fingerprint of the tool's behavior. It moves when the tool changes, which is a code change, and a forced rerun is not one. |

The generated files divide the same way. Every per-feature specification file stays byte-for-byte
identical, as does every structural finding in the diff report: the counts, the change lists, the
callouts, and the provenance table. The two generated-at timestamps in the report header do not.

So a forced rerun of identical content moves exactly three values across the whole snapshot:
`processedAt` and the two report header timestamps. If it moves anything else while the content hash
is unchanged, the tool has a determinism bug, and that is worth investigating rather than committing.
Tests guard this property directly, so it should fail in CI before it reaches a snapshot.

### Whether a timestamp-only change should open a pull request

It should not, and it cannot. A forced run reprocesses, then compares its output against the
committed snapshot and finds it identical, so nothing is rotated and nothing is published. The run
reports no change and the working tree is left untouched.

`-Force` exists to bypass the *input* check, for the times you want to verify the pipeline against
known-unchanged input. It does not bypass the *output* check, and the two are deliberately separate.
A forced run tells you the pipeline still works and leaves nothing behind to clean up.
