---
name: editing-changelog
description: Guide for editing the CHANGELOG.md file. Use this when asked to add, update, or modify changelog entries, document new features, bug fixes, breaking changes, or any other changes in the changelog. Ensures entries follow the correct style, structure, voice, and tone.
---

# Editing the Changelog

## Overview

This skill describes how to edit `CHANGELOG.md` in the `openai-dotnet` repository. The changelog is customer-facing and follows a strict style. Every edit must conform to the conventions documented here.

## Key Rules

1. **Only edit the unreleased entry.** The topmost entry in the changelog has the version number followed by `(Unreleased)`. This is the only entry you may modify. Never touch older, released entries.
2. **Preserve existing items unless proven wrong.** If the unreleased entry already contains items, evaluate whether each one is still accurate given the current state of the library. If you cannot confirm that an item is outdated or incorrect, leave it unchanged.
3. **Customer-facing only.** The changelog is for consumers of the NuGet package. Do not document changes to tests, scripts, workflows, CI pipelines, internal tooling, or anything else with no public impact.

## How to Determine What Changed

### Diffing the Public API Surface

The `api/` folder contains API listing files that describe the full public API surface of the library.

| Folder | Contents |
|--------|----------|
| `api/unreleased/` | The current (unreleased) API surface. |
| `api/{version}/` | A snapshot of the API surface as it was in the latest released version. The folder name is the version number (e.g., `2.10.0`). |

To find what changed in the public API:

1. Identify the versioned folder with the highest version number — this is the baseline.
2. Inside both the `unreleased` folder and the versioned folder, pick the API listing files for the **highest .NET target** available. For example, if you see files for `net10.0`, `net8.0`, and `netstandard2.0`, always use the `net10.0` files.
3. Diff the unreleased API listing against the versioned API listing. The delta represents the public API changes that must be documented in the unreleased changelog entry.

### Inspecting Commits for Non-API Changes

Some changes (e.g., bug fixes, behavioral changes, packaging changes) are not reflected in the API listings. To find these:

1. Identify the latest release tag in the git history. Tags follow the pattern `OpenAI_{version}` (e.g., `OpenAI_2.10.0`).
2. Inspect all commits since that tag: `git log OpenAI_{version}..HEAD --oneline`.
3. For each commit, determine whether it has public impact. Skip commits that only affect tests, scripts, workflows, internal tooling, or other non-public areas.
4. Document bug fixes, behavioral changes, and packaging/infrastructure changes that affect consumers.

### Evaluating the Nature of a Change

Not every code change belongs in the changelog. Apply this filter:

- **Include:** New public types, properties, methods, enum values; bug fixes that affect consumers; changes to the NuGet package contents or behavior; breaking changes to preview APIs.
- **Exclude:** Test changes; script/workflow changes; internal refactors with no public impact; TypeSpec or codegen changes that only restructure internals without changing the public API surface.

Use your judgment. If a TypeSpec or codegen change results in new or modified public API, it belongs in the changelog. If it only reorganizes internal plumbing, it does not.

## Version Header Format

Use an H2 (`##`) with the version number and release date in parentheses:

```markdown
## 2.10.0 (2026-04-03)
```

For unreleased versions, use `(Unreleased)` instead of a date:

```markdown
## 2.11.0 (Unreleased)
```

## Section Order

Every entry uses these H3 (`###`) sections in this exact order:

1. **Acknowledgments** — only present when community contributors are involved; omit entirely for released versions if there are none
2. **Features Added**
3. **Bugs Fixed**
4. **Other Changes**
5. **Breaking Changes in Preview APIs**

For unreleased stubs, include all sections as empty placeholders. For released versions, omit sections that have no content.

## Grouping by Namespace

Within each section, group entries by their `OpenAI.*` namespace or feature area. Use the namespace as a **plain-text** top-level bullet that ends with a colon:

```markdown
- OpenAI.Audio:
- OpenAI.Responses:
- OpenAI.VectorStores:
```

Rules for namespace grouping:

- Sort namespace groups alphabetically within each section.
- The namespace bullet ends with a colon and has **no text** after the colon on the same line.
- If a change is cross-cutting and not specific to one namespace, write it directly as a top-level bullet without a namespace prefix.

## Bullet Hierarchy

Use a **two-space indented** sub-bullet hierarchy under each namespace:

```markdown
- OpenAI.Audio:
  - Added support for audio transcriptions with speaker diarization.
```

For non-trivial features, use up to **three levels** of nesting:

1. **Namespace** — the top-level bullet (e.g., `- OpenAI.Audio:`).
2. **Capability summary** — a single sentence describing the user-facing capability in plain language. Start with "Added support for…" or "Enabled support for…".
3. **API detail** — concrete API additions (types, properties, methods, enum values) that implement the feature.

