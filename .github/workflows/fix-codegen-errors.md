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
          Leave empty to auto-detect from the latest open PR with a typespec/update-http-client-csharp-* branch name.
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
  fetch:
    - "typespec/update-http-client-csharp-*"

steps:
  - name: Setup .NET SDK
    uses: actions/setup-dotnet@c2fa09f4bde5ebb9d1777cf28262a3eb3db3ced7 # v5.2.0
    with:
      global-json-file: global.json
  - name: Setup Node.js
    uses: actions/setup-node@53b83947a5a98c8d113130e565377fae1a50d02f # v6.3.0
    with:
      node-version: '22.x'
  - name: Detect target branch and validate trigger
    id: detect-branch
    run: |
      set -euo pipefail

      echo "=== Trigger context ==="
      echo "Event: ${{ github.event_name }}"
      echo "Workflow run conclusion: ${{ github.event.workflow_run.conclusion }}"

      # --- Resolve target branch ---
      BRANCH="${{ github.event.inputs.branch }}"

      if [ -n "$BRANCH" ]; then
        echo "Using branch from workflow_dispatch input: $BRANCH"
      else
        echo "Auto-detecting target branch from open PRs..."
        PR_JSON=$(gh pr list --state open \
          --json number,headRefName,title,createdAt \
          --jq '[.[] | select(.headRefName | startswith("typespec/update-http-client-csharp-"))] | sort_by(.createdAt) | reverse | .[0]')

        if [ -z "$PR_JSON" ] || [ "$PR_JSON" = "null" ]; then
          echo "::error::No open PR found with a typespec/update-http-client-csharp-* branch."
          exit 1
        fi

        BRANCH=$(echo "$PR_JSON" | jq -r '.headRefName')
        PR_TITLE=$(echo "$PR_JSON" | jq -r '.title')
        PR_NUMBER=$(echo "$PR_JSON" | jq -r '.number')
        echo "Found PR #${PR_NUMBER}: ${PR_TITLE}"
        echo "Target branch: $BRANCH"

        # --- Gate: only continue for "Succeeded with Issues" PRs on workflow_run triggers ---
        if [ "${{ github.event_name }}" = "workflow_run" ]; then
          if echo "$PR_TITLE" | grep -q "^Succeeded with Issues:"; then
            echo "PR title indicates codegen issues to fix. Continuing."
          else
            echo "::error::PR exists but title does not start with 'Succeeded with Issues:'."
            echo "This may be an actual failure or the PR was already fixed. Exiting."
            exit 1
          fi
        fi
      fi

      echo "target-branch=$BRANCH" >> "$GITHUB_OUTPUT"
      echo "Target branch resolved: $BRANCH"
  - name: Check out target branch and verify
    run: |
      set -euo pipefail

      BRANCH="${{ steps.detect-branch.outputs.target-branch }}"

      if [ -z "$BRANCH" ]; then
        echo "::error::No target branch resolved from previous step."
        exit 1
      fi

      echo "Checking out branch: $BRANCH"

      if ! git checkout "$BRANCH"; then
        echo "::error::Failed to check out branch '$BRANCH'. It may not exist or was not fetched."
        exit 1
      fi

      git log --oneline -3
      git status

      # Verify the branch contains a version update in codegen/package.json
      if ! git diff main -- codegen/package.json | grep -q '@typespec/http-client-csharp'; then
        echo "::warning::No @typespec/http-client-csharp version change detected in codegen/package.json vs main."
        echo "The branch may not contain the expected generator update. Proceeding anyway."
      fi

      echo "Branch '$BRANCH' checked out and verified."

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
    max_patch_size: 262144
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

### Step 1: Run code generation

Run `Invoke-CodeGen.ps1` and capture all output:

```bash
pwsh -NoProfile -File scripts/Invoke-CodeGen.ps1 2>&1 | tee /tmp/codegen-output.txt
CODEGEN_EXIT=${PIPESTATUS[0]}
echo "Codegen exit code: $CODEGEN_EXIT"
```

