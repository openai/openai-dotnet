using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Responses;
using OpenAI.Telemetry;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Telemetry;

[TestFixture]
[NonParallelizable]
[Category("Telemetry")]
[Category("Smoke")]
public class LatestSpanTelemetryTests
{
    private const string ResponsesSource = "OpenAI.ResponsesClient";
    private const string ChatSource = "OpenAI.ChatClient";
    private static readonly Uri s_endpoint = new("https://example.invalid");

    private static IEnumerable<TestCaseData> BufferedCases()
    {
        foreach (var responses in new[] { false, true })
        {
            foreach (var latest in new[] { false, true })
            {
                foreach (var useAsync in new[] { false, true })
                {
                    foreach (var usage in new[] { "absent", "empty", "zero", "partial", "full", "empty_details" })
                    {
                        yield return new TestCaseData(responses, latest, useAsync, usage);
                    }
                }
            }
        }
    }

    [TestCaseSource(nameof(BufferedCases))]
    public async Task BufferedUsageAndMetadataShareLatestSpanAndMetricValues(bool responses, bool latest, bool useAsync, string usageKind)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(latest);
        using var activities = new TestActivityListener(responses ? ResponsesSource : ChatSource);
        using var metrics = new TestMeterListener(responses ? ResponsesSource : ChatSource);
        var body = ResponseBody(responses, Usage(responses, usageKind));
        var transport = new MockPipelineTransport(_ => new MockPipelineResponse(200).WithContent(body))
        {
            ExpectSyncPipeline = !useAsync,
        };
        using var parent = new Activity("buffered-caller").Start();

        if (responses)
        {
            var client = new ResponsesClient(new ApiKeyCredential("not-a-real-key"), new ResponsesClientOptions
            {
                Endpoint = s_endpoint,
                Transport = transport,
            });

            if (useAsync)
            {
                await client.CreateResponseAsync(Options());
            }
            else
            {
                client.CreateResponse(Options());
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
                await client.CompleteChatAsync([new UserChatMessage("sensitive input")]);
            }
            else
            {
                client.CompleteChat([new UserChatMessage("sensitive input")]);
            }
        }

        Assert.That(Activity.Current, Is.SameAs(parent));
        var activity = activities.Activities.Single();
        Assert.That(activity.ParentId, Is.EqualTo(parent.Id));
        Assert.That(activity.GetTagItem("gen_ai.request.stream"), Is.Null);
        Assert.That(activity.GetTagItem("openai.api.type"), Is.EqualTo(latest ? (responses ? "responses" : "chat_completions") : null));
        Assert.That(activity.GetTagItem("openai.response.service_tier"), Is.EqualTo(latest ? "default" : null));
        Assert.That(activity.GetTagItem("openai.response.system_fingerprint"), Is.EqualTo(latest ? "fp_test" : null));

        var input = usageKind switch
        {
            "absent" => (long?)null,
            "empty" => latest ? null : 0,
            "zero" => 0,
            _ => 13,
        };
        var output = usageKind switch
        {
            "absent" => (long?)null,
            "empty" or "partial" => latest ? null : 0,
            "zero" => 0,
            _ => 23,
        };
        Assert.That(activity.GetTagItem("gen_ai.usage.input_tokens"), Is.EqualTo(input));
        Assert.That(activity.GetTagItem("gen_ai.usage.output_tokens"), Is.EqualTo(output));

        foreach (var direction in new[] { "input", "output" })
        {
            var expected = direction == "input" ? input : output;

            if (latest)
            {
                var histogram = metrics.GetMeasurements($"gen_ai.client.inference.operation.{direction}_tokens");
                Assert.That(histogram?.Single().value, Is.EqualTo(expected));
                var counter = metrics.GetMeasurements($"gen_ai.client.inference.usage.{direction}_tokens");
                Assert.That(counter?.Sum(measurement => (long)measurement.value), Is.EqualTo(expected));
            }
            else
            {
                var measurement = metrics.GetMeasurements("gen_ai.client.token.usage")
                    ?.Single(item => item.tags["gen_ai.token.type"].Equals(direction));
                Assert.That(measurement?.value, Is.EqualTo(expected));
            }
        }

