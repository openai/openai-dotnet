---
name: running-tests
description: Guide for running tests in the openai-dotnet repository. Use this when running, writing, modifying, debugging, or validating tests. Explains test modes (Playback, Record, Live), how to identify recorded vs non-recorded tests, environment variable configuration, and what to do when recordings are missing or stale.
---

# Running Tests

## Overview

Tests in this repository use the [Microsoft.ClientModel.TestFramework](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/core/Microsoft.ClientModel.TestFramework/README.md) and NUnit.

Many tests rely on **session recordings** — pre-recorded HTTP interactions stored in `tests/SessionRecords/` — so they can run without live service access. The repository defaults to `Playback` mode unless overridden in the environment.

## Test Modes

| Mode | Description |
|------|-------------|
| **Playback** | Tests run against pre-recorded session data in `tests/SessionRecords/`. No API key is required. |
| **Record** | Tests run against the live OpenAI API and produce or update session recordings. Requires an API key. |
| **Live** | Tests run against the live OpenAI API without producing or updating session recordings. Requires an API key. |

The mode is controlled by the `CLIENTMODEL_TEST_MODE` environment variable and accepts `Playback`, `Record`, or `Live`.

In Playback mode, the framework may attempt to auto-record when a session recording is missing or stale. Disable that behavior by explicitly setting `CLIENTMODEL_DISABLE_AUTO_RECORDING` to `true`.

## Preparing to Run Tests

Restore the repository's local .NET tools before running tests:

```bash
dotnet tool restore
```

This installs the `test-proxy` tool required by the test framework.

## Agent Rules

Agents and ordinary CI must run recorded client tests only in `Playback` mode with
auto-recording explicitly disabled. Never run `Record` or `Live` mode, even
when a recording is missing or stale. These settings avoid test runs that require
unavailable credentials; they are not a security boundary. Never request, read,
accept, or use real API keys or other credentials.

Every agent invocation of recorded client tests must explicitly set the test mode
and the separate auto-recording setting, and target `./tests/OpenAI.Tests.csproj`. The full
solution includes example projects that can bypass Playback and contact the live
service. In PowerShell:

```powershell
$env:CLIENTMODEL_TEST_MODE = "Playback"
$env:CLIENTMODEL_DISABLE_AUTO_RECORDING = "true"
dotnet test ./tests/OpenAI.Tests.csproj
```

In Bash:

```bash
CLIENTMODEL_TEST_MODE=Playback CLIENTMODEL_DISABLE_AUTO_RECORDING=true \
  dotnet test ./tests/OpenAI.Tests.csproj
```

Focused offline test projects that do not contact the OpenAI service or require
credentials are also permitted. For example, run the generator visitor suite with
`dotnet test codegen/generator/test/`. Playback settings do not apply to that
offline suite. Agents must still never test the full `OpenAI.slnx` solution.

If a recorded test needs new recordings or updated recordings, you must follow the instructions below to ask a human to capture them for you instead of trying to capture them yourself.

## Feature-Specific mTLS Tests

### Offline mTLS Tests (Playback)

Use this feature-specific command for deterministic, loopback-only mTLS tests.
Override inherited test-mode and auto-recording settings before running the
existing category filter:

```powershell
$env:CLIENTMODEL_TEST_MODE = "Playback"
$env:CLIENTMODEL_DISABLE_AUTO_RECORDING = "true"
dotnet test ./tests/OpenAI.Tests.csproj `
  --filter "TestCategory=MutualTls"
```

### Live mTLS Example (Authorized Humans Only)

This command is for explicitly authorized humans using an approved local
process only. It makes a live OpenAI request with real API and client-certificate
credentials. Agents and ordinary CI must never run this command.

Load `OPENAI_API_KEY`, `OPENAI_CLIENT_PFX_PATH`, and any required
`OPENAI_CLIENT_PFX_PASSWORD` from the environment or an approved secrets
manager; never inline, print, commit, or record credentials. Explicitly select
`Live` mode and disable auto-recording before invoking the exact example test:

```powershell
$env:CLIENTMODEL_TEST_MODE = "Live"
$env:CLIENTMODEL_DISABLE_AUTO_RECORDING = "true"
dotnet test ./examples/OpenAI.Examples.csproj `
  --framework net10.0 `
  -- NUnit.Where="test == 'OpenAI.Examples.MutualTlsExamples.Example01_MutualTlsAsync'"
