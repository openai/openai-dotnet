## Observability with OpenTelemetry

> Note:
> OpenAI .NET SDK instrumentation is in development and is not complete. See [Available sources and meters](#available-sources-and-meters) section for the list of covered operations.

OpenAI .NET library is instrumented with distributed tracing and metrics using .NET [tracing](https://learn.microsoft.com/dotnet/core/diagnostics/distributed-tracing)
and [metrics](https://learn.microsoft.com/dotnet/core/diagnostics/metrics-instrumentation) API and supports [OpenTelemetry](https://learn.microsoft.com/dotnet/core/diagnostics/observability-with-otel).

OpenAI .NET instrumentation follows [OpenTelemetry Semantic Conventions for Generative AI systems](https://github.com/open-telemetry/semantic-conventions/tree/main/docs/gen-ai).

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
[latest supported conventions](https://github.com/open-telemetry/semantic-conventions-genai/tree/main/docs/gen-ai).
Notable changes include:

- The `gen_ai.system` attribute is replaced by `gen_ai.provider.name`.

The default behavior (without the opt-in) remains unchanged and continues to emit v1.27.0 conventions.

### Available sources and meters

The following sources and meters are available:

- `OpenAI.ChatClient` - records traces and metrics for `ChatClient` operations (except streaming and protocol methods which are not instrumented yet)

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