Example:

```markdown
- OpenAI.Audio:
  - Added support for audio transcriptions with speaker diarization.
    - Added the `Diarized` value to the `AudioTranscriptionFormat` extensible enum.
    - Added `KnownSpeakerNames` and `KnownSpeakerReferenceUris` properties to `AudioTranscriptionOptions`.
    - Added `TranscribeAudioDiarized` and `TranscribeAudioDiarizedAsync` methods to the `AudioClient`.
    - Added the following derived types of `StreamingAudioTranscriptionUpdate`:
      - `StreamingAudioTranscriptionTextSegmentUpdate`
```

Never nest deeper than three levels (namespace → capability → API detail). The API detail level may itself have a sub-list for enumerating derived types or protocol methods, but that is the maximum depth.

## Phrasing Conventions

Use these exact patterns when describing changes:

| Change | Pattern |
|--------|---------|
| New property | `` Added the `PropertyName` property to `TypeName`. `` |
| New properties (multiple) | `` Added `PropA` and `PropB` properties to `TypeName`. `` |
| New method(s) | `` Added `MethodName` and `MethodNameAsync` methods to the `ClientName`. `` |
| New type | `` Added the `TypeName` type. `` |
| New type with derived types | `` Added the `TypeName` type with the following derived types: `` followed by a sub-list |
| New derived types | `` Added the following derived types of `BaseType`: `` followed by a sub-list |
| New enum value | `` Added the `Value` value to the `EnumName` extensible enum. `` |
| New protocol methods | `` Added the following protocol methods to the `ClientName`: `` followed by a sub-list pairing sync/async: `` `Method` and `MethodAsync` `` |
| Bug fix | `` Fixed an issue where [observable symptom], causing a `ExceptionType`. [Root cause context]. `` |
| Rename | `` Renamed the `OldName` property/method of `TypeName` to `NewName`. `` |
| Type change | `` Changed the type of the `Name` property/parameter … from `OldType` to `NewType`. `` |
| Rename + type change | Combine both: `` Renamed the `OldName` property of `TypeName` to `NewName` and changed its type from `OldType` to `NewType`. `` |

## Voice and Tone

- **Third-person, passive/declarative.** The subject is the change itself: "Added…", "Fixed…", "Renamed…", "Changed…". Never "We added…" or "You can now…".
- **Terse and technical.** No filler words, no marketing language, no exclamation marks.
- **Present-tense result for bug fixes.** Describe what *was* broken and what exception or symptom it caused, not what is now fixed. Example: "Fixed an issue where X failed to Y, causing a `FormatException`."
- **Backtick all code symbols.** Property names, type names, method names, enum values, exception types — always in backticks.

## Section-Specific Guidance

### Features Added

Group by namespace. For each feature:

1. Write a summary bullet describing the user-facing capability.
2. Below it, list the concrete API additions as indented sub-bullets using the phrasing conventions above.

### Bugs Fixed

Each bug entry is a single bullet (under its namespace if applicable) following this template:

```markdown
- Fixed an issue where [what the user experienced], causing a `ExceptionType`. [Brief root-cause context].
```

Include enough detail for a user to recognize if the bug affected them, but keep it to one or two sentences.

### Other Changes

For infrastructure, dependency, or packaging changes that are not features or fixes but still affect consumers. Write as top-level bullets (no namespace grouping) in complete sentences:

```markdown
- Added `ConfigurationSchema.json` to the NuGet package via the MSBuild `JsonSchemaSegment` feature, enabling automatic JSON IntelliSense and validation for `appsettings.json` when configuring OpenAI clients.
```

### Breaking Changes in Preview APIs

Group by namespace just like Features Added. Each bullet states **what changed** and **from/to**:

- For renames: `` Renamed the `Old` … to `New`. ``
- For type changes: `` Changed the type of the `X` … from `A` to `B`. ``
- For combined rename + type change: combine in a single bullet.

These entries are factual and neutral — no apologies, no migration instructions.

## Formatting Rules

- Markdown H2 (`##`) for version headers, H3 (`###`) for section headers.
- Unordered lists (`-`) only — never numbered lists.
- Maximum three levels of nesting (namespace → feature summary → API detail).
- Always pair sync and async methods in a single bullet: `` `Method` and `MethodAsync` ``.
- Alphabetize namespace groups within a section.
- One blank line between sections; no blank lines between bullets within a section.
- No trailing periods on bullets that are sentence fragments listing types or values; use periods on full sentences (bug fixes, other changes).
