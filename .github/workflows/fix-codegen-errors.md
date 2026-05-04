---
name: Fix Codegen Errors After Generator Update
description: >
  Iteratively fixes TypeSpec codegen errors introduced by a TypeSpec generator version update,
  exports the updated API surface, and opens a pull request targeting the generator update branch.

on:
  workflow_run:
    workflows: ["Update TypeSpec Generator Version"]
    types: [completed]
    branches: [main]
  workflow_dispatch:
    inputs:
      branch:
        description: >
          TypeSpec update branch to fix (e.g., typespec/update-http-client-csharp-1.0.0-alpha.20250101.1).
          Leave empty to auto-detect from the latest open PR with a matching branch name.
        required: false
        type: string

timeout-minutes: 30

if: >
  github.event_name == 'workflow_dispatch' ||
  github.event.workflow_run.conclusion != 'success'

permissions:
  contents: read
  pull-requests: read
  actions: read
  issues: read

checkout:
  fetch-depth: 0
  fetch: ["*"]

steps:
  - name: Setup .NET SDK
    uses: actions/setup-dotnet@c2fa09f4bde5ebb9d1777cf28262a3eb3db3ced7 # v5.2.0
    with:
      global-json-file: global.json
  - name: Setup Node.js
    uses: actions/setup-node@53b83947a5a98c8d113130e565377fae1a50d02f # v6.3.0
    with:
      node-version: '22.x'
  - name: Configure git identity
    run: |
      git config user.name "github-actions[bot]"
      git config user.email "41898282+github-actions[bot]@users.noreply.github.com"

tools:
  github:
    mode: gh-proxy
    toolsets: [default, actions]
  bash: ["*"]
  edit: {}

network:
  allowed:
    - dotnet
    - node

safe-outputs:
  create-pull-request:
    draft: true
    preserve-branch-name: true
    allowed-base-branches:
      - "typespec/update-http-client-csharp-*"
    labels: [codegen]
---

# Fix TypeSpec Codegen Errors After Generator Update

You are a coding agent. Your task is to check out a recently created TypeSpec generator update
branch, fix any TypeSpec codegen errors, export the API surface, and open a pull request with
the fixes targeting that branch.

## Context

The "Update TypeSpec Generator Version" workflow periodically bumps the
`@typespec/http-client-csharp` package version and creates a branch named
`typespec/update-http-client-csharp-<version>` with a PR targeting `main`. After the version
bump, the TypeSpec code generation (`scripts/Invoke-CodeGen.ps1`) may fail due to breaking
changes in the new generator version.

Your job is to fix any such errors so the generated code compiles correctly with the new
generator version.

## Instructions

### Step 1: Check the triggering workflow run outcome

This workflow only runs when the **Update TypeSpec Generator Version** workflow concludes with
a non-`success` result, which covers two scenarios:

- **Actual failure** — the workflow failed before creating a branch (e.g., npm/git error).
  Check whether a `typespec/update-http-client-csharp-*` branch was recently created. If none
  exists, exit gracefully and note that the upstream workflow needs investigation.
- **Succeeded with issues** — the script created a PR but exited with code 1 due to warnings
  (PR title is prefixed with `"Succeeded with Issues:"`). Proceed to find and fix codegen errors.

Print the event context for reference:

```bash
echo "Triggered by: ${{ github.event_name }}"
echo "Workflow run conclusion: ${{ github.event.workflow_run.conclusion }}"
```

### Step 2: Identify the target branch

Determine which TypeSpec update branch to work on:

- **If triggered via `workflow_dispatch` with a `branch` input**: use
  `${{ github.event.inputs.branch }}`.
- **Otherwise**: find the most recently created open PR whose head branch starts with
  `typespec/update-http-client-csharp-`:

```bash
gh pr list --state open --json number,headRefName,title,createdAt \
  --jq '[.[] | select(.headRefName | startswith("typespec/update-http-client-csharp-"))] | sort_by(.createdAt) | reverse | .[0]'
```

If no such PR is found, exit gracefully with a message that no update branch was found.

### Step 3: Check out the target branch

