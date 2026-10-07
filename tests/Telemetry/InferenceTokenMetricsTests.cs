using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Responses;
using OpenAI.Telemetry;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests.Telemetry;

[TestFixture(false)]
[TestFixture(true)]
[NonParallelizable]
[Category("Telemetry")]
[Category("Smoke")]
public class InferenceTokenMetricsTests
{
    private const string Prefix = "gen_ai.client.inference.";
    private static readonly Uri s_endpoint = new("https://example.invalid");

    private static readonly string[] s_names =
    [
        Prefix + "usage.input_tokens",
        Prefix + "usage.output_tokens",
        Prefix + "usage.cache_read.input_tokens",
        Prefix + "usage.cache_write.input_tokens",
        Prefix + "usage.reasoning.output_tokens",
        Prefix + "operation.input_tokens",
        Prefix + "operation.output_tokens",
    ];

    private readonly bool _responses;

    public InferenceTokenMetricsTests(bool responses) => _responses = responses;

    private string MeterName => _responses ? "OpenAI.ResponsesClient" : "OpenAI.ChatClient";

    private string InputName => _responses ? "input_tokens" : "prompt_tokens";

    private string OutputName => _responses ? "output_tokens" : "completion_tokens";

    [Test]
    public void InstrumentContractsAndRefinementDimensions()
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var listener = new TestMeterListener(MeterName);
        var source = new OpenTelemetrySource("request-model", s_endpoint);
        using var scope = Start(source);
        Record(scope, Usage(12, 34, """{"cached_tokens":5,"cache_write_tokens":3}""", """{"reasoning_tokens":7}"""));

        foreach (var name in s_names)
        {
            var instrument = listener.GetInstrument(name);
            Assert.That(instrument, Is.Not.Null, name);
            Assert.That(instrument.Unit, Is.EqualTo("{token}"));

            if (name.Contains(".operation."))
            {
                Assert.That(instrument, Is.InstanceOf<Histogram<long>>());
                Assert.That(((Histogram<long>)instrument).Advice.HistogramBucketBoundaries,
                    Is.EqualTo(new long[] { 1, 4, 16, 64, 256, 1024, 4096, 16384, 65536, 262144, 1048576, 4194304, 16777216, 67108864 }));
            }
            else
            {
                Assert.That(instrument, Is.InstanceOf<Counter<long>>());
            }

            foreach (var measurement in listener.GetMeasurements(name) ?? [])
            {
                Assert.That(measurement.value, Is.InstanceOf<long>());
                Assert.That(measurement.tags.ContainsKey("gen_ai.token.modality"), Is.EqualTo(name.Contains(".usage.")));
                Assert.That(measurement.tags.ContainsKey("gen_ai.token.type"), Is.False);
                AssertRefinements(measurement);
            }
        }

        if (_responses)
        {
            AssertValue(listener, "usage.cache_write.input_tokens", 3);
        }
        else
        {
            Assert.That(listener.GetMeasurements(Prefix + "usage.cache_write.input_tokens"), Is.Null);
        }

