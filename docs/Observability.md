## Observability with OpenTelemetry

> Note:
> OpenAI .NET SDK instrumentation is in development and is not complete. See [Available sources and meters](#available-sources-and-meters) section for the list of covered operations.

OpenAI .NET library is instrumented with distributed tracing and metrics using .NET [tracing](https://learn.microsoft.com/dotnet/core/diagnostics/distributed-tracing)
and [metrics](https://learn.microsoft.com/dotnet/core/diagnostics/metrics-instrumentation) API and supports [OpenTelemetry](https://learn.microsoft.com/dotnet/core/diagnostics/observability-with-otel).

OpenAI .NET instrumentation follows [OpenTelemetry Semantic Conventions for Generative AI systems](https://github.com/open-telemetry/semantic-conventions-genai/tree/main/docs/gen-ai).

### How to enable

The instrumentation is **experimental** - volume and semantics of the telemetry items may change.

To enable the instrumentation:

1. Set instrumentation feature-flag using one of the following options:

   - set the `OPENAI_EXPERIMENTAL_ENABLE_OPEN_TELEMETRY` environment variable to `"true"`
   - set the `OpenAI.Experimental.EnableOpenTelemetry` context switch to true in your application code when application
     is starting and before initializing any OpenAI clients. For example:

     ```csharp
     AppContext.SetSwitch("OpenAI.Experimental.EnableOpenTelemetry", true);
     ```

2. Enable OpenAI telemetry:

   ```csharp
   builder.Services.AddOpenTelemetry()
       .WithTracing(b =>
       {
           b.AddSource("OpenAI.*")
             ...
            .AddOtlpExporter();
       })
       .WithMetrics(b =>
       {
           b.AddMeter("OpenAI.*")
            ...
            .AddOtlpExporter();
       });
   ```

   Distributed tracing is enabled with `AddSource("OpenAI.*")` which tells OpenTelemetry to listen to all [ActivitySources](https://learn.microsoft.com/dotnet/api/system.diagnostics.activitysource) with names starting with `OpenAI.*`.

   Similarly, metrics are configured with `AddMeter("OpenAI.*")` which enables all OpenAI-related [Meters](https://learn.microsoft.com/dotnet/api/system.diagnostics.metrics.meter).

Consider enabling [HTTP client instrumentation](https://www.nuget.org/packages/OpenTelemetry.Instrumentation.Http) to see all HTTP client
calls made by your application including those done by the OpenAI SDK.
Check out [OpenTelemetry documentation](https://opentelemetry.io/docs/languages/net/getting-started/) for more details.

### Semantic convention version

By default, the instrumentation emits telemetry following [OpenTelemetry GenAI Semantic Conventions v1.27.0](https://github.com/open-telemetry/semantic-conventions/tree/v1.27.0/docs/gen-ai).

To opt in to the latest experimental GenAI semantic conventions supported by this library version, set the `OTEL_SEMCONV_STABILITY_OPT_IN` environment variable to include `gen_ai_latest_experimental` (comma-separated if combined with other values):

```
OTEL_SEMCONV_STABILITY_OPT_IN=gen_ai_latest_experimental
```

When this opt-in is enabled, the instrumentation emits attributes following the
[supported conventions](https://github.com/open-telemetry/semantic-conventions-genai/tree/8ffdf568e1b4391a99adb081db16e8102e36918e/docs/gen-ai).
Notable changes include:

- The `gen_ai.system` attribute is replaced by `gen_ai.provider.name`.
- Token usage uses the inference counters and per-operation histograms described below instead of `gen_ai.client.token.usage`.
- Responses telemetry identifies the API with `openai.api.type=responses` and records available response, conversation, service-tier, reasoning, output-type, and detailed token-usage attributes.
- Chat telemetry identifies the API with `openai.api.type=chat_completions` and records explicitly configured seed, output format, non-auto service tier, frequency/presence penalties, and reasoning effort. Available response service tiers, system fingerprints, and detailed token usage are recorded on spans as well as applicable metrics.
- Responses spans include `gen_ai.request.stream=true` for streaming creation and omit the attribute for non-streaming creation. Streaming creation also supports the chunk timing histograms and first-chunk span attribute described below.

The default behavior (without the opt-in) remains unchanged and continues to emit v1.27.0 conventions.

The metric contract was checked against the GenAI conventions at commit
[`8ffdf568e1b4391a99adb081db16e8102e36918e`](https://github.com/open-telemetry/semantic-conventions-genai/tree/8ffdf568e1b4391a99adb081db16e8102e36918e)
on September 24, 2026, including the [metric definitions](https://github.com/open-telemetry/semantic-conventions-genai/blob/8ffdf568e1b4391a99adb081db16e8102e36918e/model/gen-ai/metrics.yaml)
and [token metric definitions](https://github.com/open-telemetry/semantic-conventions-genai/blob/8ffdf568e1b4391a99adb081db16e8102e36918e/model/gen-ai/token-metrics.yaml).
This identifies the supported metric baseline, not adoption of every upstream span or event convention.
Both modes retain `gen_ai.client.operation.duration`. Only the default mode retains
`gen_ai.client.token.usage` with its input/output `gen_ai.token.type` dimension.
Requests without a model continue to emit a `chat` operation without `gen_ai.request.model`.

### Available sources and meters

The following sources and meters are available:

- `OpenAI.ChatClient` - records traces and metrics for `ChatClient` operations (except streaming and protocol methods which are not instrumented yet)
- `OpenAI.ResponsesClient` - records traces and metrics for `CreateResponse` and `CreateResponseStreaming` operations, including their asynchronous overloads. Protocol, retrieval (including streaming retrieval), cancellation, deletion, and input-item methods are not instrumented yet.

### Latest-mode token metrics

With `gen_ai_latest_experimental`, all instrumented Chat and Responses operations use the same
token metric contract, on their respective meters. All token instruments use integer values and
the unit `{token}`.

| Metric | Instrument | Measurement |
| --- | --- | --- |
| `gen_ai.client.inference.usage.input_tokens` | Counter | Total input tokens, including cached tokens. |
| `gen_ai.client.inference.usage.output_tokens` | Counter | Total output tokens, including reasoning tokens. |
| `gen_ai.client.inference.usage.cache_read.input_tokens` | Counter | The input-token subset read from the provider cache, when available. |
| `gen_ai.client.inference.usage.cache_write.input_tokens` | Counter | The input-token subset written to the provider cache, when available. |
| `gen_ai.client.inference.usage.reasoning.output_tokens` | Counter | The output-token subset used for reasoning, when available. |
| `gen_ai.client.inference.operation.input_tokens` | Histogram | One total input-token count per operation with reported usage. |
| `gen_ai.client.inference.operation.output_tokens` | Histogram | One total output-token count per operation with reported usage. |

Usage counters include `gen_ai.token.modality`. Reported modality breakdowns are used where available;
tokens whose modality cannot be reliably determined use `unknown`, not an inferred `text` label.
Cache and reasoning counters are subsets of the totals, not additional consumption. They do not
inherit a modality breakdown from unrelated usage fields. Per-operation histograms are deliberately
not split by modality, since percentiles across modalities do not add up.

Measurements include the operation, provider, endpoint, and available request and response models.
Token and duration metrics also include available OpenAI response service tier and system fingerprint.
Response identifiers and content are not metric dimensions. Missing usage is not estimated from
content or stream chunks.
If a reported cache or reasoning subset exceeds its available corresponding total, latest-mode
spans and metrics omit that subset without changing the total. Subsets remain reportable when
the corresponding total is unavailable.

When migrating a latest-mode dashboard, use the usage counters for cumulative consumption and rates,
summing across modalities when a combined total is needed. Replace percentile queries over the old
`gen_ai.client.token.usage` input/output series with the corresponding per-operation histogram.
Do not add the cache or reasoning subsets to total usage, or add histogram sums to usage counters.
Latest-mode operations do not also emit the legacy token histogram. Default-mode dashboards are unchanged.

### Responses streaming lifecycle and timing

Streaming creation is lazy. Each enumeration starts its own operation when it sends the request;
creating a collection or disposing an enumerator before its first move emits no telemetry.
The operation uses the ambient parent at that time, preserves baggage, and parents HTTP work beneath
the Responses span. It restores the caller's current activity between enumeration calls.

The first `response.completed`, `response.failed`, or `response.incomplete` update ends the operation,
using the same status, finish-reason, service-tier, and usage mappings as non-streaming creation.
Duration and any supplied usage are recorded once. Duplicate terminal updates and subsequent stream
activity cannot emit another operation or token measurement. Usage is never estimated from chunks,
and missing usage remains absent. The SDK still yields updates and throws stream exceptions as before.

When no terminal response has been observed:

- Send, read, deserialization, and cancellation exceptions record the exception type (or HTTP status
  for `ClientResultException`) without the exception message.
- A streaming `error` update records the bounded error type `stream_error`, not its arbitrary code,
  message, or parameter.
- EOF or `[DONE]` records `incomplete_stream`; neither is treated as successful response completion.
- Early disposal of typed enumeration records `cancelled`, without manufacturing response usage.
- Raw-page enumeration remains unbuffered and does not decode events. In latest mode, its activity
  remains open after response handoff until the caller reads EOF, disposes the response stream,
  or encounters a read/disposal exception. Reads use the operation activity and restore the caller's
  current activity afterward. Merely ending raw enumeration does not end the activity. With the
  default conventions, the activity continues to end at raw-page handoff.
  Time while the caller retains an unread raw response is included in the latest-mode span duration.
  Latest-mode raw processing records operation duration once at that same boundary, including
  `error.type` for read/disposal exceptions and omitting it otherwise. This also works when only
  metrics are enabled. Raw processing does not emit token usage or chunk timing measurements, and does
  not infer provider status or finish reasons from uninterpreted content. Handoff and disposal
  without an observed error do not manufacture cancellation errors. Request-send failures are
  still reported. Raw-page callers still own disposal of the response.

In latest mode, typed stream failures, premature EOF, and early disposal record an `error` finish
reason when a final response was expected, including a successfully opened typed stream that
received no events. Request-send failures and raw-response handoff do not manufacture a generation
finish reason. Authoritative terminal finish reasons are preserved.

Latest-mode spans retain response model, identifier, service tier, and system fingerprint as those
values become available from response-bearing events. Later supplied values update earlier ones;
omitted fields, including on a terminal response, do not erase previously observed metadata.
Receiving metadata alone does not finalize the operation or emit token usage.

Always dispose enumerators (normally with `foreach` or `await foreach`) and caller-owned raw responses.
An abandoned raw response whose body was not read to EOF cannot guarantee completion telemetry.
Abandoned, undisposed enumerators cannot guarantee completion telemetry. Cancellation preserves existing stream behavior;
synchronous enumeration checks cancellation between events rather than interrupting a blocking read.

With `gen_ai_latest_experimental`, the following histograms are emitted in seconds, using a monotonic
clock and the recommended duration histogram bucket boundaries:

| Metric | Measurement |
| --- | --- |
| `gen_ai.client.operation.time_to_first_chunk` | Once, from lazy request start to the first parsed SSE event, including metadata or unknown events, not necessarily the first output token. No measurement is emitted if no event is received. |
| `gen_ai.client.operation.time_per_output_chunk` | Once per output chunk after the first output chunk, between parsed event arrival times. Text, refusal, reasoning, tool arguments/input/code, audio/transcript deltas, and partial images count as output. Completed output items without previously observed indexed deltas count as discrete output chunks; cumulative item completion after those deltas does not count again. |

These are protocol-level observations, not transport packet or token timings. Structural progress
events do not count as output. The SDK adds no buffering or read-ahead: application pauses between
enumeration calls can therefore contribute to observed duration and chunk intervals. Neither timing
histogram is emitted for non-streaming operations or in the default semantic-convention mode.
Timing measurements include the response model when it has been observed, including when the first
event supplies it. They are not buffered to attach metadata learned from later events. Latest-mode
duration measurements on interrupted streams retain model and service-tier metadata already observed.
Streaming spans record `gen_ai.response.time_to_first_chunk` from the same elapsed-time observation
as the histogram, including when only tracing is enabled.

### Exception logs

Latest-mode instrumentation can emit operation exception logs through the caller-supplied
`ClientLoggingOptions.LoggerFactory`. This uses the existing logging abstraction from
`System.ClientModel`; the OpenAI SDK does not create an OpenTelemetry logger provider or exporter.
For example, an application using the OpenTelemetry logging bridge and OTLP exporter can configure:

```csharp
using Microsoft.Extensions.Logging;
using OpenAI.Responses;
using OpenTelemetry.Logs;

using var loggerFactory = LoggerFactory.Create(logging =>
    logging.AddOpenTelemetry(options => options.AddOtlpExporter()));

var options = new ResponsesClientOptions
{
    ClientLoggingOptions = new()
    {
        LoggerFactory = loggerFactory,
    },
};
```

Use the same inherited configuration on `OpenAIClientOptions` for Chat or clients obtained from
`OpenAIClient`. Both the experimental
instrumentation switch and `gen_ai_latest_experimental` must be enabled. `EnableLogging=false`
disables these records; null retains the enabled default. Logger category/level filters also apply.
The caller owns the factory and must keep it alive for the clients' logging lifetime.

Operation categories are `OpenAI.ChatClient.Operations` and `OpenAI.ResponsesClient.Operations`.
The event name is `gen_ai.client.operation.exception`, severity is warning, and the body is the fixed
text `GenAI operation failed.` The record contains `exception.type`, not the raw exception instance,
message, stack trace, or request/response content. Omitting recommended exception message/stack
details and not passing the exception instance are deliberate privacy choices.
The contract follows the pinned [exception-log conventions](https://github.com/open-telemetry/semantic-conventions/blob/838e414e11e30bc4f67d733b2b0baf2887d12d31/docs/exceptions/exceptions-logs.md).

One record is emitted for a final escaping exception from an observed operation, not each retry.
This includes `ClientResultException` for API errors and rate limiting, following the
[GenAI-specific exception convention](https://github.com/open-telemetry/semantic-conventions-genai/blob/8ffdf568e1b4391a99adb081db16e8102e36918e/docs/gen-ai/gen-ai-exceptions.md).
Recovered retries produce no operation exception record. Provider outcomes that do not throw and
synthetic stream-completion errors remain represented by span status and metrics without
manufacturing exception objects.

When an operation activity exists, the record carries its context. Logging alone does not create
an activity: a record can instead carry the ambient caller's context or no trace context. An
exception after the streaming operation has already finalized does not produce another operation log.

Supplying a factory also changes System.ClientModel's existing pipeline-log routing from EventSource
to `ILogger`. The operation logger does not alter pipeline logging options, and
`EnableMessageLogging=false` can independently disable pipeline message logs. The type-only privacy
policy above applies to the new operation records, not to separately configured pipeline content
logging or existing Chat span status descriptions.

### OpenTelemetry data and privacy

Responses OpenTelemetry instrumentation records operational details such as model names, response and conversation identifiers, service tiers, token counts, finish reasons, and errors.

Responses instrumentation does not record prompts, generated output, instructions, tool definitions, tool arguments or results, metadata, safety identifiers, end-user identifiers, prompt cache keys, or multimodal payloads. The OpenTelemetry GenAI semantic conventions classify content attributes as opt-in. Enabling this library's experimental OpenTelemetry instrumentation does not opt in to content capture.

## Telemetry and privacy

Separately from the OpenTelemetry instrumentation described above, the library includes a small set of headers on outgoing requests that describe the SDK and the platform it is running on. Unlike the instrumentation above, which is opt-in, these are sent by default and can be turned off; see [Opting out](#opting-out). The two features are independent: `OpenAI.Experimental.EnableOpenTelemetry` does not govern these headers, and `OpenAI.DisableTelemetry` does not govern OpenTelemetry tracing or metrics.

### What is sent

| Header | Value | Source |
| --- | --- | --- |
| `X-Stainless-Lang` | `csharp` | Constant. |
| `X-Stainless-Package-Version` | The `OpenAI` package version, such as `2.12.0`. | The assembly informational version, with any `+<commit>` suffix removed. This is the version token from the `User-Agent`, sent verbatim; if it could not be transmitted unaltered it is reported as `unknown` rather than rewritten, so the two can never silently disagree. |
| `X-Stainless-Runtime` | `dotnet` | Constant. |
| `X-Stainless-Runtime-Version` | The running .NET version, such as `8.0.11`. | The version portion of `RuntimeInformation.FrameworkDescription`. Falls back to the full description when no version is present, and to `unknown` when it cannot be determined. |
| `X-Stainless-OS` | `Windows`, `MacOS`, `Linux`, `FreeBSD`, `Android`, `iOS`, `MacCatalyst`, or `Browser`. | `RuntimeInformation.IsOSPlatform`. Unrecognized platforms report `Other:<description>`. |
| `X-Stainless-Arch` | `x64`, `x86`, `arm64`, or `arm`. | `RuntimeInformation.ProcessArchitecture`. Other architectures report `other:<name>`. |

These headers restate, in a machine-parseable form, information already present in the `User-Agent` header, plus the process CPU architecture. Sending them individually means consumers do not have to parse the user agent string, which is not a stable contract.

Values are:

- Derived locally and deterministically, with no per-user or per-installation entropy. Two installations of the same package version on the same platform and runtime produce byte-for-byte identical values.
- Free of user names, machine names, tenant identifiers, file paths, and persistent identifiers.
- Printable ASCII with no line breaks. Free-form platform text (the operating system and runtime fallbacks) is additionally bounded in length. The package version is not bounded, so that it matches the `User-Agent` verbatim; the sole exception is the `unknown` fallback described above, used when the version could not be sent unaltered.

Any of these headers that you set yourself is preserved; the library only supplies values that are absent. Header names are matched case-insensitively.

### Opting out

Use either of the following, in order of precedence:

1. Set the `OpenAI.DisableTelemetry` context switch when your application starts, before creating any clients:

   ```csharp
   AppContext.SetSwitch("OpenAI.DisableTelemetry", true);
   ```

2. Set the `OPENAI_DISABLE_TELEMETRY` environment variable to `true` or `1`.

The setting is read when a client is created, so it must be applied before constructing any client.

Opting out suppresses:

- All six `X-Stainless-*` headers.
- The `User-Agent` header that the library adds. Neither `HttpClient` nor the underlying transport substitutes a default, so an opted-out request carries no user agent unless you supply one.

Opting out does not affect the `Authorization`, `OpenAI-Organization`, or `OpenAI-Project` headers, nor any header you set yourself.

### Realtime sessions

The WebSocket handshake that opens a Realtime session sends only the `Authorization` header along with any headers you supply through `RealtimeSessionClientOptions.Headers`. It does not send the `X-Stainless-*` headers, matching the behavior of the other official OpenAI SDKs. Regular HTTP requests made by `RealtimeClient` do include them.
