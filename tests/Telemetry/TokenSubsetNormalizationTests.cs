using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Responses;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Tests.Telemetry;

[NonParallelizable]
[Category("Telemetry")]
[Category("Smoke")]
public class TokenSubsetNormalizationTests
{
    private const string Prefix = "gen_ai.client.inference.";

    private static readonly Uri s_endpoint = new("https://example.invalid");

    private static IEnumerable<TestCaseData> Cases()
    {
        foreach (var api in new[] { "chat", "response", "stream" })
        {
            foreach (var useAsync in new[] { false, true })
            {
                foreach (var latest in new[] { false, true })
                {
                    foreach (var scenario in new[] { "exceeds", "equal", "zero", "unknown", "input_unknown", "output_unknown", "independent", "below" })
                    {
                        yield return new TestCaseData(api, useAsync, latest, scenario);
                    }
                }
            }
        }
    }

    [TestCaseSource(nameof(Cases))]
    public async Task SubsetsAreIndividuallyBoundedByKnownTotalsInLatestSignals(string api, bool useAsync, bool latest, string scenario)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(latest);
        using var activities = new TestActivityListener(api == "chat" ? "OpenAI.ChatClient" : "OpenAI.ResponsesClient");
        using var metrics = new TestMeterListener(api == "chat" ? "OpenAI.ChatClient" : "OpenAI.ResponsesClient");
        using var parent = new Activity("subset-caller").Start();
        var values = GetCase(scenario);
        var body = ResponseBody(api == "chat", values);
        var transport = new MockPipelineTransport(_ =>
        {
            if (api == "stream")
            {
                var content = "data: {\"type\":\"response.completed\",\"sequence_number\":1,\"response\":" + body + "}\n\n";

                return new MockPipelineResponse(200) { ContentStream = new MemoryStream(Encoding.UTF8.GetBytes(content)) };
            }

            return new MockPipelineResponse(200).WithContent(body);
        })
        {
            ExpectSyncPipeline = !useAsync,
        };

        if (api == "chat")
        {
            var client = new ChatClient("request-model", new ApiKeyCredential("not-a-key"), new OpenAIClientOptions
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
        else
        {
            var client = new ResponsesClient(new ApiKeyCredential("not-a-key"), new ResponsesClientOptions
            {
                Endpoint = s_endpoint,
                Transport = transport,
            });
            var request = new CreateResponseOptions("request-model", [ResponseItem.CreateUserMessageItem("input")])
            {
                StreamingEnabled = api == "stream",
            };

            if (api == "stream")
            {
                if (useAsync)
                {
                    await foreach (var update in client.CreateResponseStreamingAsync(request))
                    {
                    }
                }
                else
                {
                    foreach (var update in client.CreateResponseStreaming(request))
                    {
                    }
                }
            }
            else if (useAsync)
            {
                await client.CreateResponseAsync(request);
            }
            else
            {
                client.CreateResponse(request);
            }
        }

        Assert.That(Activity.Current, Is.SameAs(parent));
        var activity = activities.Activities.Single();
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Unset));
        Assert.That(activity.GetTagItem("gen_ai.usage.cache_read.input_tokens"), Is.EqualTo(latest ? values.ExpectedRead : null));
        Assert.That(activity.GetTagItem("gen_ai.usage.cache_write.input_tokens"), Is.EqualTo(((latest) && (api != "chat")) ? values.ExpectedWrite : null));
        Assert.That(activity.GetTagItem("gen_ai.usage.reasoning.output_tokens"), Is.EqualTo(latest ? values.ExpectedReasoning : null));
        Assert.That(activity.GetTagItem("gen_ai.usage.input_tokens"), Is.EqualTo(latest ? values.Input : values.Input ?? 0));
        Assert.That(activity.GetTagItem("gen_ai.usage.output_tokens"), Is.EqualTo(latest ? values.Output : values.Output ?? 0));
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration"), Has.Count.EqualTo(1));

