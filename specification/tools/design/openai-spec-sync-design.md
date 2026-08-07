# OpenAI spec sync: keeping the .NET SDK aligned with the REST API

The OpenAI .NET SDK is generated from a TypeSpec definition that is meant to mirror OpenAI's public
REST API. That REST API moves on its own schedule. OpenAI ships changes to its OpenAPI specification
whenever the platform evolves, and it is on us to notice, understand, and fold those changes into our
TypeSpec. Today that awareness is manual. Someone has to remember to look, pull the latest spec, and
reason about what actually changed and what it means for the SDK. It is easy to miss a change, and
even when we catch one, the work of figuring out what changed across a 2.8 MB specification is slow
and error-prone.

This document describes how we close that gap: a scheduled, intelligent spec sync that runs
every month, detects exactly what changed upstream, and delivers a clear, reviewable summary as a pull
request. The goal is that a maintainer opens a PR that already answers "what changed, and what does it
mean for us?" rather than starting from a blank page. Over time, the same workflow grows from telling
us what changed toward proposing the TypeSpec changes that the deltas imply.

The emphasis here is on the scenarios we are working toward, the experience we want maintainers to
have, and the implementation that delivers both. The first half is the why: what a maintainer sees and
why it is shaped that way. The second half, starting at
[how this is put together](#how-this-is-put-together), is the how: the repository layout, the
processor, the orchestrator, the workflow, and the contracts between them, at the level of detail an
engineer needs to build or change it.

For the day-to-day operating rules, such as how to run a sync, what has to be true before a snapshot
publishes, and what is allowed to change on a forced rerun, see the OpenAI Specification Snapshots
section of [`CONTRIBUTING.md`](../../../CONTRIBUTING.md). This document explains the design; that one is
the reference you reach for while working.

## Things to know before reading

- Everything described here is implemented. Paths, file names, options, and outputs in the detailed
  design sections are the ones in this repository, not placeholders.

- The scope of this document is Phase 1: detecting upstream changes, producing a per-feature diff
  report, and delivering it as a PR. Phase 2, an agentic step that proposes the actual TypeSpec edits,
  is sketched as the horizon we are building toward, not specified in detail.

- The decisions that shaped this, and the alternatives that were considered and rejected, are in
  [Appendix B](#appendix-b-key-decisions-condensed) and
  [Appendix C](#appendix-c-alternatives-we-considered-and-did-not-take) rather than in the main body.
  The blow-by-blow of how the design got here, including every defect a review round turned up, is in
  the companion [`agentic-workflow-migration-plan.md`](agentic-workflow-migration-plan.md).

- A guiding constraint runs throughout: the parts that must be exact and repeatable (which schemas
  moved, what changed) are done by deterministic code, and the parts that benefit from judgment (what
  a change means, how significant it is) are done by an AI agent. Appendix A explains why the line is
  drawn where it is.

## How the pieces fit together

It helps to hold the whole chain in mind, because it explains where this project sits and why its
output matters.

1. OpenAI publishes a REST API description as an OpenAPI document (in `openai/openai-openapi`). This
   is the upstream source of truth for the HTTP API.

2. Our SDK is not generated from that file directly. `openai-dotnet` maintains a TypeSpec definition
   that a code generator turns into the C# SDK. TypeSpec is the SDK's source of truth, and the OpenAPI
   document is what the TypeSpec is meant to mirror.

3. The gap we are closing is the step between those two. When OpenAI changes the OpenAPI spec, someone
   has to notice and update the TypeSpec to match. This project makes that noticing automatic, and
   increasingly makes the understanding automatic too.

4. The repository already documents how that update is done, in the `ingesting-spec` skill under
   `.github/skills/`. That skill describes how a human or an agent works through a change area by
   area. This project feeds it rather than replacing it, and the report speaks in the area names that
   skill already uses.

So the spec sync is a change-detection and comprehension aid for the spec-to-TypeSpec step. It does
not generate the SDK, and in Phase 1 it does not edit TypeSpec. It makes sure we always know what
upstream did, in terms we can act on.

## What we are building

A workflow that, on a monthly schedule, takes a fresh snapshot of OpenAI's REST specification, splits
it into the 24 feature areas the SDK is organized around, compares it against last month's snapshot,
and opens a pull request containing both the precise structural diff and an AI-authored explanation of
what changed and what it likely means for our TypeSpec. It can also be run on demand whenever we need
an answer sooner.

---

## The scenarios we are working toward

### Scenario 1: the monthly heartbeat

It is the 2nd of the month. Without anyone lifting a finger, the workflow wakes, fetches the latest
OpenAI REST specification, and records exactly which upstream commit it pulled. It splits that
specification into 24 self-contained per-feature specifications and compares them, feature by feature,
against the snapshot it produced last month.

OpenAI shipped changes this month, so there is something to say. The workflow rotates last month's
snapshot into place as the new baseline, writes the fresh snapshot alongside it, and generates a diff
report: which operations and schemas were added, removed, or changed, per feature area. It then opens
a ready-for-review pull request whose description leads with a human-readable summary. That summary
covers the significant changes, anything that looks breaking, and a sense of how much TypeSpec work
each area implies. The detailed, navigable report follows underneath.

A maintainer arrives to a PR that has already done the reading for them. Instead of "did anything
change upstream, and where?", the question becomes "do I agree with this read, and which of these do
we act on?". That shift, from discovery to decision, is the point of the whole exercise.

### Scenario 2: a quiet month

It is the 2nd of the month again, but this time OpenAI has not touched the specification since our last
run. The workflow notices immediately, because the document it just downloaded fingerprints identically
to the one recorded in last month's metadata. Rather than churn through processing and open an empty,
confusing PR, it quietly does nothing. There is no snapshot rotation, no PR, no notification, and no AI
cost. Silence here is a feature: the absence of a PR is a trustworthy signal that nothing changed.

"Nothing changed" turns out to be two questions rather than one, and it is worth separating them.

The first is whether the input moved, and there the fingerprint is the document itself rather than the
upstream commit. A commit that touches other files in that repository moves the commit identifier
without moving the thing we care about, and treating that as news would produce a PR with nothing in it.

The second is whether *we* moved. A month where upstream is perfectly still but somebody has since
edited the feature map, the exclusions, or the comparison scope is not a quiet month at all: the
snapshot in the repository was produced by a tool that no longer exists, and skipping the run leaves it
describing itself in terms nothing else uses. So the run also fingerprints its own behavior, and either
fingerprint moving is a reason to work. That second one is derived from the configuration wherever
possible, because a version number a person has to remember to bump is a version number that will
eventually be wrong.

And there is a third case that looks like a change and is not. Upstream edits a path we exclude, or a
documentation key we strip. The source moves, the run happens, and the generated files come out
byte-identical. That is correctly no PR, which is why the decision to publish is made by comparing the
output rather than by remembering that the input moved.

### Scenario 3: getting an answer on demand

OpenAI ships a substantial spec change mid-month, well ahead of our scheduled run, and we want to
understand it today. A maintainer triggers the workflow on demand, from the Actions tab, from the
command line, or locally on their own machine, and gets the same report without waiting for the
calendar.

The on-demand path is deliberately flexible for the two reasons people actually reach for it.

- Getting an advance copy. A maintainer can point the run at a specific upstream commit or branch,
  not just the current default, to preview what a not-yet-released or in-progress spec change would
  mean for us. They can also ask for a "just show me" run that produces the report as an artifact
  without opening a PR, when they only want to look.

- Testing the workflow itself. Because the same logic runs identically in CI and on a laptop, a
  maintainer changing the tool or the report format can run it locally against a scratch copy,
  inspect the output, and iterate quickly, with a "force" option to regenerate even when the upstream
  commit has not changed. There is no drift between how CI runs it and how someone tests it.

### Scenario 4: making sense of a change

A maintainer opens this month's PR. The upstream diff is real but sprawling: dozens of schema changes
spread across several feature areas, a couple of renamed types, a new optional field here, a widened
enum there. Reading that raw is exactly the tedious, error-prone work we are trying to eliminate.

Instead, the PR opens with an explanation written for a human. It gives a short narrative of the most
consequential changes, grouped by feature area, a call-out of anything that looks like a breaking
change, and, for each area, a read on how confidently the change maps onto a mechanical TypeSpec
update versus something that needs real design judgment. The precise, exhaustive structural diff is
still there beneath it, so nothing is hidden, but the maintainer starts from understanding rather than
from raw data.

This analysis is not a fixed feature we build once and freeze. It is expected to keep getting better,
with sharper summaries, more useful groupings, and more reliable judgments about significance. That is
a core reason the workflow is built to host a capable AI agent rather than a one-off script.

Underneath the narrative sits the structural diff itself. It opens with a per-area scoreboard, so the
first thing a maintainer sees is where the weight of the month landed. The box below is what that
section actually looks like when rendered:

> **Example: the per-area scoreboard**
>
> | Feature | Paths +/- | Ops +/-/▲ | Schemas +/-/▲/↔ | Total Changes |
> |---------|:---------:|:---------:|:----------------:|:-------------:|
> | Responses | +1 / -0 | +1 / -0 / ▲0 | +59 / -12 / ▲65 / ↔9 | 147 |
> | Chat | +0 / -0 | +0 / -0 / ▲0 | +1 / -1 / ▲20 | 22 |
> | Embeddings | +0 / -0 | +0 / -0 / ▲0 | +0 / -0 / ▲1 | 1 |
>
> Each feature name links to that area's section further down the report.

Each area then expands into its own section, with renames called out separately from additions so a
type that merely moved is never mistaken for one that appeared. The box below is a live version of
that structure, collapsed exactly as the report presents it, so it can be opened and read:

<blockquote>
<strong>Example: one feature area's section</strong>

<p><strong>Responses</strong></p>

<details><summary>147 changes &middot; <code>responses.yml</code></summary>

<details><summary>Renames detected (9)</summary>
<pre>
~ CodeInterpreterContainerAuto → AutoCodeInterpreterToolParam  (line 619)
~ Summary                     → SummaryTextContent            (line 6950)
</pre>
</details>

<details><summary>Added (52)</summary>
<pre>
+ /responses/compact                             (line 217)
+ /responses/compact.post: Compactconversation   (line 218)
</pre>
</details>

<details><summary>Changed (65)</summary>
<pre>
▲ ResponseProperties.truncation: enum widened (+ "auto_summary")
▲ ResponseTextConfig.format: now required
</pre>
</details>

</details>
</blockquote>

Everything is nested in collapsed sections, because the full report for a busy month runs to hundreds
of kilobytes. The scoreboard is what a maintainer reads; the detail is what they open when a line in
the scoreboard raises a question. A complete example is linked from the
[references](#references).

### Scenario 5: when a new feature area appears

OpenAI introduces an entirely new surface area of the API, a set of endpoints that does not map onto
any of the 24 feature areas the SDK is organized around. The deterministic split notices this
precisely. It cannot place these operations, so it flags them rather than guessing or silently
dropping them.

The agent then does what it is good at. It looks at the flagged, unassigned operations and proposes
where they belong, whether that is an existing area or a case that a new area is warranted, and
surfaces that proposal in the PR for a human to accept or adjust. The feature-area map is allowed to
evolve, but it evolves through a reviewed proposal, never through the workflow silently
re-categorizing things on its own.

There is a wrinkle worth being explicit about, because it decides whether this scenario stays useful
past its first outing. Some gaps get looked at and deliberately left open. `/content_provenance_checks`
is one today: it is a real new upstream surface, and the answer to "which area owns it" is currently
"none, and that is fine for now." A callout that re-announces that same decision as a fresh finding
every month is a callout people stop reading. So each flagged path is marked as new, unchanged, or
resolved against last month's snapshot. The standing ones stay visible without pretending to be news.

The resolved case is deliberately said once. When an area finally claims a path, that month's report
says so and the entry then leaves the record, rather than being carried forward as a permanent note
that something used to be a problem. The trade is that if the routing regresses later, the path shows
up as new again. That reads correctly: a gap reopening is fresh news, not the continuation of one that
was already closed.

### Scenario 6: closing the loop (the horizon)

Everything above stops at understanding the upstream change. The direction we are building toward is
to close the loop: an agentic step that reads the same diff report and proposes the actual TypeSpec
(`.tsp`) edits the deltas imply, as its own reviewable pull request.

This is genuinely harder and higher-stakes than reporting. It edits code that feeds the SDK
generator, and it calls for real judgment about how to model an API change in TypeSpec. It is
explicitly out of scope for Phase 1. But it is the reason the Phase 1 foundation is built the way it
is, on a workflow engine designed to host exactly this kind of guarded, human-reviewed agentic work,
so that reaching Scenario 6 is an extension of what already exists rather than a rebuild.

---

## Goals

- Never silently fall behind. If OpenAI changes the REST spec, we find out automatically, on a
  predictable monthly cadence, with the exact upstream commit recorded for provenance.

- Deliver understanding, not just data. Every change set arrives as a PR that explains what changed
  and what it likely means for TypeSpec, backed by a precise structural diff.

- Make the answer available on demand. Anyone can trigger a run, for an advance copy, a specific
  upstream commit, or to test a change, and get the same result the schedule would produce.

- Keep the trustworthy parts exact. The determination of what changed is deterministic and repeatable,
  so month-over-month comparisons are clean and free of spurious noise.

- Let the intelligent parts grow. The summarization, analysis, and feature-area proposals are designed
  to improve over time without disturbing the deterministic foundation.

- Fit the house. The workflow lives and behaves like the automation already in `openai-dotnet`,
  delivering its output as a reviewable pull request against a protected `main`.

## Non-Goals

- Editing TypeSpec in Phase 1. Proposing `.tsp` changes from the deltas is the Phase 2 horizon
  (Scenario 6), not part of this design.

- Generating or building the SDK. That remains the existing TypeSpec code-generation pipeline. We feed
  the humans who steward it.

- Replacing human review. The workflow always proposes, and a person always decides. Nothing merges
  itself, and the feature-area map never changes without approval.

- Redefining the processing rules. The split and diff logic is the existing, tested behavior. This
  project automates and surrounds it. It does not re-litigate it.

---

## What success looks like

- A change is never missed. In months where upstream changed, a PR appears; in months where it did
  not, none does; and both outcomes are correct. The presence or absence of a PR is a signal we can
  trust.

- Time-to-understanding drops. A maintainer can tell what materially changed, and where, in minutes
  from the PR summary, not hours from reading a raw diff.

- The noise stays out. Because the comparison baseline is deterministic, PRs contain real changes
  only, so we do not train ourselves to ignore them.

- The analysis earns more trust over time. As the agentic summarization and feature-area proposals
  improve, maintainers rely on them more, and the manual reading shrinks.

- The path to Phase 2 is short. When we are ready to have the workflow propose TypeSpec edits, it is an
  added capability on the same foundation, not a new system.

---

## How this is put together

The shape of the implementation follows directly from the deterministic and agentic split described
above. There are four pieces, and each one exists because something in that split demands it.

### The snapshot pair is the whole trick

The repository carries two snapshots of the upstream specification, `current` and `previous`, each a
folder of per-area specification files plus a report and a provenance record. A run rotates `current`
into `previous` and builds a new `current`. That is the entire comparison mechanism, and it is worth
being explicit about why it is done this way rather than by diffing against a remembered upstream
commit.

Committing both sides means the baseline is a reviewed artifact rather than a moving target. Anyone
can see exactly what the last run believed the world looked like, a bad month can be corrected by
fixing a file rather than by re-running against a hoped-for state, and the diff a maintainer reads in
the pull request is a diff of files that are actually in the repository. The cost is repository size,
which is real but bounded and well worth what it buys.

It also makes byte-stability a hard requirement rather than a nicety. If the same input can produce
two different outputs, the snapshot pair starts generating diffs nobody made, and the trust the whole
approach rests on is gone. That is the constraint every decision below answers to.

### A console tool owns the split and the diff

A small .NET console application in the repository does the exact work: download, sanitize, strip the
documentation-only metadata, assign every operation to a feature area, resolve the transitive
reference closure so each area file stands alone, compare against `previous`, and write the report.

It lives in the repository as source rather than as a package, so it is reviewed, versioned, and
changed alongside the thing it produces. It is deliberately excluded from the SDK solution, so it can
never enter the SDK build or ship in a package.

Two properties matter more than any other. It emits the same bytes for the same input, including line
endings, which the snapshot pair depends on. And it never guesses: an operation it cannot place is
reported as unassigned rather than filed somewhere plausible, because a wrong answer that looks right
is worse than an obvious gap.

Both properties are asserted by a test suite that runs on every pull request touching the tooling.
One of those tests compares the tool's feature area list against the repository's own specification
area folders, so a regrouping on either side surfaces as a reviewed change rather than a quiet
divergence between what the SDK is organized around and what the report speaks in.

### One script drives both CI and a laptop

A single PowerShell orchestrator sits between the workflow and the tool. It resolves the upstream ref
to a commit, downloads the specification pinned to that commit, fingerprints it, decides whether
anything actually moved, runs the tool, and rotates the snapshots.

There is exactly one of these, and CI calls the same script a maintainer calls. That is a deliberate
guard against the failure where the automated path and the manual path drift apart and a change works
locally but not in the pull request, or vice versa.

That ordering, rotating last rather than first, is what keeps a failed run harmless. Processing writes
to a staging directory, and the committed snapshots are not touched until the tool has succeeded. A
network failure, a malformed download, or a validation error leaves the baseline exactly as it was.
The alternative, rotating up front and then processing, means every failure costs you the baseline
you were going to compare against next time.

The subtler half of that is what counts as success. A document can parse cleanly and still be the
wrong document, and a snapshot with nothing in it will publish quite happily if nobody asks whether
it makes sense. So the tool refuses to produce a snapshot that splits into no feature areas, and it
treats a feature file that fails validation as a failed run rather than a warning. Both were found by
pointing the real orchestrator at deliberately broken sources and watching what it did, and one of
them was busy overwriting a good snapshot with an empty one at the time.

There is one gate rather than several. Publication turns on the tool's exit code and nothing else, so
every route to a published snapshot, the monthly run, a local refresh, a forced run, and the mode that
skips the diff entirely, is stopped by the same check. That matters because the mode that skips the
diff also skips the report, and it would be easy to assume it skips the verdict too.

A staging directory that never becomes a snapshot is cleaned up on the way out, whatever went wrong.
The one exception is a dry run that succeeded, where the staged output is the deliverable and the run
tells you where to find it. A dry run that *failed* is cleaned up like any other failure, so a broken
experiment cannot sit in a temporary folder waiting to be mistaken for a good one.

Pinning to a resolved commit matters more than it first appears. Without it, an upstream push part way
through a run could produce a snapshot stitched from two different versions of the document, and
nothing downstream would ever notice.

### The agentic layer is a thin, guarded top

The workflow itself is authored as markdown for GitHub Agentic Workflows and compiled into a lock file
that Actions runs. The deterministic work runs first, as ordinary workflow steps. Only then does the
agent start, and what it receives is a finished report to interpret.

The agent has no write access to the repository, and it does not stage files, create a branch, or
push. Everything it produces leaves through the safe outputs mechanism, which collects the working
tree, opens the pull request on its behalf, and bounds how large that pull request can be. So the
worst case for a bad agent run is a poor summary on a correct diff, never a corrupted baseline.

There is a quiet-month path worth calling out, and being precise about it matters because the obvious
description of it was, for a while, not quite true. The deterministic steps decide there is nothing to
do and stop, but the agent job itself still starts. So the agent is told, as the first thing it does,
to read a status marker and stop without reading anything else if there is nothing to report. Behind
that sits the harder guarantee: the pull request step does nothing when the working tree is unchanged,
so a run that reaches the agent by mistake still cannot open an empty pull request.

Publishing is a separate decision from running, deliberately. A run that was forced past the input
check, which is the normal way to test the pipeline, still compares what it produced against what is
committed, and publishes nothing when they match. Otherwise the diagnostic tool for checking the
pipeline would itself be a source of noise, which is a good way to teach people not to use it.

### Provenance, so a snapshot can always answer for itself

Every snapshot records the URL it came from, the upstream commit it was pinned to, a hash of the bytes
as downloaded, and the timestamps around the run. The hash is what the no-op check uses, deliberately,
because an upstream commit that touches other files moves the commit identifier without moving the
document we care about.

The record also carries a version of its own shape, which sounds like bookkeeping and is not. Reading
that file has five possible outcomes, not two: it can be valid, absent, older than the current shape,
corrupt, or newer than the tool understands. Only absence is ordinary, and it is what every first run
starts from. The rest either read cleanly or stop the run, because a hash that cannot be read is not
the same as a hash that does not match, and quietly conflating them is how a corrupt file talks you
into replacing a perfectly good baseline.

That record also has to outlive the build that wrote it. Metadata written before the shape-version
field existed is still readable, as is metadata whose enum values were written as numbers by an
earlier tool. A baseline only its own writer can read is not a baseline; it is a snapshot that
expires the first time the tooling changes. The values it persists for those enums are fixed and not
reusable, since renumbering them would leave old files reading successfully and meaning something
different, which is a worse failure than not reading at all.

None of that provenance is any use hidden in a JSON file the reviewer will not open, so the report
opens with it: where the document came from, which commit it was pinned to, the content hash on both
sides of the comparison, and whether the sanitizer had to repair anything before parsing. Both are
written from a single record in a single run, so the report and the metadata cannot tell different
stories about what was processed.

---

## What actually gets built

Everything above is the reasoning. What follows is the implementation, at the level of detail needed
to build it, review it, or change it later.

### What changes in the repository

Two new folder trees, one new script, three new workflow files, and a handful of edits to files that
already exist. Nothing in the SDK build path is touched.

```
openai-dotnet/
├── .github/
│   ├── aw/
│   │   └── actions-lock.json                      NEW  gh-aw's pinned action digests
│   └── workflows/
│       ├── openai-spec-sync.md                    NEW  the agentic workflow source
│       ├── openai-spec-sync.lock.yml              NEW  compiled from the above; what Actions runs
│       ├── agentic-lockfile-check.yml             NEW  fails a PR whose lock file is stale
│       ├── agentics-maintenance.yml               NEW  generated by gh-aw; do not edit
│       ├── spec-tooling-checks.yml                NEW  runs the processor's test suite
│       └── integrity-checks.yml                   MOD  excludes the two new specification/ subtrees
├── scripts/
│   └── Sync-OpenAISpec.ps1                        NEW  the orchestrator, shared by CI and laptops
├── specification/                                 (existing TypeSpec codegen input, untouched)
│   ├── base/typespec/                             (existing; the authoritative area folder set)
│   ├── openai/                                    NEW  the snapshot pair
│   │   ├── current/
│   │   │   ├── .spec-metadata.json                     provenance and processing identity
│   │   │   ├── diff-report.md                          the rendered report
│   │   │   └── <24 feature files>.yml                  responses.yml, chat.yml, ...
│   │   └── previous/                                   same shape; the comparison baseline
│   └── tools/                                     NEW  the processor (never in the SDK build)
│       ├── design/                                     these design documents
│       ├── openai-spec-processor.json                  the config the orchestrator passes in
│       ├── taxonomy-exceptions.json                    declared, reasoned taxonomy divergences
│       ├── OpenAI.SpecProcessor/                       console app source
│       └── OpenAI.SpecProcessor.Tests/                 the test suite
├── .editorconfig                                  MOD  C# rules rescoped to all hand-written code
├── .gitattributes                                 MOD  lock files marked generated
├── CONTRIBUTING.md                                MOD  folder table plus the operating contracts
├── Directory.Packages.props                       MOD  the processor's four dependencies
└── .github/workflows.md                           MOD  an entry for the new workflow
```

The edits to existing files are small but each one exists for a reason worth knowing.

| File | Change | Why |
|---|---|---|
| `integrity-checks.yml` | Adds `!specification/openai/**` and `!specification/tools/**` to its path filter | That workflow re-runs code generation and fails on drift. A snapshot has nothing to say about generated code, so without the exclusion every sync PR would run a long, irrelevant check. |
| `.gitattributes` | `.github/workflows/*.lock.yml linguist-generated=true merge=ours` | The lock file is compiler output. Marking it collapses it in reviews and stops merge conflicts in a file nobody hand-edits. |
| `Directory.Packages.props` | A conditional item group for `OpenAI.SpecProcessor*` | Central package management is on repository-wide, so versions cannot live in the csproj. The condition keeps the tool's dependencies out of the SDK projects' resolution. |
| `.editorconfig` | C# section rescoped from `[src/*/{Custom.*}/**.cs]` to `[*.cs]`, with `src/Generated/**` and `api/*.cs` marked generated | The old glob matched nothing in the repository, so the style rules were dead. Every rule is `suggestion` severity or lower and style is not enforced at build time, so broadening it cannot fail a build. |
| `CONTRIBUTING.md` | The folder table, plus the publication, no-op, and reporting contracts | The contracts belong where a contributor will look for them, not only in a design document. |
| `.github/workflows.md` | An entry for the sync workflow | The repository documents each workflow there. |

The processor project is deliberately absent from `OpenAI.slnx`, so it can never enter the SDK build
or ship in a package. It builds and runs via `dotnet run --project`, and its tests run from their own
workflow.

The csproj opts out of the repository-wide defaults that only make sense for shipping libraries:
packaging, strong-name signing, documentation generation, and source link. It targets `net10.0`
alone rather than the SDK's multi-target set, and builds clean under the repository's
`TreatWarningsAsErrors`.

### The processor, in order

The console app is the deterministic half. A single `preprocess` run does the following, and any step
failing is a failed run.

1. **Acquire.** Take the specification from `--new-spec` when the orchestrator has already downloaded
   it, which is the normal path, or download it from the configured URL when run standalone.

2. **Sanitize.** The upstream document does not parse under a strict YAML reader. `SpecSanitizer`
   repairs the one offending construct and reports how many lines it touched, which is surfaced in
   both the report and the metadata so a silent repair is never invisible. The sanitizer carries a
   version, and that version participates in the processing identity.

3. **Parse and clean.** Load the document, then strip the keys that exist only to build OpenAI's
   website: `x-oaiMeta` and `x-oaiTypeLabel`, removed everywhere in the tree.

4. **Exclude.** Drop the paths the SDK does not and will not carry, before anything else looks at
   them. See the exclusion table below.

5. **Split.** Assign every remaining operation to a feature area, tags first and path prefix second,
   then resolve the transitive `$ref` closure for each area so its file stands alone. A reference the
   splitter does not support is an error naming the kind, not a skip.

6. **Validate.** Prove each feature file resolves completely, that no non-excluded operation was
   lost, and that the split produced feature areas at all. A split producing zero areas is a failed
   run rather than a published empty snapshot.

7. **Diff.** Compare the new feature set against the baseline and classify every difference.

8. **Report and record.** Write `diff-report.md` and `.spec-metadata.json` from the same in-memory
   record, so they cannot disagree.

Every file it emits uses LF endings, because the repository normalizes to LF and a CRLF emission
would produce phantom diffs, which is the one thing the snapshot pair cannot tolerate.

### The feature area map

Twenty-four areas, named after the folders under `specification/base/typespec` so the report speaks
the vocabulary maintainers already use when ingesting a change:

`administration`, `assistants`, `audio`, `batch`, `chat`, `containers`, `conversations`,
`embeddings`, `evals`, `files`, `fine-tuning`, `graders`, `images`, `messages`, `models`,
`moderations`, `realtime`, `responses`, `runs`, `skills`, `threads`, `uploads`, `vector-stores`,
`videos`.

Assignment is by tag first, then by path prefix for untagged operations, with an explicit path list
for the handful of operations whose tags do not place them correctly. That precedence has a sharp
edge worth knowing about: an area's excluded path prefixes are only consulted during path matching,
so an operation carrying a matching tag is claimed before exclusion is ever considered.

Eight paths are dropped at the door and never appear in any feature file.

| Excluded | Count | Why |
|---|---|---|
| `/chatkit/**` | 5 | No SDK client, untagged upstream, beta-only in the published docs. |
| `/completions` | 1 | The legacy completions endpoint. Chat at `/chat/completions` is a different surface and is kept. |
| `/realtime/sessions`, `/realtime/transcription_sessions` | 2 | Legacy in the platform docs. The stable Realtime Calls paths are kept. |

This is a relevance judgment rather than a structural one, and it has a consequence to be clear-eyed
about: a path dropped at the door can never be observed changing. The validator's operation-coverage
guarantee therefore covers every *non-excluded* operation, which is a narrower claim than it reads
as. If the SDK ever picks up one of these surfaces, this list is the first thing to revisit.

**Where the taxonomy's authority lives.** Two things describe the same taxonomy from opposite
directions: the folders under `specification/base/typespec`, which is what the SDK is organized
around, and the processor's feature map, which is what the snapshot and the report speak in. Neither
is subordinate to the other, and a test asserts they agree. An intentional divergence is declared in
`taxonomy-exceptions.json` with a reason, so the check keeps catching the accidental kind. Two
entries exist today: `completions`, excluded by decision, and `streaming`, which owns no paths of its
own. A further test asserts every declared exception still names something real, so the list cannot
rot into a set of stale silencers.

Note that the "Available Areas" table in `.github/skills/ingesting-spec/file-locations.md` is a stale
view of this taxonomy and is not authoritative. It lists seventeen areas, omitting the seven that
exist on disk but are absent from it: `administration`, `evals`, `messages`, `runs`, `skills`,
`threads`, and `uploads`.

### The processor's command line

```
dotnet run --project specification/tools/OpenAI.SpecProcessor -- \
  --config specification/tools/openai-spec-processor.json \
  preprocess --new-spec <file> --previous-spec <dir> --output <dir> --report <dir> \
             --source-url <url> --commit-sha <sha>
```

| Option | Purpose |
|---|---|
| `--config` | The configuration file to bind, replacing the embedded `appsettings.json`. The orchestrator always passes this, so the checked-in config is what governs a real run. |
| `--new-spec` | The already-downloaded document. Omitting it makes the tool download for itself, which is the standalone path. |
| `--previous-spec` | The baseline directory to compare against. |
| `--output`, `--report` | Where the feature files and the report are written. The orchestrator points both at a staging directory. |
| `--source-url`, `--commit-sha` | Provenance, recorded rather than used. Passed because the orchestrator is what resolved and downloaded. |
| `--no-diff` | Split and validate only. Note that this skips the report, not the verdict: validation still gates the exit code. |
| `--include-descriptions` | Include summary and description edits in the diff. Off by default, because prose churn upstream would otherwise dominate every report. |

There is a second subcommand, `identity --fingerprint-only`, which prints the processor's behavior
fingerprint and nothing else. It exists so the orchestrator can ask the tool what it would do without
carrying a second implementation of what "the same behavior" means.

Configuration is deliberately thin, holding only the source URL and the default input and output
locations:

```json
{
  "Spec": {
    "SourceUrl": "https://raw.githubusercontent.com/openai/openai-openapi/main/openapi.yaml",
    "RawDownloadFile": "openai-rest-raw.yml",
    "DiffReportFile": "diff-report.md"
  },
  "Defaults": {
    "OutputDirectory": "../openai/current",
    "ReportDirectory": "../openai/current",
    "PreviousSpecDirectory": "../openai/previous"
  }
}
```

The feature map itself is code rather than configuration, on purpose. It is the thing whose change
must be reviewed, and code review is the mechanism the repository already has for that.

### The snapshot metadata record

`.spec-metadata.json` is the contract between one month's run and the next. Every field is either
provenance a reviewer needs or an input to a decision the next run makes.

| Field | Purpose | Moves on a forced identical rerun? |
|---|---|---|
| `version` | `info.version` from the upstream document | No |
| `schemaVersion` | The shape of this file, currently `1` | No |
| `source` | The commit-pinned download URL | No |
| `downloadedCommitSha` | The upstream commit the download was pinned to | No |
| `sourceContentHash` | SHA-256 of the downloaded bytes; the input side of the no-op check | No |
| `processedAt` | When the run happened, ISO-8601 UTC | **Yes**, and it is the only field that does |
| `featureCount` | How many feature files the split produced | No |
| `unassignedPaths` | Every path no area claimed, with methods, operation IDs, tags, a reason, and a `new`/`unchanged`/`resolved` status | No |
| `excludedPaths` | What was dropped at the door, recorded so an exclusion can be told apart from a gap | No |
| `diffScope` | What the comparison covers, what it does not, and which findings are heuristic, with a version of its own | No |
| `processingIdentity` | The behavior fingerprint: behavior, sanitizer, and diff-scope versions plus a hash derived from the feature map | No |

Timestamps are ISO-8601 UTC with no local-timezone field, so a run's output does not depend on which
runner executed it.

**Reading that file has five outcomes, not two.** Both the processor and the orchestrator classify it
the same way, because a disagreement between them about what a metadata file means is exactly the
kind of bug that only shows up on a bad month.

| State | Meaning | What happens |
|---|---|---|
| Valid | Current schema, parses, carries a hash | Proceed |
| Missing | No file at all | Proceed. This is the legitimate bootstrap case, and the only benign one. |
| Legacy | Older schema, still readable | Proceed with a warning. Reseed before trusting it as a baseline. |
| Malformed | Unparseable, hash absent, or an enum value outside the defined set | Stop |
| UnsupportedVersion | Written by a newer tool than this one | Stop rather than reinterpret |

Only absence is ordinary. A hash that cannot be read is not a hash that does not match, and quietly
conflating them is how a corrupt file talks the pipeline into replacing a perfectly good baseline.

**The record has to outlive the build that wrote it.** Metadata written before `schemaVersion`
existed still reads, as does metadata whose enum values were persisted as numbers by an earlier tool.
Those numeric values are pinned by explicit declaration and must never be renumbered, since an old
file would then read successfully and mean something different, which is worse than not reading at
all. Legacy numeric values are rewritten by name on the next run.

### The orchestrator, step by step

`scripts/Sync-OpenAISpec.ps1` is the single code path CI and a laptop both take. Its parameters are
`-SourceUrl`, `-SourceRef`, `-Force`, `-DryRun`, and `-RepoPath`, and it follows the repository's
PowerShell conventions: shebang, comment-based help, `SupportsShouldProcess`, `Write-Log` family
helpers, `$LASTEXITCODE` checked after every external call, and `Push-Location`/`Pop-Location` in
`try`/`finally`.

1. **Resolve the ref to a commit.** `GET /repos/openai/openai-openapi/commits/<ref>` yields a SHA,
   and the download URL is built from that SHA rather than from the branch name. Without this, an
   upstream push partway through a run could produce a snapshot stitched from two revisions with
   nothing downstream ever noticing. A `GH_TOKEN` is optional and only raises the rate limit; the
   upstream repository is public. An explicit `-SourceUrl` is an escape hatch and skips this.

2. **Download and hash.** Fetch to a temporary file and take its SHA-256. Hashing the document is
   exact, works for any source including ones with no commit of their own, and does not move when an
   upstream commit touches unrelated files.

3. **Decide whether to run.** Two questions, both of which must answer "unchanged" to skip:

   - Did the input move? Compare the fresh content hash against the recorded one.

   - Did *we* move? Ask the tool for its fingerprint and compare it against the one recorded in the
     snapshot. A snapshot recording no fingerprint reads as "cannot be shown to match", not as a
     match.

   `-Force` bypasses this check. Nothing else does.

4. **Process into staging.** Everything is written to a temporary directory. The committed snapshots
   are not touched until the processor has succeeded, so a network failure, a malformed download, a
   parse error, or a validation failure leaves the baseline exactly as it was. Rotating first and
   processing second would mean every failure costs the baseline it was about to compare against.

5. **Decide whether to publish.** A separate question from whether the run succeeded.
   `Test-MeaningfulChange` compares the staged feature files byte-for-byte against the committed
   snapshot, plus the metadata with `processedAt` removed. Identical means nothing is published. This
   is what stops a forced diagnostic run from opening a timestamp-only pull request, and it also
   covers the case where upstream genuinely changed but the change landed entirely in an excluded
   path or a stripped documentation key.

6. **Rotate and publish.** Copy `current` into `previous`, then staging into `current`. On a first
   run, where no `current` metadata exists, rotation is skipped so the seeded baseline survives.

7. **Clean up, whatever happened.** The staging directory is removed in `finally`. The one exception
   is a dry run that *succeeded*, where the staged output is the deliverable and its path is reported
   to the caller. A dry run that failed is cleaned up like any other failure, so a broken experiment
   cannot accumulate in temp or be mistaken for good output.

It emits three GitHub Actions outputs: `changed`, `content-hash`, and `output-path`.

### The workflow

`.github/workflows/openai-spec-sync.md` is authored for GitHub Agentic Workflows and compiled by
`gh aw compile` into `openai-spec-sync.lock.yml`, which is what Actions actually runs. The markdown
is inert. That asymmetry is a trap, so `agentic-lockfile-check.yml` recompiles on any PR touching the
markdown and fails if the lock file moves.

**Triggers.** A monthly schedule at `0 3 2 * *` in `America/Los_Angeles`, plus `workflow_dispatch`
with three inputs that map one-to-one onto orchestrator parameters:

| Input | Parameter | Use |
|---|---|---|
| `source_ref` | `-SourceRef` | Snapshot a specific branch, tag, or commit for an advance copy |
| `force` | `-Force` | Regenerate even when the input check says nothing moved |
| `dry_run` | `-DryRun` | Produce the output as an artifact and open no pull request |

**Permissions.** `contents: read` and `copilot-requests: write`, and nothing else. The agent job
cannot write to the repository at all.

**Steps, in order.** The deterministic work runs as ordinary workflow steps before the agent starts.

1. Set up .NET from `global-json-file: global.json`.

2. Run `./scripts/Sync-OpenAISpec.ps1` with the dispatch inputs threaded through.

3. Write a status marker to `${RUNNER_TEMP}/spec-sync-status.txt`, either `proceed` or
   `nothing-to-do`, from the orchestrator's `changed` output and the dry-run flag.

4. Upload the dry-run artifact, when this was a dry run that produced something.

5. Emit a `noop` safe output when there is nothing to report.

**The quiet-month path deserves precision, because the obvious description of it was wrong for
several review rounds.** The deterministic steps decide there is nothing to do, but gh-aw injects
frontmatter steps into the agent job, so the agent execution step itself cannot be conditioned from
the markdown. The agent therefore runs. The mitigation is two layers: the agent is instructed, as the
very first thing it does, to read the status marker and stop without reading anything else if it says
`nothing-to-do`; and `if-no-changes: "ignore"` on the pull request step means a run that reaches the
agent by mistake still cannot open an empty pull request.

**What the agent is given and what it may do.** A read-only shell restricted to
`cat, ls, head, tail, wc, grep`. No edit tool. A network allow-list of `defaults` and `github`. Its
prompt points it at the diff report, the metadata, and the existing `ingesting-spec` skill, and
explicitly forbids touching anything under `specification/openai/`, since those files are the
comparison baseline and must stay byte-for-byte as the tool emitted them. If something looks wrong
with them, it says so in the pull request body rather than fixing it.

**How the pull request happens.** The agent does not stage files, create a branch, or push. The
safe-outputs mechanism collects the working tree, creates the branch, and opens a ready-for-review
pull request with the body the agent wrote, bounded by `max-patch-files: 400` and
`max-patch-size: 10240`. So the worst case for a bad agent run is a poor summary attached to a
correct diff, never a corrupted baseline.

### Authentication, and the one setting to check

Three independent surfaces, none of which requires a long-lived secret.

| Surface | Mechanism | Secret? | Verify before the first run |
|---|---|---|---|
| Copilot inference | `copilot-requests: write` authenticates with the Actions token and bills through the org's Copilot subscription | No | The org has a Copilot subscription with centralized billing enabled |
| Pull request creation | The generated safe-outputs job, using the default `GITHUB_TOKEN` | No | "Allow GitHub Actions to create and approve pull requests" is enabled |
| Spec download | Public `raw.githubusercontent.com` | No | Covered by the `defaults` network allow-list |
| Commit resolution | Public `api.github.com`, with `GITHUB_TOKEN` passed only to raise the rate limit | No | Covered by the `github` network allow-list |

The most likely pitfall is the second row. If Actions cannot create pull requests, gh-aw silently
degrades to opening an issue instead, which looks like the workflow working until someone notices
there is no PR. The repository's existing `update-generator.yml` already opens automated pull
requests with the default token, so this is effectively proven here, but it is worth confirming
rather than assuming.

If the org-billed Copilot path is unavailable, the fallback is a `COPILOT_GITHUB_TOKEN` repository
secret holding a fine-grained PAT with Copilot Requests: Read. That makes PAT lifecycle our problem,
which is exactly why it is the fallback. Note that gh-aw treats `${{ secrets.* }}` in a
workflow-level `env:` block as a compile error in strict mode, since the value would be visible to
the model.

Branch protection on `main` is not an obstacle. The auto-opened pull request simply waits for its
required checks and reviews, which is the desired behavior.

### Running it by hand

Three ways in, all of them the same code path.

**From the Actions tab.** Pick the workflow, click Run workflow, optionally set the inputs. This is
the path for someone who does not want to install anything.

**From the CLI.** `gh aw run openai-spec-sync`, with the same dispatch inputs. `gh aw trial` runs it
without committing anything to the repository.

**Locally.** The fastest inner loop, and the one to use when changing the tool:

```powershell
# Preview an in-progress upstream change without touching the snapshot
./scripts/Sync-OpenAISpec.ps1 -SourceRef "some-feature-branch" -DryRun

# Re-verify determinism after a tool change, with upstream unchanged
./scripts/Sync-OpenAISpec.ps1 -Force -DryRun
```

A `-DryRun` writes to a temporary directory, leaves the committed snapshot alone, and tells you where
the output landed. Without `-DryRun`, a local run publishes into the working tree exactly as CI would,
which is occasionally what you want and more often not.

Two things are worth knowing before running locally. Set `$env:GH_TOKEN = (gh auth token)` first, or
unauthenticated ref resolution will eventually hit the 60-per-hour rate limit. And after regenerating
a snapshot deliberately, reseed `previous` from `current` to restore a zero-change baseline, or the
next run will report a month of movement that never happened.

### What the tests hold in place

The suite runs on every pull request touching `specification/tools/**`,
`specification/base/typespec/**`, or the orchestrator, via `spec-tooling-checks.yml`. It exists
because the properties it covers are ones that fail silently rather than loudly.

| Area | What is asserted |
|---|---|
| Diff idempotence | Comparing a snapshot with itself yields zero changes, including for schemas with discriminators, compositions, defaults, and nested properties. This started as a real defect that produced 56 phantom changes. |
| Publication gates | A dangling reference and a degenerate source each fail the run, with and without `--no-diff`, and leave the baseline directory untouched. |
| Snapshot layout | The expected file set is *derived* from the feature map rather than hardcoded, so adding an area is an explicit change rather than a count someone bumps without looking. |
| Processing identity | The fingerprint moves when the feature map, exclusions, sanitizer, or diff scope moves, and does not move otherwise. |
| Metadata | Round-trip for every enum value, the persisted numeric mapping pinned by value, an undefined numeric value rejected as malformed, and a document predating `schemaVersion` still readable. |
| Reference closure | Fixtures for parameter, response, request body, and header references, plus three external reference shapes, each failing with an error naming the kind rather than being silently skipped. Local schema references resolve transitively. |
| Taxonomy drift | The feature map and the TypeSpec folder set agree, modulo declared exceptions, and every declared exception still names something real. |
| Unassigned lifecycle | A gap is reported once as `new`, then as `unchanged` while it persists, then once as `resolved`, then dropped. |
| Provenance | The report and the metadata agree field for field, because they are written from one record. |
| YAML repair | The sanitizer fixes the construct it is meant to fix and touches nothing else. |

### The determinism contract, stated plainly

Because it is the thing the whole approach rests on, it is worth writing down exactly what must and
must not be stable.

**Byte-identical for identical input, always:** every feature `.yml` file, and every structural
finding in the report.

**Expected to move on a forced rerun of identical content:** `processedAt` in the metadata, and the
timestamps in the report header. Nothing else.

**Therefore:** a timestamp-only difference is not a change, and the orchestrator will not publish
one. That is enforced by comparing the output rather than by trusting anyone to remember it.

---

## Appendix A: why part of this is deterministic and part is agentic

The design draws a firm line between two kinds of work, because they have different failure modes.

Exact and repeatable work is done by deterministic code. Turning a 2.8 MB specification into 24
self-contained per-feature specs requires resolving every transitive reference completely and copying
each schema verbatim, and detecting month-over-month changes requires an exhaustive structural
comparison. These must be complete, so that nothing is missed, and identical run-to-run, so that the
committed snapshots do not produce spurious diffs. A language model cannot promise either at this
scale, so this work is done by a deterministic tool.

Judgment is done by an AI agent. Deciding what a change means, how significant or breaking it is, how
it should be summarized, and where a brand-new feature area belongs all benefit from exactly the
flexible interpretation a model is good at, and none of them require bit-for-bit repeatability.

The two coexist cleanly because the comparison baseline is the deterministic split, while the report
prose and PR narrative are the agent's work, and the report is never itself an input to the next
month's comparison. So the analysis can change and improve freely without ever undermining the
exactness the whole approach depends on.

## Appendix B: key decisions, condensed

- **Engine: GitHub Agentic Workflows (gh-aw).** Chosen because the AI analysis is a first-class,
  evolving part of Phase 1 rather than an optional bolt-on, and because it is the natural host for
  the Phase 2 TypeSpec-proposal work. Plain Actions would have been a retrofit.

- **The deterministic tool is retained.** The .NET processor owns the split and the diff, running as
  a console app from in-repo source.

- **Location: inside `specification/`.** The snapshots and the tool live alongside the existing
  TypeSpec definition as new sibling folders, which keeps everything about the specification in one
  place. They are excluded from the code-generation integrity checks so they cannot slow or fail
  them.

- **Delivery: a ready-for-review pull request** through the repository's standard, already-proven
  Actions-to-PR mechanism, never a direct push to `main`.

- **Cadence and provenance: monthly**, with the exact upstream commit and a content hash of the
  document captured, and a no-op when neither the source nor the processor has moved.

- **On-demand controls:** manual runs can target a specific upstream commit, force regeneration, or
  produce a report without opening a PR.

- **The feature map is code, not configuration.** It is the thing whose change must be reviewed, and
  code review is the mechanism the repository already has for that.

## Appendix C: alternatives we considered and did not take

**A fully agentic workflow, with no deterministic tool.** The agent would read the instructions and
perform the clean, split, and diff itself. Rejected because model output is not repeatable, which
would produce noisy diffs every month; because completeness across roughly 900 schemas is unprovable;
and because a regression in the split would be very hard to notice. The whole approach rests on the
comparison baseline being exact, and this option gives that up first.

**Rewriting the deterministic core as a Python or PowerShell script.** Worth taking seriously, since
determinism comes from running code with stable serialization rather than from .NET specifically.
Rejected on three grounds: the existing tool already encodes the feature split, the transitive
reference closure, and a report format that took real iteration; `openai-dotnet` is a C# codebase
whose maintainers review C# comfortably; and static typing genuinely helps the closure and diff
logic. The decisive risk is output parity. A rewrite must emit byte-identical splits and an
identical report on its first run, or the inaugural pull request is pure noise. That is careful work
for guarantees we already have. The cost of keeping .NET is one `setup-dotnet` and one build per
run, which is negligible at a monthly cadence.

If the team ever wants to consolidate on a single scripting language, Python with `ruamel.yaml` is
the viable target for the heavy logic, with a parity-tested port. PowerShell is the weakest fit for
the closure and diff specifically, which is why the orchestration is PowerShell per repository
convention and the heavy logic is not.

**Comparing upstream commit SHAs instead of content hashes.** Simpler, and wrong. A commit touching
any other file in `openai/openai-openapi` moves the SHA without moving the document, which would
open a pull request containing nothing.

**Rotating the snapshots first and processing second.** The obvious order, and it makes every
failure cost the baseline it was about to compare against. Processing into staging and rotating last
means a network failure, a bad download, or a validation error leaves the repository exactly as it
was.

**Letting the agent stage files and commit.** Rejected because it would give a model the ability to
rewrite the deterministic snapshot, the report, and the processor before the pull request is
created. Safe outputs bound the agent to producing prose, which keeps the worst case at a poor
summary on a correct diff.

---

## References

- [Example diff report](https://gist.github.com/jsquire/347a1ac59ae73496854d84450b729044): a complete
  report from a real three-month gap, showing the per-area scoreboard and the full expanded detail
  the excerpt in Scenario 4 is drawn from.

- [GitHub Agentic Workflows](https://github.github.io/gh-aw/): the engine the workflow is authored
  for, covering frontmatter, triggers, permissions, safe outputs, and the network sandbox.

- [gh-aw safe outputs](https://github.github.io/gh-aw/reference/safe-outputs/): specifically how an
  agent with no write access still opens a pull request, which is the guarantee the agentic layer
  rests on.

- [OpenAI OpenAPI specification](https://github.com/openai/openai-openapi): the upstream document this
  workflow tracks, at `openapi.yaml` on `main`.

- [OpenAI API reference](https://developers.openai.com/api/reference/overview): the published taxonomy
  the feature-area split is reconciled against.

- [TypeSpec](https://typespec.io/): the definition language the SDK is actually generated from, and
  the target of the Phase 2 horizon.

- [`CONTRIBUTING.md`](../../../CONTRIBUTING.md): the operating reference for running a sync, the
  publication rules, and the forced-rerun contract.

- [`agentic-workflow-migration-plan.md`](agentic-workflow-migration-plan.md): the companion working
  document. The twelve-question decision log with the alternatives weighed, the risk register, the
  phased implementation plan, the gh-aw capability reference with primary source citations, and
  Appendix E, which records what changed when this was actually built and what each review round
  caught. Read it when you want to know why a decision went the way it did, or when a change you are
  considering looks like it re-opens one.

- [`.github/skills/ingesting-spec/`](../../../.github/skills/ingesting-spec/): the existing
  area-by-area ingestion process this workflow feeds.