        Assert.That(listener.GetMeasurements("gen_ai.client.token.usage"), Is.Null);
        AssertValue(listener, "usage.input_tokens", 12);
        AssertValue(listener, "usage.output_tokens", 34);
        AssertValue(listener, "operation.input_tokens", 12);
        AssertValue(listener, "operation.output_tokens", 34);
        AssertValue(listener, "usage.cache_read.input_tokens", 5);
        AssertValue(listener, "usage.reasoning.output_tokens", 7);
        var duration = listener.GetMeasurements("gen_ai.client.operation.duration").Single();
        AssertRefinements(duration);
        Assert.That(duration.tags.ContainsKey("gen_ai.token.modality"), Is.False);
    }

    [TestCaseSource(nameof(s_names))]
    public void ExclusiveInstrumentSubscriptionEnablesScopeWithoutTracing(string instrumentName)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var listener = new TestMeterListener(MeterName, instrumentName);
        var source = new OpenTelemetrySource("request-model", s_endpoint);
        using var scope = Start(source);
        Assert.That(scope, Is.Not.Null);
        Assert.That(Activity.Current, Is.Null);
        Record(scope, Usage(12, 34, """{"cached_tokens":5,"cache_write_tokens":3}""", """{"reasoning_tokens":7}"""));
        Assert.That(listener.GetMeasurements(instrumentName)?.Count ?? 0,
            Is.EqualTo((instrumentName.Contains("cache_write") && !_responses) ? 0 : 1));
        Assert.That(listener.GetMeasurements("gen_ai.client.operation.duration"), Is.Null);
    }

    [TestCase(null)]
    [TestCase("null")]
    [TestCase("{}")]
    public void UnavailableUsageDoesNotBecomeZero(string usage)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var listener = new TestMeterListener(MeterName);
        using var scope = Start(new OpenTelemetrySource("request-model", s_endpoint));
        Record(scope, usage);

        foreach (var name in s_names)
        {
            Assert.That(listener.GetMeasurements(name), Is.Null, name);
        }

        Assert.That(listener.GetMeasurements("gen_ai.client.operation.duration"), Has.Count.EqualTo(1));
    }

    [TestCase(null)]
    [TestCase("null")]
    [TestCase("{}")]
    public void UnavailableDetailsDoNotBecomeZero(string details)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var listener = new TestMeterListener(MeterName);
        using var scope = Start(new OpenTelemetrySource("request-model", s_endpoint));
        Record(scope, Usage(12, 34, details, details));
        AssertValue(listener, "usage.input_tokens", 12);
        AssertValue(listener, "usage.output_tokens", 34);
        Assert.That(listener.GetMeasurements(Prefix + "usage.cache_read.input_tokens"), Is.Null);
        Assert.That(listener.GetMeasurements(Prefix + "usage.cache_write.input_tokens"), Is.Null);
        Assert.That(listener.GetMeasurements(Prefix + "usage.reasoning.output_tokens"), Is.Null);
    }

    [Test]
    public void ReportedZerosAreMeasurements()
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var listener = new TestMeterListener(MeterName);
        using var scope = Start(new OpenTelemetrySource("request-model", s_endpoint));
        Record(scope, Usage(0, 0, """{"cached_tokens":0,"cache_write_tokens":0}""", """{"reasoning_tokens":0}"""));

        foreach (var name in s_names.Where(name => (_responses) || (!name.Contains("cache_write"))))
        {
            Assert.That(listener.GetMeasurements(name).Single().value, Is.EqualTo(0L), name);
        }
    }

    [Test]
    public void MissingDirectionIsNotInferredFromTotal()
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var listener = new TestMeterListener(MeterName);
        using var scope = Start(new OpenTelemetrySource("request-model", s_endpoint));
        Record(scope, $$"""{"{{InputName}}":12,"total_tokens":46}""");
        AssertValue(listener, "operation.input_tokens", 12);
        Assert.That(listener.GetMeasurements(Prefix + "operation.output_tokens"), Is.Null);
        Assert.That(listener.GetMeasurements(Prefix + "usage.output_tokens"), Is.Null);
    }

    [TestCase(0)]
    [TestCase(3)]
    [TestCase(12)]
    [TestCase(15)]
    public void ReportedAudioPartitionsTotalsWithoutGuessingOtherModalities(int audio)
    {
        if (_responses)
        {
            Assert.Ignore("Responses does not expose an audio token breakdown.");
        }

        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var listener = new TestMeterListener(MeterName);
        using var scope = Start(new OpenTelemetrySource("request-model", s_endpoint));
        Record(scope, Usage(12, 34,
            $$"""{"cached_tokens":5,"audio_tokens":{{audio}}}""",
            """{"reasoning_tokens":7,"audio_tokens":4}"""));
        var input = listener.GetMeasurements(Prefix + "usage.input_tokens");
        Assert.That(input.Sum(measurement => (long)measurement.value), Is.EqualTo(12L));
        Assert.That(input.All(measurement => !measurement.tags["gen_ai.token.modality"].Equals("text")), Is.True);

        if (audio <= 12)
        {
            Assert.That(input.Single(measurement => measurement.tags["gen_ai.token.modality"].Equals("audio")).value, Is.EqualTo((long)audio));
        }
        else
        {
            Assert.That(input.Single().tags["gen_ai.token.modality"], Is.EqualTo("unknown"));
        }

        var output = listener.GetMeasurements(Prefix + "usage.output_tokens");
        Assert.That(output.Sum(measurement => (long)measurement.value), Is.EqualTo(34L));
        Assert.That(output.Single(measurement => measurement.tags["gen_ai.token.modality"].Equals("audio")).value, Is.EqualTo(4L));
        Assert.That(output.Single(measurement => measurement.tags["gen_ai.token.modality"].Equals("unknown")).value, Is.EqualTo(30L));
        AssertValue(listener, "operation.input_tokens", 12);
        AssertValue(listener, "operation.output_tokens", 34);
        AssertValue(listener, "usage.cache_read.input_tokens", 5);
        AssertValue(listener, "usage.reasoning.output_tokens", 7);
    }

    [Test]
    public void MixedModeSourcesKeepIndependentMetricContracts()
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        OpenTelemetrySource latest;
        OpenTelemetrySource legacy;

        using (TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true))
        {
            latest = new OpenTelemetrySource("request-model", s_endpoint);
        }

        using (TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(false))
        {
            legacy = new OpenTelemetrySource("request-model", s_endpoint);
        }

        using var listener = new TestMeterListener(MeterName);

        foreach (var source in new[] { latest, legacy, latest, legacy })
        {
            using var scope = Start(source);
            Record(scope, Usage(12, 34));
        }

        var oldUsage = listener.GetMeasurements("gen_ai.client.token.usage");
        Assert.That(oldUsage, Has.Count.EqualTo(4));
        Assert.That(oldUsage.All(measurement => (measurement.tags.ContainsKey("gen_ai.system"))
            && (!measurement.tags.ContainsKey("gen_ai.provider.name"))
            && (!measurement.tags.ContainsKey("openai.response.service_tier"))
            && (!measurement.tags.ContainsKey("openai.response.system_fingerprint"))), Is.True);

        foreach (var name in s_names.Where(name => !name.Contains("cache_") && !name.Contains("reasoning")))
        {
            var measurements = listener.GetMeasurements(name);
            Assert.That(measurements, Has.Count.EqualTo(2));
            Assert.That(measurements.All(measurement => (measurement.tags.ContainsKey("gen_ai.provider.name"))
                && (!measurement.tags.ContainsKey("gen_ai.system"))), Is.True);
        }

        var durations = listener.GetMeasurements("gen_ai.client.operation.duration");
        Assert.That(durations, Has.Count.EqualTo(4));
        Assert.That(durations.Count(measurement => measurement.tags.ContainsKey("openai.response.system_fingerprint")), Is.EqualTo(2));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void OppositeConventionInstrumentDoesNotEnableScope(bool latest)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(latest);
        using var listener = new TestMeterListener(MeterName, latest ? "gen_ai.client.token.usage" : s_names[0]);
        Assert.That(Start(new OpenTelemetrySource("request-model", s_endpoint)), Is.Null);
    }

    [Test]
    public void SharedRecorderUsesLongValuesAndKeepsSubsetsSeparate()
    {
        using var meter = new Meter($"OpenAI.Tests.InferenceTokens.{_responses}");
        using var listener = new TestMeterListener(meter.Name);
        var recorder = new OpenTelemetryTokenMetrics(meter);
        var total = (long)int.MaxValue + 10;
        recorder.Record(new OpenTelemetryTokenUsage
        {
            InputTokens = total,
            OutputTokens = total,
            CacheReadInputTokens = 5,
            CacheWriteInputTokens = 3,
            ReasoningOutputTokens = 7,
        }, default);
        AssertValue(listener, "usage.input_tokens", total);
        AssertValue(listener, "usage.output_tokens", total);
        AssertValue(listener, "operation.input_tokens", total);
        AssertValue(listener, "operation.output_tokens", total);
        AssertValue(listener, "usage.cache_read.input_tokens", 5);
        AssertValue(listener, "usage.cache_write.input_tokens", 3);
        AssertValue(listener, "usage.reasoning.output_tokens", 7);
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task BufferedClientEmitsOneOperation(bool useAsync)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var listener = new TestMeterListener(MeterName);
        var body = ResponseBody(Usage(12, 34, """{"cached_tokens":5}""", """{"reasoning_tokens":7}"""));
        var transport = new MockPipelineTransport(_ => new MockPipelineResponse(200).WithContent(body))
        {
            ExpectSyncPipeline = !useAsync,
        };

        if (_responses)
        {
            var client = new ResponsesClient(new ApiKeyCredential("not-a-real-key"), new ResponsesClientOptions
            {
                Endpoint = s_endpoint,
                Transport = transport,
            });
            var options = new CreateResponseOptions("request-model", [ResponseItem.CreateUserMessageItem("input")]);

            if (useAsync)
            {
                await client.CreateResponseAsync(options);
            }
            else
            {
                client.CreateResponse(options);
            }
        }
        else
        {
            var client = new ChatClient("request-model", new ApiKeyCredential("not-a-real-key"), new OpenAIClientOptions
            {
                Endpoint = s_endpoint,
                Transport = transport,
            });

            if (useAsync)
            {
                await client.CompleteChatAsync([new UserChatMessage("input")]);
            }
            else
            {
                client.CompleteChat([new UserChatMessage("input")]);
            }
        }

        Assert.That(Activity.Current, Is.Null);
        Assert.That(listener.GetMeasurements("gen_ai.client.operation.duration"), Has.Count.EqualTo(1));
        Assert.That(listener.GetMeasurements("gen_ai.client.token.usage"), Is.Null);
        AssertValue(listener, "operation.input_tokens", 12);
        AssertValue(listener, "operation.output_tokens", 34);
        AssertValue(listener, "usage.input_tokens", 12);
        AssertValue(listener, "usage.output_tokens", 34);
    }

    private OpenTelemetryScope Start(OpenTelemetrySource source)
        => _responses
            ? source.StartResponsesScope(new CreateResponseOptions("request-model", []))
            : source.StartChatScope(new ChatCompletionOptions());

    private void Record(OpenTelemetryScope scope, string usage)
    {
        var data = BinaryData.FromString(ResponseBody(usage));

        if (_responses)
        {
            scope.RecordResponseResult(ModelReaderWriter.Read<ResponseResult>(data));
        }
        else
        {
            scope.RecordChatCompletion(ModelReaderWriter.Read<ChatCompletion>(data));
        }
    }

    private string ResponseBody(string usage)
    {
        var usageProperty = usage is null ? "" : $",\"usage\":{usage}";

        return _responses
            ? $$"""{"id":"resp_sensitive","object":"response","created_at":1,"status":"completed","model":"response-model","output":[],"parallel_tool_calls":false,"tools":[],"service_tier":"default","system_fingerprint":"fp_test"{{usageProperty}}}"""
            : $$$"""{"id":"chatcmpl_sensitive","created":1,"model":"response-model","choices":[{"index":0,"finish_reason":"stop","message":{"role":"assistant","content":"sensitive"}}],"service_tier":"default","system_fingerprint":"fp_test"{{{usageProperty}}}}""";
    }

    private string Usage(int input, int output, string inputDetails = null, string outputDetails = null)
    {
        var inputDetailProperty = inputDetails is null ? "" : $",\"{InputName}_details\":{inputDetails}";
        var outputDetailProperty = outputDetails is null ? "" : $",\"{OutputName}_details\":{outputDetails}";

        return $$"""{"{{InputName}}":{{input}},"{{OutputName}}":{{output}},"total_tokens":{{input + output}}{{inputDetailProperty}}{{outputDetailProperty}}}""";
    }

    private static void AssertValue(TestMeterListener listener, string suffix, long value)
    {
        var measurement = listener.GetMeasurements(Prefix + suffix).Single();
        Assert.That(measurement.value, Is.EqualTo(value), suffix);

        if (suffix.StartsWith("usage."))
        {
            Assert.That(measurement.tags["gen_ai.token.modality"], Is.EqualTo("unknown"));
        }
    }

    private static void AssertRefinements(TestMeterListener.TestMeasurement measurement)
    {
        Assert.That(measurement.tags["gen_ai.response.model"], Is.EqualTo("response-model"));
        Assert.That(measurement.tags["gen_ai.request.model"], Is.EqualTo("request-model"));
        Assert.That(measurement.tags["gen_ai.provider.name"], Is.EqualTo("openai"));
        Assert.That(measurement.tags["openai.response.service_tier"], Is.EqualTo("default"));
        Assert.That(measurement.tags["openai.response.system_fingerprint"], Is.EqualTo("fp_test"));
        Assert.That(measurement.tags.Keys, Is.EquivalentTo(
            new[] { "gen_ai.operation.name", "gen_ai.request.model", "gen_ai.response.model", "gen_ai.provider.name",
                "server.address", "server.port", "openai.response.service_tier", "openai.response.system_fingerprint" }
            .Concat(measurement.tags.ContainsKey("gen_ai.token.modality") ? ["gen_ai.token.modality"] : Array.Empty<string>())));
    }
}