        var hasDetails = (latest) && ((usageKind == "full") || (usageKind == "zero"));
        var cache = hasDetails ? (usageKind == "zero" ? 0L : 4L) : (long?)null;
        var reasoning = hasDetails ? (usageKind == "zero" ? 0L : 6L) : (long?)null;
        var written = ((hasDetails) && (responses)) ? (usageKind == "zero" ? 0L : 2L) : (long?)null;
        Assert.That(activity.GetTagItem("gen_ai.usage.cache_read.input_tokens"), Is.EqualTo(cache));
        Assert.That(activity.GetTagItem("gen_ai.usage.cache_write.input_tokens"), Is.EqualTo(written));
        Assert.That(activity.GetTagItem("gen_ai.usage.reasoning.output_tokens"), Is.EqualTo(reasoning));
        Assert.That(activity.GetTagItem("gen_ai.usage.audio.input_tokens"),
            Is.EqualTo(((hasDetails) && (!responses)) ? (usageKind == "zero" ? 0L : 3L) : null));
        Assert.That(activity.GetTagItem("gen_ai.usage.audio.output_tokens"),
            Is.EqualTo(((hasDetails) && (!responses)) ? (usageKind == "zero" ? 0L : 5L) : null));
        Assert.That(activity.GetTagItem("gen_ai.usage.text.input_tokens"), Is.Null);
        Assert.That(activity.GetTagItem("gen_ai.usage.audio.cache_read.input_tokens"), Is.Null);

        if (latest)
        {
            Assert.That(metrics.GetMeasurements("gen_ai.client.inference.usage.cache_read.input_tokens")?.Single().value, Is.EqualTo(cache));
            Assert.That(metrics.GetMeasurements("gen_ai.client.inference.usage.cache_write.input_tokens")?.Single().value, Is.EqualTo(written));
            Assert.That(metrics.GetMeasurements("gen_ai.client.inference.usage.reasoning.output_tokens")?.Single().value, Is.EqualTo(reasoning));
        }

