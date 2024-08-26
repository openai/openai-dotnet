using NUnit.Framework;
using OpenAI.Chat;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class ChatExamples
{
    [Test]
    public async Task OpenTelemetryExamples()
    {
        AppContext.SetSwitch("OpenAI.Experimental.EnableOpenTelemetry", true);

        // Let's configure OpenTelemetry to collect OpenAI and HTTP client traces and metrics
        // and export them to console and also to the local OTLP endpoint.
        //
        // If you have some local OTLP listener (e.g. Aspire dashboard) running,
        // you can explore traces and metrics produced by the test there.
        //
        // Check out https://opentelemetry.io/docs/languages/net/getting-started/ for more details and
        // examples on how to set up OpenTelemetry with ASP.NET Core.

        ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault().AddService("test");
        using TracerProvider tracerProvider = OpenTelemetry.Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(resourceBuilder)
            .AddSource("OpenAI*")
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter()
            .Build();

        using MeterProvider meterProvider = OpenTelemetry.Sdk.CreateMeterProviderBuilder()
            .SetResourceBuilder(resourceBuilder)
            .AddView("gen_ai.client.operation.duration", new Base2ExponentialBucketHistogramConfiguration { MaxSize = 10 })
            .AddMeter("OpenAI*")
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter()
            .Build();

        ChatClient client = new("gpt-4o-mini", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        ChatCompletion completion = await client.CompleteChatAsync("Say 'this is a test.'");

        Console.WriteLine($"{completion}");
    }
}
