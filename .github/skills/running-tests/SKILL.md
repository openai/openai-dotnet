---
name: running-tests
description: Guide for running tests in the openai-dotnet repository. Use this when asked to run, debug, or validate tests, or when writing new tests. Explains test modes (Playback, Record, Live), how to identify recorded vs non-recorded tests, environment variable configuration, and what to do when recordings are missing or outdated.
---

# Running Tests

## Overview

Tests in this repository use the [Microsoft.ClientModel.TestFramework](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/core/Microsoft.ClientModel.TestFramework/README.md) and NUnit. Many tests rely on **session recordings** — pre-recorded HTTP interactions stored in `tests/SessionRecords/` — so they can run without hitting a live API.

## Test Modes

| Mode | Description |
|------|-------------|
| **Playback** | Tests run against pre-recorded session data in `tests/SessionRecords/`. No API key required. |
| **Record** | Tests run against the live OpenAI API and record HTTP interactions for later playback. Requires an API key. |
| **Live** | Tests run against the live OpenAI API without recording. Requires an API key. |

The mode is controlled by the `CLIENTMODEL_TEST_MODE` environment variable (`Playback`, `Record`, or `Live`).

In Playback mode, if an existing session record does not match the test's requests, the framework will automatically attempt to re-record the test against the live API. To disable this auto-recording behavior, set `CLIENTMODEL_DISABLE_AUTO_RECORDING` to `true`.

## Identifying Recorded Tests vs Non-Recorded Tests

### A test IS a recorded test if **all** of the following are true:

- The test needs to hit a live cloud service.
- The test method has the `[RecordedTest]` attribute.
- The test class extends `OpenAIRecordedTestBase`.
- The test obtains a client via `GetProxiedOpenAIClient`.

### A test is NOT a recorded test if **any** of the following are true:

- The test does not need to hit a live cloud service.
- The test method has the plain `[Test]` attribute instead of `[RecordedTest]`.
- The test class extends `ClientTestBase` (or has no special test base class).
- The test uses `MockPipelineTransport`, `MockPipelineResponse`, or `GetClientOptionsWithMockResponse` to simulate HTTP responses.

## Running Tests as an Agent

**You can ONLY run tests in Playback mode.** You cannot run tests in Live or Record mode because those require a live API key and access to the OpenAI API.

Before running tests, always set the following environment variables:

```powershell
$env:CLIENTMODEL_TEST_MODE = "Playback"
$env:CLIENTMODEL_DISABLE_AUTO_RECORDING = "true"
```

## When Recordings Are Missing or Outdated

Recordings must be captured by a human. **You cannot capture recordings yourself.** There are two scenarios where you must request recordings:

1. **You wrote new recorded tests.** You must proactively request a human to record them as part of your workflow — do not wait for tests to fail. New recorded tests will not have session recordings and cannot pass in Playback mode until recordings are captured. Request recordings before considering your work complete.

2. **Existing recorded tests fail in Playback mode** because recordings are missing or outdated (e.g., the tests were modified and the existing recordings no longer match). Request a human to re-record the affected tests.

When requesting recordings, provide the exact `NUnit.Where` expression so the human can copy and paste it directly. Format the request like this:

> Please record tests using the following `NUnit.Where` expression:
> ```
> test =~ ".*Namespace\\.TestClass.*" and test =~ ".*TestMethodName$"
> ```

Use `NUnit.Where` for all recording requests. It works for ordinary tests and for NUnit fixture-parameterized tests such as classes constructed with `bool isAsync`, where the underlying test names may include fixture arguments like `(True)` or `(False)`.

If you need to run or record only one specific fixture instance, use an exact `test == ...` selector instead:

> Please record tests using the following `NUnit.Where` expression:
> ```
> test == "Namespace.TestClass(True).TestMethodName"
> ```

If multiple tests need recording, combine them in a single `NUnit.Where` expression:

> Please record tests using the following `NUnit.Where` expression:
> ```
> (test =~ ".*Namespace\\.TestClass.*" and test =~ ".*TestA$") or (test =~ ".*Namespace\\.TestClass.*" and test =~ ".*TestB$")
> ```

For example, to record both `GenerateSingleEmbedding` fixture instances from `EmbeddingsTests`, use:

> Please record tests using the following `NUnit.Where` expression:
> ```
> test =~ ".*OpenAI\\.Tests\\.Embeddings\\.EmbeddingsTests.*" and test =~ ".*GenerateSingleEmbedding$"
> ```
