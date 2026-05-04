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
CODEGEN_EXIT=$?
echo "Codegen exit code: $CODEGEN_EXIT"
```

- **Exit code 0** → No codegen errors. Skip to Step 6 (Export API).
- **Non-zero exit code** → Errors need to be fixed. Continue to Step 5.

### Step 5: Fix codegen errors iteratively

Analyze the error output at `/tmp/codegen-output.txt` and fix all errors. Repeat until
`Invoke-CodeGen.ps1` exits with code 0, up to **10 iterations**.

#### Error Categories

Use the error message to identify the category and apply the appropriate fix:

| Category | Symptoms | Fix |
|---|---|---|
| **TypeSpec compiler error** | `error` lines referencing `.tsp` files | Fix syntax or add `@@suppress("<rule-id>", "reason")` in the relevant `specification/client/` file |
| **Missing `@@clientLocation`** | `error: Operation ... missing clientLocation` | Add `@@clientLocation(<ClientName>)` on the operation in `specification/client/<area>/operations.tsp` |
| **Type union error** | `error: ... union ... not supported` | Replace inline unions (`string \| SomeType`) with a named `@discriminator` model or a declared union |
| **Deprecated TypeSpec API** | `warning/error: ... is deprecated` | Update to the replacement API named in the error message |
| **C# build error in generated code** | `error CS...` after codegen succeeds | Update stubs in `src/Custom/` (e.g., `GeneratorStubs.cs`) to match renamed or restructured generated types |

#### Rules

- **Never modify `specification/base/`** — these are upstream copies. All fixes go in
  `specification/client/` or `src/Custom/`.
- Add `@@suppress` directives only in `specification/client/`, not in `specification/base/`.
- Use discriminators instead of type unions.

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

### Step 8: Create a pull request

If there are changes, output a `create_pull_request` action targeting the typespec update branch:

```json
{
  "type": "create_pull_request",
  "title": "fix: codegen fixes for <BRANCH_NAME>",
  "body": "This PR fixes TypeSpec codegen errors introduced by the TypeSpec generator version update.\n\n**Base branch**: `<BRANCH_NAME>`\n\n## Changes\n\n<concise summary of the files changed and why>\n\n## Verification\n\n- `scripts/Invoke-CodeGen.ps1` ran successfully after fixes.\n- `scripts/Export-Api.ps1` ran successfully.",
  "branch": "typespec/fix-codegen-<VERSION>",
  "base": "<BRANCH_NAME>"
}
```

Where:
- `<BRANCH_NAME>` is the TypeSpec update branch (e.g., `typespec/update-http-client-csharp-1.0.0-alpha.20250101.1`)
- `<VERSION>` is the version suffix extracted from the branch name (e.g., `1.0.0-alpha.20250101.1`)
- `base` must be set to the TypeSpec update branch so the PR stacks on top of the generator update PR
