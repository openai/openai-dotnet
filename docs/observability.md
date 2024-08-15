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

### Available sources and meters

The following sources and meters are available:

- `OpenAI.ChatClient` - records traces and metrics for `ChatClient` operations (except streaming and protocol methods which are not instrumented yet)