```bash
git checkout <BRANCH_NAME>
git status
```

Confirm the branch is checked out and contains the version bump in `codegen/package.json`.

### Step 4: Run code generation

Run `Invoke-CodeGen.ps1` and capture all output:

```bash
pwsh -NoProfile -File scripts/Invoke-CodeGen.ps1 2>&1 | tee /tmp/codegen-output.txt
CODEGEN_EXIT=${PIPESTATUS[0]}
echo "Codegen exit code: $CODEGEN_EXIT"
```

- **Exit code 0** → No codegen errors. Continue to the build verification step, then proceed to Step 6 (Export API) only if `dotnet build OpenAI.slnx` succeeds.
- **Non-zero exit code** → Errors need to be fixed. Continue to Step 5.

### Step 5: Fix codegen errors iteratively

Read `/tmp/codegen-output.txt`, identify the failing phase, apply the fix, and re-run. Repeat
until `Invoke-CodeGen.ps1` exits with code 0, up to **10 iterations**. After codegen succeeds,
run `dotnet build OpenAI.slnx` before proceeding to Step 6 so the generated code is validated
on both the "no errors" and "fixed errors" paths.

**Triage by phase:**
- `npm ci` or `npm run build` failure → **Category 4** (npm/plugin)
- `tsp compile` error referencing a namespace → **Category 1** (prohibited-namespace)
- `tsp compile` error for a missing type → **Category 2** (unresolved reference)
- `tsp compile` error on a decorator → **Category 3** (client TSP)
- Codegen succeeds but `dotnet build` fails → **Category 5** (post-generation build)

> **Ground rule:** Never modify `specification/base/` — these are upstream copies. All fixes go
> in `specification/client/` or `src/Custom/`.

#### Category 1 — `prohibited-namespace` errors

**Symptom:** Compiler error naming a type with `prohibited-namespace`.

**Cause:** A TypeSpec type landed in the root `OpenAI` namespace but has no `[CodeGenType]` stub
to place it in the correct area namespace (e.g., `OpenAI.Audio`).

**Fix:**
1. Identify the type name from the error message.
2. Determine visibility: types prefixed with `Internal` are internal; others are public.
3. Add a stub in the correct file:
   - Internal → `src/Custom/{Area}/Internal/GeneratorStubs.cs`
   - Public → `src/Custom/{Area}/GeneratorStubs.cs`

```csharp
// Internal example
[CodeGenType("SomeNewInternalType")] internal partial class InternalSomeNewInternalType { }

// Public example
[CodeGenType("SomeNewPublicType")] public partial class SomeNewPublicType { }
```

#### Category 2 — Missing type / unresolved reference errors

**Symptom:** `error type-not-found: Type "Foo" is not defined` or similar.

**Cause:** The base spec references a type from `common/` that hasn't been copied locally yet.

**Fix:**
1. Search locally for the type definition:
   ```powershell
   Select-String -Path "specification/base/typespec/common/*.tsp" -Pattern "model Foo|union Foo|enum Foo|alias Foo|scalar Foo"
   ```
2. If not found, retrieve it from upstream `microsoft/openai-openapi-pr` →
   `packages/openai-typespec/src/common/`.
3. Copy **only the specific type definition** into the local common file — do not copy entire
   files.

#### Category 3 — Client TSP decorator errors

**Symptom:** Errors referencing `@@clientLocation`, `@@clientName`, `@@visibility`,
`@@alternateType`, or `@@usage` decorators.

**Cause:** A type or operation was renamed/removed in the new base spec, but the client TSP still
references the old name.

**Fix:**
1. Open `specification/client/{area}.client.tsp`.
2. Find the stale reference named in the error.
3. Update it to match the new name from the base spec, or remove the `@@clientLocation` line if
   the operation was removed.

#### Category 4 — npm / build errors (plugin compilation)

**Symptom:** Errors during `npm ci` or `npm run build` in the codegen plugin.

**Cause:** Version incompatibilities after a generator update, or breaking API changes in the
TypeSpec SDK.