        AssertNoContent(activity);
    }

    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public void ChatExplicitRequestAttributesAreLatestOnly(bool latest, bool explicitValues)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(latest);
        using var activities = new TestActivityListener(ChatSource);
        var options = new ChatCompletionOptions();

        if (explicitValues)
        {
            options.Seed = 0;
            options.FrequencyPenalty = 0.25f;
            options.PresencePenalty = 0;
            options.ReasoningEffortLevel = ChatReasoningEffortLevel.High;
            options.ServiceTier = ChatServiceTier.Default;
        }

        using (new OpenTelemetrySource("request-model", s_endpoint).StartChatScope(options))
        {
        }

        var activity = activities.Activities.Single();
        var populated = (latest) && (explicitValues);
        Assert.That(activity.GetTagItem("gen_ai.request.seed"), Is.EqualTo(populated ? 0L : null));
        Assert.That(activity.GetTagItem("gen_ai.request.frequency_penalty"), Is.EqualTo(populated ? 0.25 : null));
        Assert.That(activity.GetTagItem("gen_ai.request.presence_penalty"), Is.EqualTo(populated ? 0.0 : null));
        Assert.That(activity.GetTagItem("gen_ai.request.reasoning.level"), Is.EqualTo(populated ? "high" : null));
        Assert.That(activity.GetTagItem("openai.request.service_tier"), Is.EqualTo(populated ? "default" : null));
        Assert.That(activity.GetTagItem("gen_ai.output.type"), Is.Null);
        Assert.That(activity.GetTagItem("gen_ai.request.stream"), Is.Null);
    }

    [Test]
    public void InconsistentAudioBreakdownIsOmittedFromBothSignals()
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var activities = new TestActivityListener(ChatSource);
        using var metrics = new TestMeterListener(ChatSource);
        var completion = ModelReaderWriter.Read<ChatCompletion>(BinaryData.FromString(ResponseBody(false,
            """{"prompt_tokens":2,"completion_tokens":3,"prompt_tokens_details":{"audio_tokens":4},"completion_tokens_details":{"audio_tokens":5}}""")));

        using (var scope = new OpenTelemetrySource("request-model", s_endpoint).StartChatScope(new ChatCompletionOptions()))
        {
            scope.RecordChatCompletion(completion);
        }

        var activity = activities.Activities.Single();
        Assert.That(activity.GetTagItem("gen_ai.usage.audio.input_tokens"), Is.Null);
        Assert.That(activity.GetTagItem("gen_ai.usage.audio.output_tokens"), Is.Null);
        Assert.That(activity.GetTagItem("gen_ai.usage.input_tokens"), Is.EqualTo(2L));
        Assert.That(activity.GetTagItem("gen_ai.usage.output_tokens"), Is.EqualTo(3L));
        Assert.That(metrics.GetMeasurements("gen_ai.client.inference.usage.input_tokens").Single().tags["gen_ai.token.modality"], Is.EqualTo("unknown"));
        Assert.That(metrics.GetMeasurements("gen_ai.client.inference.usage.output_tokens").Single().tags["gen_ai.token.modality"], Is.EqualTo("unknown"));
    }

    [TestCase(false, "absent", null)]
    [TestCase(true, "absent", null)]
    [TestCase(false, "text", null)]
    [TestCase(true, "text", "text")]
    [TestCase(false, "json_object", null)]
    [TestCase(true, "json_object", "json")]
    [TestCase(false, "json_schema", null)]
    [TestCase(true, "json_schema", "json")]
    [TestCase(true, "future", null)]
    public void ChatOutputTypeUsesExplicitFormatWithoutRecordingSchema(bool latest, string format, string expected)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(latest);
        using var activities = new TestActivityListener(ChatSource);
        var options = new ChatCompletionOptions
        {
            ServiceTier = ChatServiceTier.Auto,
            ResponseFormat = format switch
            {
                "text" => ChatResponseFormat.CreateTextFormat(),
                "json_object" => ChatResponseFormat.CreateJsonObjectFormat(),
                "json_schema" => ChatResponseFormat.CreateJsonSchemaFormat("sensitive-schema", BinaryData.FromString("""{"type":"object"}""")),
                "future" => ModelReaderWriter.Read<ChatResponseFormat>(BinaryData.FromString("""{"type":"future"}""")),
                _ => null,
            },
        };

        using (new OpenTelemetrySource("request-model", s_endpoint).StartChatScope(options))
        {
        }

        var activity = activities.Activities.Single();
        Assert.That(activity.GetTagItem("gen_ai.output.type"), Is.EqualTo(expected));
        Assert.That(activity.GetTagItem("openai.request.service_tier"), Is.Null);
        Assert.That(activity.TagObjects.Select(tag => tag.Value?.ToString()), Does.Not.Contain("sensitive-schema"));
    }

    private static IEnumerable<TestCaseData> MetadataCases()
    {
        foreach (var useAsync in new[] { false, true })
        {
            foreach (var latest in new[] { false, true })
            {
                foreach (var first in new[] { "created", "in_progress", "queued" })
                {
                    yield return new TestCaseData(useAsync, latest, first);
                }
            }
        }
    }

    [TestCaseSource(nameof(MetadataCases))]
    public async Task StreamingMetadataIsRetainedAndNormalTerminalFallsBack(bool useAsync, bool latest, string first)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(latest);
        using var activities = new TestActivityListener(ResponsesSource);
        using var metrics = new TestMeterListener(ResponsesSource);
        using var parent = new Activity("stream-caller").Start();
        Activity captured = null;
        var initial = ResponseBody(true, Usage(true, "full"));
        var replacement = """{"created_at":1,"id":"later-id","model":"later-model","output":[],"parallel_tool_calls":false}""";
        var terminal = ResponseBody(true, Usage(true, "zero"), metadata: false);
        var content = Event("response." + first, initial)
            + Event("response.in_progress", replacement)
            + Event("response.completed", terminal);
        var client = StreamingClient(content, useAsync, () => captured = Activity.Current);
        var count = 0;
        void Observe()
        {
            count++;
            Assert.That(Activity.Current, Is.SameAs(parent));

            if (count < 3)
            {
                Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration"), Is.Null);
                Assert.That(metrics.GetMeasurements("gen_ai.client.inference.usage.input_tokens"), Is.Null);
                Assert.That(captured.GetTagItem("gen_ai.usage.input_tokens"), Is.Null);
                Assert.That(captured.GetTagItem("gen_ai.response.id"), Is.EqualTo(latest ? (count == 1 ? "response-id" : "later-id") : null));
            }
        }

        if (useAsync)
        {
            await foreach (var update in client.CreateResponseStreamingAsync(Options(streaming: true)))
            {
                Observe();
            }
        }
        else
        {
            foreach (var update in client.CreateResponseStreaming(Options(streaming: true)))
            {
                Observe();
            }
        }

        Assert.That(count, Is.EqualTo(3));
        var activity = activities.Activities.Single();
        Assert.That(activity.GetTagItem("gen_ai.response.id"), Is.EqualTo(latest ? "later-id" : null));
        Assert.That(activity.GetTagItem("gen_ai.response.model"), Is.EqualTo(latest ? "later-model" : null));
        Assert.That(activity.GetTagItem("openai.response.service_tier"), Is.EqualTo(latest ? "default" : null));
        Assert.That(activity.GetTagItem("openai.response.system_fingerprint"), Is.EqualTo(latest ? "fp_test" : null));
        Assert.That(activity.GetTagItem("gen_ai.response.finish_reasons"), Is.EqualTo(new[] { "stop" }));
        Assert.That(activity.GetTagItem("gen_ai.usage.input_tokens"), Is.EqualTo(0));
        Assert.That(activity.GetTagItem("gen_ai.request.stream"), Is.EqualTo(latest ? true : null));
        var duration = metrics.GetMeasurements("gen_ai.client.operation.duration").Single();
        Assert.That(duration.tags.GetValueOrDefault("gen_ai.response.model"), Is.EqualTo(latest ? "later-model" : null));
        Assert.That(duration.tags.GetValueOrDefault("openai.response.system_fingerprint"), Is.EqualTo(latest ? "fp_test" : null));

        if (latest)
        {
            foreach (var name in new[] { "usage.input_tokens", "usage.output_tokens", "operation.input_tokens", "operation.output_tokens" })
            {
                var measurement = metrics.GetMeasurements("gen_ai.client.inference." + name).Single();
                Assert.That(measurement.tags["gen_ai.response.model"], Is.EqualTo("later-model"));
                Assert.That(measurement.tags["openai.response.service_tier"], Is.EqualTo("default"));
                Assert.That(measurement.tags["openai.response.system_fingerprint"], Is.EqualTo("fp_test"));
                Assert.That(measurement.tags.ContainsKey("gen_ai.response.id"), Is.False);
            }

            var firstChunk = metrics.GetMeasurements("gen_ai.client.operation.time_to_first_chunk").Single();
            Assert.That(firstChunk.tags["gen_ai.response.model"], Is.EqualTo("response-model"));
        }

        Assert.That(Activity.Current, Is.SameAs(parent));
        AssertNoContent(activity);
    }

    private static IEnumerable<TestCaseData> FinishCases()
    {
        foreach (var latest in new[] { false, true })
        {
            foreach (var state in new[] { "before_send", "typed_empty", "created", "queued", "output", "raw" })
            {
                foreach (var ending in new[] { "eof", "dispose", "exception" })
                {
                    yield return new TestCaseData(latest, state, ending);
                }
            }
        }
    }

    [TestCaseSource(nameof(FinishCases))]
    public void StreamingFinishReasonStateTableIsLatestOnly(bool latest, string state, string ending)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(latest);
        using var activities = new TestActivityListener(ResponsesSource);
        using var metrics = new TestMeterListener(ResponsesSource);
        using var parent = new Activity("state-caller").Start();
        var source = new OpenTelemetrySource(s_endpoint);
        var lifecycle = source.StartResponsesStreamingScope(Options(streaming: true));

        if (state == "raw")
        {
            lifecycle.Complete(SseCompletionKind.RawResponse);
        }
        else if (state != "before_send")
        {
            lifecycle.OnTypedResponse();

            if ((state == "created") || (state == "queued"))
            {
                lifecycle.OnEvent();
                lifecycle.OnUpdate(Update("response." + state, ResponseBody(true, Usage(true, "full"))));
            }
            else if (state == "output")
            {
                lifecycle.OnEvent();
                lifecycle.OnUpdate(ModelReaderWriter.Read<StreamingResponseUpdate>(BinaryData.FromString(
                    """{"type":"response.output_text.delta","sequence_number":1,"output_index":0,"delta":"sensitive output"}""")));
            }
        }

        if (ending == "exception")
        {
            lifecycle.OnException(new IOException("sensitive failure"));
        }
        else
        {
            lifecycle.Complete(ending == "eof" ? SseCompletionKind.EndOfStream : SseCompletionKind.Disposed);
        }

        lifecycle.Complete(SseCompletionKind.Disposed);
        lifecycle.OnException(new IOException("must not overwrite outcome"));

        Assert.That(Activity.Current, Is.SameAs(parent));
        var activity = activities.Activities.Single();
        var expected = (latest) && (state != "before_send") && (state != "raw");
        Assert.That(activity.GetTagItem("gen_ai.response.finish_reasons"), Is.EqualTo(expected ? new[] { "error" } : null));
        Assert.That(activity.Status, Is.EqualTo(state == "raw" ? ActivityStatusCode.Unset : ActivityStatusCode.Error));
        Assert.That(activity.GetTagItem("gen_ai.usage.input_tokens"), Is.Null);
        Assert.That(metrics.GetMeasurements("gen_ai.client.inference.usage.input_tokens"), Is.Null);
        Assert.That(metrics.GetMeasurements("gen_ai.client.token.usage"), Is.Null);
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration")?.Count ?? 0, Is.EqualTo(state == "raw" ? 0 : 1));

        if ((latest) && ((state == "created") || (state == "queued")))
        {
            Assert.That(activity.GetTagItem("gen_ai.response.id"), Is.EqualTo("response-id"));
            Assert.That(activity.GetTagItem("openai.response.system_fingerprint"), Is.EqualTo("fp_test"));
            var duration = metrics.GetMeasurements("gen_ai.client.operation.duration").Single();
            Assert.That(duration.tags["gen_ai.response.model"], Is.EqualTo("response-model"));
            Assert.That(duration.tags["openai.response.system_fingerprint"], Is.EqualTo("fp_test"));
        }

        AssertNoContent(activity);
    }

    [TestCase(false, "empty")]
    [TestCase(true, "empty")]
    [TestCase(false, "read_failure")]
    [TestCase(true, "read_failure")]
    [TestCase(false, "send_failure")]
    [TestCase(true, "send_failure")]
    public async Task ZeroEventTypedEndingsAreDistinguishedFromSendFailures(bool useAsync, string ending)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var activities = new TestActivityListener(ResponsesSource);
        using var metrics = new TestMeterListener(ResponsesSource);
        using var parent = new Activity("empty-caller").Start();
        var client = StreamingClient("", useAsync,
            onSend: () =>
            {
                if (ending == "send_failure")
                {
                    throw new IOException("sensitive send failure");
                }
            },
            streamFactory: ending == "read_failure" ? () => new ImmediateReadFailureStream() : null);
        async Task Consume()
        {
            if (useAsync)
            {
                await foreach (var update in client.CreateResponseStreamingAsync(Options(streaming: true)))
                {
                }
            }
            else
            {
                foreach (var update in client.CreateResponseStreaming(Options(streaming: true)))
                {
                }
            }
        }

        if (ending == "empty")
        {
            await Consume();
        }
        else
        {
            Assert.ThrowsAsync<IOException>(Consume);
        }

        var activity = activities.Activities.Single();
        Assert.That(activity.GetTagItem("gen_ai.response.finish_reasons"), Is.EqualTo(ending == "send_failure" ? null : new[] { "error" }));
        Assert.That(activity.GetTagItem("gen_ai.response.time_to_first_chunk"), Is.Null);
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.time_to_first_chunk"), Is.Null);
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration"), Has.Count.EqualTo(1));
        Assert.That(Activity.Current, Is.SameAs(parent));
    }

    [TestCase(true, false, false)]
    [TestCase(false, true, false)]
    [TestCase(true, true, false)]
    [TestCase(true, false, true)]
    [TestCase(false, true, true)]
    [TestCase(true, true, true)]
    public void FirstChunkSpanAndHistogramShareOneScalarIndependently(bool trace, bool measure, bool noEvents)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var activities = trace ? new TestActivityListener(ResponsesSource) : null;
        using var metrics = measure ? new TestMeterListener(ResponsesSource) : null;
        using var scope = OpenTelemetryScope.StartResponses("request-model", "chat", s_endpoint.Host, 443,
            Options(), "gen_ai.provider.name", true, streaming: true);
        var now = 1.25;
        var lifecycle = scope.CreateStreamingLifecycle(() => now);
        lifecycle.OnTypedResponse();

        if (!noEvents)
        {
            lifecycle.OnEvent();
            now = 9.5;
            lifecycle.OnUpdate(Update("response.created", ResponseBody(true, null)));
        }

        lifecycle.Complete(SseCompletionKind.EndOfStream);

        if (trace)
        {
            Assert.That(activities.Activities.Single().GetTagItem("gen_ai.response.time_to_first_chunk"), Is.EqualTo(noEvents ? null : 1.25));
        }

        if (measure)
        {
            var measurements = metrics.GetMeasurements("gen_ai.client.operation.time_to_first_chunk");
            Assert.That(measurements?.Single().value, Is.EqualTo(noEvents ? null : 1.25));

            if ((trace) && (!noEvents))
            {
                Assert.That(measurements.Single().value, Is.EqualTo(activities.Activities.Single().GetTagItem("gen_ai.response.time_to_first_chunk")));
            }
        }
    }

    private static CreateResponseOptions Options(bool streaming = false)
        => new("request-model", [ResponseItem.CreateUserMessageItem("sensitive input")])
        {
            StreamingEnabled = streaming,
        };

    private static ResponsesClient StreamingClient(string content, bool useAsync, Action onSend = null, Func<Stream> streamFactory = null)
    {
        var transport = new MockPipelineTransport(_ =>
        {
            onSend?.Invoke();

            return new MockPipelineResponse(200) { ContentStream = streamFactory?.Invoke() ?? new MemoryStream(Encoding.UTF8.GetBytes(content)) };
        })
        {
            ExpectSyncPipeline = !useAsync,
        };

        return new ResponsesClient(new ApiKeyCredential("not-a-real-key"), new ResponsesClientOptions
        {
            Endpoint = s_endpoint,
            Transport = transport,
            RetryPolicy = new ClientRetryPolicy(0),
        });
    }

    private static string ResponseBody(bool responses, string usage, bool metadata = true)
    {
        var body = new Dictionary<string, object>();

        if (responses)
        {
            body["created_at"] = 1;
            body["status"] = "completed";
            body["output"] = Array.Empty<object>();
            body["parallel_tool_calls"] = false;
        }
        else
        {
            body["created"] = 1;
            body["choices"] = new[] { new { index = 0, finish_reason = "stop", message = new { role = "assistant", content = "sensitive output" } } };
        }

        if (metadata)
        {
            body["id"] = "response-id";
            body["model"] = "response-model";
            body["service_tier"] = "default";
            body["system_fingerprint"] = "fp_test";
        }

        if (usage is not null)
        {
            body["usage"] = JsonSerializer.Deserialize<JsonElement>(usage);
        }

        return JsonSerializer.Serialize(body);
    }

    private static string Usage(bool responses, string kind)
    {
        if (kind == "absent")
        {
            return null;
        }

        if (kind == "empty")
        {
            return "{}";
        }

        var input = responses ? "input_tokens" : "prompt_tokens";
        var output = responses ? "output_tokens" : "completion_tokens";
        var usage = new Dictionary<string, object>
        {
            [input] = kind == "zero" ? 0 : 13,
        };

        if (kind != "partial")
        {
            usage[output] = kind == "zero" ? 0 : 23;
        }

        var inputDetails = new Dictionary<string, object>();
        var outputDetails = new Dictionary<string, object>();

        if ((kind == "full") || (kind == "zero"))
        {
            inputDetails["cached_tokens"] = kind == "zero" ? 0 : 4;
            outputDetails["reasoning_tokens"] = kind == "zero" ? 0 : 6;

            if (responses)
            {
                inputDetails["cache_write_tokens"] = kind == "zero" ? 0 : 2;
            }
            else
            {
                inputDetails["audio_tokens"] = kind == "zero" ? 0 : 3;
                outputDetails["audio_tokens"] = kind == "zero" ? 0 : 5;
            }
        }
        usage[input + "_details"] = inputDetails;
        usage[output + "_details"] = outputDetails;

        return JsonSerializer.Serialize(usage);
    }

    private static StreamingResponseUpdate Update(string kind, string body)
        => ModelReaderWriter.Read<StreamingResponseUpdate>(BinaryData.FromString(UpdateBody(kind, body)));

    private static string Event(string kind, string body) => "data: " + UpdateBody(kind, body) + "\n\n";

    private static string UpdateBody(string kind, string body)
        => JsonSerializer.Serialize(new { type = kind, sequence_number = 1, response = JsonSerializer.Deserialize<JsonElement>(body) });

    private static void AssertNoContent(Activity activity)
    {
        Assert.That(activity.TagObjects.Select(tag => tag.Value?.ToString()), Does.Not.Contain("sensitive input"));
        Assert.That(activity.TagObjects.Select(tag => tag.Value?.ToString()), Does.Not.Contain("sensitive output"));
        Assert.That(activity.StatusDescription, Is.Null);
        Assert.That(activity.Events, Is.Empty);
    }

    private sealed class ImmediateReadFailureStream : MemoryStream
    {
        public override int Read(byte[] buffer, int offset, int count) => throw new IOException("sensitive read failure");

        public override int Read(Span<byte> buffer) => throw new IOException("sensitive read failure");

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => Task.FromException<int>(new IOException("sensitive read failure"));

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            => ValueTask.FromException<int>(new IOException("sensitive read failure"));
    }
}