        if (latest)
        {
            AssertCounter(metrics, "cache_read.input_tokens", values.ExpectedRead);
            AssertCounter(metrics, "cache_write.input_tokens", api == "chat" ? null : values.ExpectedWrite);
            AssertCounter(metrics, "reasoning.output_tokens", values.ExpectedReasoning);
            AssertCounter(metrics, "input_tokens", values.Input);
            AssertCounter(metrics, "output_tokens", values.Output);
            Assert.That(metrics.GetMeasurements(Prefix + "operation.input_tokens")?.Single().value, Is.EqualTo(values.Input));
            Assert.That(metrics.GetMeasurements(Prefix + "operation.output_tokens")?.Single().value, Is.EqualTo(values.Output));
            Assert.That(metrics.GetMeasurements("gen_ai.client.token.usage"), Is.Null);
        }
        else
        {
            foreach (var suffix in new[] { "input_tokens", "output_tokens", "cache_read.input_tokens", "cache_write.input_tokens", "reasoning.output_tokens" })
            {
                Assert.That(metrics.GetMeasurements(Prefix + "usage." + suffix), Is.Null);
            }

            var measurements = metrics.GetMeasurements("gen_ai.client.token.usage");
            Assert.That(measurements, Has.Count.EqualTo(2));
            Assert.That(measurements.Single(measurement => measurement.tags["gen_ai.token.type"].Equals("input")).value, Is.EqualTo(values.Input ?? 0));
            Assert.That(measurements.Single(measurement => measurement.tags["gen_ai.token.type"].Equals("output")).value, Is.EqualTo(values.Output ?? 0));
        }
    }

    private static void AssertCounter(TestMeterListener metrics, string name, long? expected)
    {
        var measurements = metrics.GetMeasurements(Prefix + "usage." + name);
        Assert.That(measurements?.Count ?? 0, Is.EqualTo(expected.HasValue ? 1 : 0), name);

        if (expected.HasValue)
        {
            var measurement = measurements.Single();
            Assert.That(measurement.value, Is.EqualTo(expected.Value), name);
            Assert.That(measurement.tags["gen_ai.token.modality"], Is.EqualTo("unknown"));
        }
    }

    private static UsageCase GetCase(string scenario) => scenario switch
    {
        "exceeds" => new(10, 20, 11, 12, 21, null, null, null),
        "equal" => new(10, 20, 10, 10, 20, 10, 10, 20),
        "zero" => new(0, 0, 0, 0, 0, 0, 0, 0),
        "unknown" => new(null, null, 11, 12, 21, 11, 12, 21),
        "input_unknown" => new(null, 20, 11, 12, 21, 11, 12, null),
        "output_unknown" => new(10, null, 11, 12, 21, null, null, 21),
        "independent" => new(10, 20, 11, 9, 20, null, 9, 20),
        "below" => new(10, 20, 4, 7, 6, 4, 7, 6),
        _ => throw new ArgumentOutOfRangeException(nameof(scenario)),
    };

    private static string ResponseBody(bool chat, UsageCase values)
    {
        var inputName = chat ? "prompt_tokens" : "input_tokens";
        var outputName = chat ? "completion_tokens" : "output_tokens";
        var usage = new Dictionary<string, object>();

        if (values.Input.HasValue)
        {
            usage[inputName] = values.Input.Value;
        }

        if (values.Output.HasValue)
        {
            usage[outputName] = values.Output.Value;
        }

        var inputDetails = new Dictionary<string, object> { ["cached_tokens"] = values.Read };

        if (!chat)
        {
            inputDetails["cache_write_tokens"] = values.Write;
        }
        usage[inputName + "_details"] = inputDetails;
        usage[outputName + "_details"] = new Dictionary<string, object> { ["reasoning_tokens"] = values.Reasoning };
        var response = new Dictionary<string, object>
        {
            ["id"] = "response-id",
            ["model"] = "response-model",
            ["usage"] = usage,
        };

        if (chat)
        {
            response["created"] = 1;
            response["choices"] = new[] { new { index = 0, finish_reason = "stop", message = new { role = "assistant", content = "output" } } };
        }
        else
        {
            response["created_at"] = 1;
            response["status"] = "completed";
            response["output"] = Array.Empty<object>();
            response["parallel_tool_calls"] = false;
        }

        return JsonSerializer.Serialize(response);
    }

    private sealed record UsageCase(int? Input, int? Output, int Read, int Write, int Reasoning,
        long? ExpectedRead, long? ExpectedWrite, long? ExpectedReasoning);
}