**Fix:**
1. Check `codegen/package.json` for version mismatches.
2. Delete `node_modules` only, then reinstall from the existing lockfile:
   ```powershell
   npm ci
   npm run build -w codegen
   ```
3. Do not delete or regenerate `package-lock.json` as part of this workflow fix path.
4. If `codegen/generator/src/` has TypeScript compile errors, fix the TypeScript visitor code.

#### Category 5 — Post-generation build errors (`dotnet build`)

**Symptom:** `Invoke-CodeGen.ps1` succeeds but `dotnet build OpenAI.slnx` fails.

**Cause:** Generated C# code references renamed/removed custom types, or numeric type conversions
need updating.

**Fix:**
1. Check for renamed types → update `[CodeGenType]` attributes in `GeneratorStubs.cs`.
2. Check for numeric type issues → update exclusion lists in
   `codegen/generator/src/Visitors/NumericTypesVisitor.cs`.
3. Check custom code in `src/Custom/{Area}/` for broken references.

---

After each fix, re-run codegen:

```bash
pwsh -NoProfile -File scripts/Invoke-CodeGen.ps1 2>&1 | tee /tmp/codegen-output.txt
CODEGEN_EXIT=$?
echo "Codegen exit code: $CODEGEN_EXIT"
```

Once codegen succeeds, verify the .NET solution builds cleanly:

```bash
dotnet build OpenAI.slnx --configuration Release 2>&1 | tee /tmp/build-output.txt
echo "Build exit code: $?"
```

Fix any C# build errors before proceeding.

### Step 6: Export the API surface

```bash
pwsh -NoProfile -File scripts/Export-Api.ps1 2>&1
echo "Export-Api exit code: $?"
```

### Step 7: Check for changes

```bash
git status
git diff --stat
```

If there are **no changes** (codegen and API export produced no modifications to tracked files),
exit gracefully without creating a PR — the new generator version may be fully
backwards-compatible.

If there are changes, check whether any `api/` files were modified:

```bash
git diff --name-only | grep '^api/'
```

API surface changes (additions, removals, or modifications in `api/*.cs`) do **not** block PR
creation — they may or may not be expected depending on the generator update. Capture a diff of
each changed `api/` file so you can explain the changes in the PR description:

```bash
git diff -- 'api/*.cs'
```

For each changed `api/` file, note:
- Which types or members were **added** (e.g., new model introduced by the generator update).
- Which types or members were **removed** or **renamed** (e.g., type renamed upstream).
- Which types or members were **modified** (e.g., property type changed).

### Step 8: Create a pull request

If there are changes, output a `create_pull_request` action targeting the typespec update branch.
When `api/` files changed, include an **API Changes** section that explains the cause of each
change based on the diff captured in Step 7:

```json
{
  "type": "create_pull_request",
  "title": "fix: codegen fixes for <BRANCH_NAME>",
  "body": "This PR fixes TypeSpec codegen errors introduced by the TypeSpec generator version update.\n\n**Base branch**: `<BRANCH_NAME>`\n\n## Changes\n\n<concise summary of the files changed and why>\n\n## API Changes\n\n<If api/ files were modified, list each change and its likely cause, e.g.:\n- `OpenAI.net8.0.cs`: Added `SomeNewType` — new model introduced by the generator update.\n- `OpenAI.net8.0.cs`: Removed `OldType` — type was renamed to `NewType` in the base spec.\nIf no api/ files were modified, write: None.>\n\n## Verification\n\n- `scripts/Invoke-CodeGen.ps1` ran successfully after fixes.\n- `scripts/Export-Api.ps1` ran successfully.",
  "branch": "typespec/fix-codegen-<VERSION>",
  "base": "<BRANCH_NAME>"
}
```

Where:
- `<BRANCH_NAME>` is the TypeSpec update branch (e.g., `typespec/update-http-client-csharp-1.0.0-alpha.20250101.1`)
- `<VERSION>` is the version suffix extracted from the branch name (e.g., `1.0.0-alpha.20250101.1`)
- `base` must be set to the TypeSpec update branch so the PR stacks on top of the generator update PR
