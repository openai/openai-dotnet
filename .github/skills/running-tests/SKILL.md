---
name: running-tests
description: Guide for running tests in the openai-dotnet repository. Use this when asked to run, debug, or validate tests, or when writing new tests. Explains test modes (Playback, Record, Live), how to identify recorded vs non-recorded tests, environment variable configuration, and what to do when recordings are missing or stale.
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

## Agent Rules

As the agent, only execute tests in `Playback` mode.

Do not run `Record` or `Live` mode yourself. Those paths require live credentials which you do not have.

Before running tests, always set:

```powershell
$env:CLIENTMODEL_TEST_MODE = "Playback"
$env:CLIENTMODEL_DISABLE_AUTO_RECORDING = "true"
```

If a recorded test needs new recordings or updated recordings, you must follow the instructions below to ask a human to capture them for you instead of trying to capture them yourself.

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

Recordings must be captured by a human.

Request recordings from a human before considering your work complete in either of the following cases:

1. You added a new recorded test (which implies its recording is missing because it has never been recorded before).
2. An existing recorded test fails in Playback mode because its recording is missing or stale (for example, if the recorded test was modified and the existing recording no longer matches).

When asking for recordings, always provide:

1. The link to the recording workflow
2. The exact `NUnit.Where` expression
3. A copy-pasteable `dotnet test` command

Use this template:

> Please record tests using the recording workflow:
> https://github.com/openai/openai-dotnet/actions/workflows/record-test.yml
>
> Use the following `NUnit.Where` expression:
> ```text
> test == 'Namespace.TestClass.TestMethodName'
> ```
>
> Alternatively, run the following command locally and push the recordings manually:
> ```powershell
> dotnet test ./tests/OpenAI.Tests.csproj --configuration Release --framework "net10.0" -- NUnit.Where="test == 'Namespace.TestClass.TestMethodName'"
> ```

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

## PR Body Requirement for Recording Requests

When recordings are required, include a **Recording Request** section in the PR description so reviewers can act without reading the full comment thread.

Include:

1. The recording workflow link
2. The exact `NUnit.Where` expression
3. A copy-pasteable `dotnet test` command

Use this PR description snippet:

~~~markdown
### Recording Request

Please record tests using the recording workflow:
https://github.com/openai/openai-dotnet/actions/workflows/record-test.yml

`NUnit.Where`:
```text
test == 'Namespace.TestClass.TestMethodName'
```

Local command:
```powershell
dotnet test ./tests/OpenAI.Tests.csproj --configuration Release --framework "net10.0" -- NUnit.Where="test == 'Namespace.TestClass.TestMethodName'"
```
~~~
