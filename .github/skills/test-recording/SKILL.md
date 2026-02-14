---
name: test-recording
description: Record or re-record NUnit test session recordings for tests that connect to the live OpenAI service. Use this skill when you need to record a test, re-record a test, update a session recording, or when working with tests that have the [RecordedTest] attribute.
---

# Test Recording Skill

This skill defines the procedure for recording or re-recording NUnit tests that connect to the live OpenAI service. Follow each section in order.

## Section 1: Triage — Does the test need recording?

Inspect the test file to determine whether the test connects to the live OpenAI service or runs entirely offline.

### Tests that need recording

A test **needs recording** if **all** of the following are true:

- The test class extends `OpenAIRecordedTestBase`.
- The test obtains a client via `GetProxiedOpenAIClient` (or a helper that wraps it, such as `GetTestClient`).
- The test method has the `[RecordedTest]` attribute.

A **newly added** `[RecordedTest]` always needs recording. An **existing** `[RecordedTest]` only needs re-recording if the request to the live service changed (for example: a property was added or modified, or the library behavior changed in a way that makes the HTTP requests in the test look different). See Section 2 for how to check.

### Tests that do NOT need recording

A test **does not need recording** if **any** of the following are true:

- The test class extends `ClientTestBase` (or has no special test base class).
- The test uses `MockPipelineTransport`, `MockPipelineResponse`, or `GetClientOptionsWithMockResponse` to simulate HTTP responses.
- The test method has the plain `[Test]` attribute instead of `[RecordedTest]`.

If the test does not need recording, confirm that it uses `[Test]` (not `[RecordedTest]`) and **stop — nothing else to do**.

### Fix mismatched attributes

- If a test connects to the live service but has `[Test]`, replace `[Test]` with `[RecordedTest]`.
- If a test runs entirely offline but has `[RecordedTest]`, replace `[RecordedTest]` with `[Test]`.

## Section 2: Check if re-recording is needed

This section applies only to **existing** `[RecordedTest]` tests that already have session recordings. For newly added tests, skip to Section 3.

1. Set the environment variables:

   ```powershell
   $env:CLIENTMODEL_TEST_MODE = "Playback"
   $env:CLIENTMODEL_DISABLE_AUTO_RECORDING = "true"
   ```

2. Run the test:

   ```powershell
   dotnet test tests/OpenAI.Tests.csproj --filter "FullyQualifiedName~{TestMethodName}"
   ```

3. Evaluate the result:
   - If the test **passes**, the existing recording is still valid. **Stop — no re-recording needed.**
   - If the test **fails**, the recording is outdated. Proceed to Section 3.

## Section 3: Record the test

### Prerequisites

Before attempting to record, verify that the `OPENAI_API_KEY` environment variable is set:

```powershell
if (-not $env:OPENAI_API_KEY) {
    Write-Error "OPENAI_API_KEY is not set. Recording requires a valid API key."
}
```

If `OPENAI_API_KEY` is not set, **stop immediately** and tell the user that a valid API key is required. Do not attempt to record any tests.

### Recording procedure

Record tests **one at a time**. For each test:

1. Set the environment variable:

   ```powershell
   $env:CLIENTMODEL_TEST_MODE = "Record"
   ```

2. Run the test:

   ```powershell
   dotnet test tests/OpenAI.Tests.csproj --filter "FullyQualifiedName~{TestMethodName}"
   ```

3. Evaluate the result:
   - If the test **passes**, proceed to Section 4 to validate the recording.
   - If the test **fails**, analyze the error output:
     - **Environmental failure** (HTTP 401 Unauthorized, HTTP 403 Forbidden, service outage, DNS resolution failure, rate limiting / HTTP 429, etc.): **stop all recording immediately** — do not retry this test or attempt any remaining tests. Report the issue clearly to the user.
     - **Non-environmental failure** (assertion error, code bug, unexpected response shape): attempt to fix the problem, then retry. You may retry fixing the problem up to **3 total attempts** per test (including the initial attempt). If the test still fails after 3 attempts, stop trying to record this test and any remaining tests to avoid wasting resources, and report the failure to the user.

## Section 4: Validate the recording

After a test is successfully recorded, validate the recording by running the test in Playback mode.

1. Set the environment variables:

   ```powershell
   $env:CLIENTMODEL_TEST_MODE = "Playback"
   $env:CLIENTMODEL_DISABLE_AUTO_RECORDING = "true"
   ```

2. Confirm that the session recording files exist. Recorded tests produce two files (one for the synchronous run and one for the asynchronous run):

   - `tests/SessionRecords/{ClassName}/{MethodName}.json`
   - `tests/SessionRecords/{ClassName}/{MethodName}Async.json`

   For parameterized tests, the file names include the parameter values:

   - `tests/SessionRecords/{ClassName}/{MethodName}({ParameterValue}).json`
   - `tests/SessionRecords/{ClassName}/{MethodName}({ParameterValue})Async.json`

3. Re-run the test in Playback mode:

   ```powershell
   dotnet test tests/OpenAI.Tests.csproj --filter "FullyQualifiedName~{TestMethodName}"
   ```

4. Evaluate the result:
   - If the test **passes** and the session files are present, the recording is valid. Proceed to record the next test (go back to Section 3 for the next test).
   - If the test **fails**, go back to Section 3 and retry. This failed validation counts toward the 3-attempt limit for the test.