- **Exit code 0** → No codegen errors. Continue to Step 3 (Build verification).
- **Non-zero exit code** → Errors need to be fixed. Continue to Step 2.

### Step 2: Fix codegen errors iteratively

Read `/tmp/codegen-output.txt`, identify the failing phase, and apply the appropriate fix.


**Triage by phase:**
- `npm ci` or `npm run build` failure → **Category 4** (npm/plugin)
- `tsp compile` error referencing a namespace → **Category 1** (prohibited-namespace)
- `tsp compile` error for a missing type → **Category 2** (unresolved reference)
- `tsp compile` error on a decorator → **Category 3** (client TSP)
- Codegen succeeds but `dotnet build` fails → **Category 5** (post-generation build)

> **Ground rule:** Never modify `specification/base/` — these are upstream copies. All fixes go
> in `specification/client/`, `OpenAI/src/Custom/`, or `OpenAI.Responses/src/Custom/`.

Detailed fix instructions for each category are in the
[fixing-codegen-errors skill](/.github/skills/fixing-codegen-errors/SKILL.md):

| Category | Fix guide |
|----------|-----------|
| 1 — Prohibited namespace | [prohibited-namespace.md](/.github/skills/fixing-codegen-errors/prohibited-namespace.md) |
| 2 — Missing type | [missing-type.md](/.github/skills/fixing-codegen-errors/missing-type.md) |
| 3 — Client TSP decorators | [client-tsp-decorators.md](/.github/skills/fixing-codegen-errors/client-tsp-decorators.md) |
| 4 — npm/plugin build | [npm-plugin-build.md](/.github/skills/fixing-codegen-errors/npm-plugin-build.md) |
| 5 — Post-generation build | [post-generation-build.md](/.github/skills/fixing-codegen-errors/post-generation-build.md) |

> **After applying fixes, always go back to Step 1** to re-run `Invoke-CodeGen.ps1` and
> verify the fixes worked. Repeat this Step 2 → Step 1 cycle until codegen exits with code 0.

### Step 3: Build verification

Verify the .NET solution builds cleanly:

```bash
dotnet build OpenAI.slnx --configuration Release 2>&1 | tee /tmp/build-output.txt
BUILD_EXIT=${PIPESTATUS[0]}
echo "Build exit code: $BUILD_EXIT"
```

If the build fails, apply the appropriate fix
([Category 5 — post-generation build](/.github/skills/fixing-codegen-errors/post-generation-build.md))
and go back to Step 1 to re-run codegen and build again. Once both codegen and build succeed, proceed to Step 4.

### Step 4: Export the API surface

```bash
pwsh -NoProfile -File scripts/Export-Api.ps1 2>&1
EXPORT_EXIT=$?
echo "Export-Api exit code: $EXPORT_EXIT"
```

If `Export-Api.ps1` exits with a non-zero code, do not proceed. Report the error and exit by running:

```bash
echo "::error::Export-Api.ps1 failed with exit code $EXPORT_EXIT"
exit 1
```

### Step 5: Check for changes

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

### Step 6: Create a pull request

If there are changes, commit them and create a pull request targeting the typespec update branch.
When `api/` files changed, include an **API Changes** section in the PR body that explains the
cause of each change based on the diff captured in Step 5.

Use the target branch from Step 1 (e.g., `typespec/update-http-client-csharp-1.0.0-alpha.20250101.1`)
as the **base branch** for the PR so it stacks on top of the generator update PR. Extract the
version suffix from the branch name (e.g., `1.0.0-alpha.20250101.1`) for the PR title.

The PR title must be: `Fix codegen errors for <VERSION>` (e.g.,
`Fix codegen errors for 1.0.0-alpha.20250101.1`).

Commit all changes to a new branch (e.g., `typespec/fix-codegen-<VERSION>`), push it, and open
a draft PR with `gh pr create` using `--base <TARGET_BRANCH>` and `--label codegen`. The PR
must target the typespec update branch (not `main`) so it stacks on top of the generator
update PR.