```

## Identifying Recorded Tests vs Non-Recorded Tests

Treat these as practical indicators instead of a rigid checklist.

### Strong signals that a test is a recorded test:

- The test class inherits from `OpenAIRecordedTestBase`.
- The test method uses `[RecordedTest]`.
- The test gets clients through `GetProxiedOpenAIClient`.
- The test exercises real service behavior rather than mocked responses.

### Strong signals that a test is not a recorded test:

- The test class inherits from something other than `OpenAIRecordedTestBase` or has no base class.
- The test method uses plain `[Test]` instead of `[RecordedTest]`.
- The test uses mocked transports or handcrafted responses to simulate HTTP responses, such as `MockPipelineTransport`, `MockPipelineResponse`, or `GetClientOptionsWithMockResponse`.
- The test does not need to reach a live cloud service.

## When Recordings Are Missing or Stale

Recordings may only be captured locally by an explicitly authorized human.
Do not recommend, dispatch, or run `.github/workflows/record-test.yml`: it
stages, commits, and pushes recordings before required human inspection. Until
that workflow enforces human approval before any publication, an approved local
process is the only supported recording path.

An explicitly authorized human must load approved credentials from the
environment or an approved secrets manager before using `Record` mode. Use
`Live` mode only with separate explicit authorization. These commands are for
authorized humans only; agents and ordinary CI must never run them.

```powershell
$env:CLIENTMODEL_TEST_MODE = "Record"  # Use "Live" only when explicitly authorized.
dotnet test ./tests/OpenAI.Tests.csproj `
  -- NUnit.Where="test == 'Namespace.TestClass.TestMethodName'"
```

```bash
CLIENTMODEL_TEST_MODE=Record \
  dotnet test ./tests/OpenAI.Tests.csproj \
  -- NUnit.Where="test == 'Namespace.TestClass.TestMethodName'"
```

Before staging, committing, pushing, uploading, or otherwise publishing a
recording, the authorized human must inspect every automatically sanitized
file. If sensitive data remains, add or improve an enabled deterministic
sanitizer and regenerate the recording, or report the gap before publication.
Never manually sanitize a recording or upload raw recording artifacts.

Request recordings from a human before considering your work complete in either of the following cases:

1. You added a new recorded test (which implies its recording is missing because it has never been recorded before).
2. An existing recorded test fails in Playback mode because its recording is missing or stale (for example, if the recorded test was modified and the existing recording no longer matches).

When asking for recordings, always provide:

1. The authorized-local-only and pre-publication review requirement
2. The exact `NUnit.Where` expression
3. A command clearly marked for explicitly authorized humans only

You must use this template:

> An authorized maintainer must generate these recordings locally using
> approved credentials. Do not use the current automated recording workflow.
>
> Use the following `NUnit.Where` expression:
> ```text
> test == 'Namespace.TestClass.TestMethodName'
> ```
>
> Run only after explicit authorization:
> ```powershell
> $env:CLIENTMODEL_TEST_MODE = "Record"
> dotnet test ./tests/OpenAI.Tests.csproj --configuration Release --framework "net10.0" -- NUnit.Where="test == 'Namespace.TestClass.TestMethodName'"
> ```
>
> Before staging, committing, pushing, uploading, or otherwise publishing any
> recording, inspect every automatically sanitized file. If sensitive data
> remains, add or improve an enabled deterministic sanitizer and regenerate the
> recording, or report the gap to the core team. Never manually sanitize a
> recording or upload raw recording artifacts.

Use `NUnit.Where` for recording requests even when `dotnet test --filter` would work locally. `NUnit.Where` is the contract used by the recording workflow. Prefer `test == ...` because it matches the exact discovered NUnit test name and avoids ambiguity. For NUnit fixture-parameterized tests such as classes constructed with `bool isAsync`, the discovered test names may include fixture arguments like `(True)` or `(False)`.

For a single ordinary test, use:

```text
test == 'Namespace.TestClass.TestMethodName'
```

If you need one exact fixture instance, use:

```text
test == 'Namespace.TestClass(True).TestMethodName'
```

Example for async fixture instance of the `GenerateSingleEmbedding` recorded test:

```text
test == 'OpenAI.Tests.Embeddings.EmbeddingsTests(True).GenerateSingleEmbedding'
```

If multiple tests need recording, combine them in a single `NUnit.Where` expression:

```text
(test == 'Namespace.TestClass.TestA') or (test == 'Namespace.TestClass.TestB')
```

Example for both fixture instances of the `GenerateSingleEmbedding` recorded test:

```text
(test == 'OpenAI.Tests.Embeddings.EmbeddingsTests(True).GenerateSingleEmbedding') or (test == 'OpenAI.Tests.Embeddings.EmbeddingsTests(False).GenerateSingleEmbedding')
```
