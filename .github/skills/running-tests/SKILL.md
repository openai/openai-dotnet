---
name: running-tests
description: Guide for running tests in the openai-dotnet repository. Use this when asked to run, debug, or validate tests. Explains test modes (Playback, Record, Live), how to identify recorded vs non-recorded tests, environment variable configuration, and what to do when recordings are missing or outdated.
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

Then run tests with:

```powershell
dotnet test OpenAI.slnx
```

Or to run specific tests:

```powershell
dotnet test OpenAI.slnx --filter "FullyQualifiedName~YourTestName"
```

## When Tests Fail Due to Missing or Outdated Recordings

If one or more recorded tests fail because no recordings exist (e.g., the tests are new and have not been recorded yet) or because the recordings are outdated (e.g., the tests were modified), new recordings must be captured. **You cannot capture recordings yourself** — you must request a human to do it.

When requesting recordings, provide the exact `dotnet test --filter` expression so the human can copy and paste it directly. Format the request like this:

> Please record or re-record tests using the following filter expression:
> ```
> FullyQualifiedName=Namespace.TestClass.TestMethodName
> ```

If multiple tests need recording, combine them in a single filter expression:

> Please record or re-record tests using the following filter expression:
> ```
> FullyQualifiedName=Namespace.TestClass.TestA|FullyQualifiedName=Namespace.TestClass.TestB
> ```
