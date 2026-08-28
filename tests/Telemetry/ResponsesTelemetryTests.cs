using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Responses;
using OpenAI.Telemetry;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests.Telemetry;

#pragma warning disable OPENAI001

[TestFixture]
[NonParallelizable]
[Category("Telemetry")]
[Category("Smoke")]
public class ResponsesTelemetryTests
{
    private const string ActivitySourceName = "OpenAI.ResponsesClient";
    private const string RequestModel = "request-model";
    private const string ResponseModel = "response-model";
    private const string ResponseId = "resp_synthetic";
    private const string PreviousResponseId = "resp_previous";
    private const string ConversationId = "conv_synthetic";
    private const string Host = "example.invalid";
    private const string SensitiveInput = "sensitive input";
    private const int Port = 443;
    private const int InputTokens = 12;
    private const int OutputTokens = 34;
    private static readonly Uri s_endpoint = new($"https://{Host}");

    [Test]
    public void AllTelemetryOff()
    {
        var telemetry = new OpenTelemetrySource(s_endpoint);

        Assert.That(telemetry.StartResponsesScope(CreateOptions()), Is.Null);
        Assert.That(Activity.Current, Is.Null);
    }

    [Test]
    public void SwitchOffAllTelemetryOn()
    {
        using var activityListener = new TestActivityListener(ActivitySourceName);
        using var meterListener = new TestMeterListener(ActivitySourceName);
        var telemetry = new OpenTelemetrySource(s_endpoint);

        Assert.That(telemetry.StartResponsesScope(CreateOptions()), Is.Null);
        Assert.That(Activity.Current, Is.Null);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void MetricsEnabledWithoutTracingEmitsMeasurements(bool useLatestSemanticConventions)
    {
        using var _ = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var _semanticConvention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(useLatestSemanticConventions);
        using var meterListener = new TestMeterListener(ActivitySourceName);
        var telemetry = new OpenTelemetrySource(s_endpoint);

        using (var scope = telemetry.StartResponsesScope(CreateOptions()))
        {
            Assert.That(scope, Is.Not.Null);
            Assert.That(Activity.Current, Is.Null);
            scope.RecordResponseResult(CreateResponseResult("completed", null));
        }

        AssertMetrics(meterListener, useLatestSemanticConventions);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void TracingEnabledWithoutMetricsEmitsActivity(bool useLatestSemanticConventions)
    {
        using var _ = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var _semanticConvention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(useLatestSemanticConventions);
        using var activityListener = new TestActivityListener(ActivitySourceName);
        var telemetry = new OpenTelemetrySource(s_endpoint);

        using (var scope = telemetry.StartResponsesScope(CreateOptions()))
        {
            Assert.That(scope, Is.Not.Null);
            Assert.That(Activity.Current, Is.Not.Null);
            scope.RecordResponseResult(CreateResponseResult("completed", null));
        }

        Assert.That(Activity.Current, Is.Null);
        var activity = activityListener.Activities.Single();
        Assert.That(activity.DisplayName, Is.EqualTo($"chat {RequestModel}"));
        Assert.That(activity.GetTagItem("gen_ai.operation.name"), Is.EqualTo("chat"));
        Assert.That(activity.GetTagItem("gen_ai.request.model"), Is.EqualTo(RequestModel));
        Assert.That(activity.GetTagItem("gen_ai.request.max_tokens"), Is.EqualTo(100));
        Assert.That(activity.GetTagItem("gen_ai.response.id"), Is.EqualTo(ResponseId));
        Assert.That(activity.GetTagItem("gen_ai.response.model"), Is.EqualTo(ResponseModel));
        Assert.That(activity.GetTagItem("gen_ai.response.finish_reasons"), Is.EqualTo(new[] { "stop" }));
        Assert.That(activity.GetTagItem("gen_ai.usage.input_tokens"), Is.EqualTo(InputTokens));
        Assert.That(activity.GetTagItem("gen_ai.usage.output_tokens"), Is.EqualTo(OutputTokens));
        AssertProviderAttribute(activity, useLatestSemanticConventions);
    }

    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task CreateResponseEmitsTelemetry(bool useAsync, bool useLatestSemanticConventions)
    {
        using var _ = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var _semanticConvention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(useLatestSemanticConventions);
        using var activityListener = new TestActivityListener(ActivitySourceName);
        using var meterListener = new TestMeterListener(ActivitySourceName);
        var client = CreateClient(CompletedResponseBody, useAsync: useAsync);
        var options = CreateOptions();

        var result = useAsync
            ? await client.CreateResponseAsync(options)
            : client.CreateResponse(options);

        Assert.That(result.Value.Id, Is.EqualTo(ResponseId));
        Assert.That(activityListener.Activities, Has.Count.EqualTo(1));

        var activity = activityListener.Activities.Single();
        Assert.That(activity.DisplayName, Is.EqualTo($"chat {RequestModel}"));
        Assert.That(activity.GetTagItem("gen_ai.operation.name"), Is.EqualTo("chat"));
        Assert.That(activity.GetTagItem("gen_ai.request.model"), Is.EqualTo(RequestModel));
        Assert.That(activity.GetTagItem("server.address"), Is.EqualTo(Host));
        Assert.That(activity.GetTagItem("server.port"), Is.EqualTo(Port));
        Assert.That(activity.GetTagItem("gen_ai.request.max_tokens"), Is.EqualTo(100));
        Assert.That(activity.GetTagItem("gen_ai.request.temperature"), Is.EqualTo(0.4f));
        Assert.That(activity.GetTagItem("gen_ai.request.top_p"), Is.EqualTo(0.8f));
        Assert.That(activity.GetTagItem("gen_ai.response.id"), Is.EqualTo(ResponseId));
        Assert.That(activity.GetTagItem("gen_ai.response.model"), Is.EqualTo(ResponseModel));
        Assert.That(activity.GetTagItem("gen_ai.response.finish_reasons"), Is.EqualTo(new[] { "stop" }));
        Assert.That(activity.GetTagItem("gen_ai.usage.input_tokens"), Is.EqualTo(InputTokens));
        Assert.That(activity.GetTagItem("gen_ai.usage.output_tokens"), Is.EqualTo(OutputTokens));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Unset));
        AssertProviderAttribute(activity, useLatestSemanticConventions);

        if (useLatestSemanticConventions)
        {
            Assert.That(activity.GetTagItem("openai.api.type"), Is.EqualTo("responses"));
            Assert.That(activity.GetTagItem("gen_ai.request.previous_response.id"), Is.EqualTo(PreviousResponseId));
            Assert.That(activity.GetTagItem("gen_ai.conversation.id"), Is.EqualTo(ConversationId));
            Assert.That(activity.GetTagItem("gen_ai.request.reasoning.level"), Is.EqualTo("high"));
            Assert.That(activity.GetTagItem("gen_ai.output.type"), Is.EqualTo("json"));
            Assert.That(activity.GetTagItem("openai.request.service_tier"), Is.EqualTo("flex"));
            Assert.That(activity.GetTagItem("openai.response.service_tier"), Is.EqualTo("default"));
            Assert.That(activity.GetTagItem("gen_ai.usage.cache_read.input_tokens"), Is.EqualTo(5));
            Assert.That(activity.GetTagItem("gen_ai.usage.reasoning.output_tokens"), Is.EqualTo(7));
            Assert.That(activity.GetTagItem("gen_ai.conversation.compacted"), Is.EqualTo(true));
        }
        else
        {
            Assert.That(activity.GetTagItem("openai.api.type"), Is.Null);
            Assert.That(activity.GetTagItem("gen_ai.request.previous_response.id"), Is.Null);
            Assert.That(activity.GetTagItem("gen_ai.conversation.id"), Is.Null);
            Assert.That(activity.GetTagItem("gen_ai.request.reasoning.level"), Is.Null);
            Assert.That(activity.GetTagItem("gen_ai.output.type"), Is.Null);
            Assert.That(activity.GetTagItem("openai.request.service_tier"), Is.Null);
            Assert.That(activity.GetTagItem("openai.response.service_tier"), Is.Null);
            Assert.That(activity.GetTagItem("gen_ai.conversation.compacted"), Is.Null);
        }

        AssertSensitiveDataNotCaptured(activity);
        AssertMetrics(meterListener, useLatestSemanticConventions);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void CreateResponseRecordsHttpErrors(bool useLatestSemanticConventions)
    {
        using var _ = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var _semanticConvention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(useLatestSemanticConventions);
        using var activityListener = new TestActivityListener(ActivitySourceName);
        using var meterListener = new TestMeterListener(ActivitySourceName);
        var client = CreateClient(ErrorResponseBody, 400);

        var exception = Assert.Throws<ClientResultException>(() => client.CreateResponse(CreateOptions()));

        Assert.That(exception.Status, Is.EqualTo(400));
        var activity = activityListener.Activities.Single();
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.GetTagItem("error.type"), Is.EqualTo("400"));
        Assert.That(activity.StatusDescription, Is.Null);
        Assert.That(activity.TagObjects.Select(tag => tag.Value?.ToString()).Append(activity.StatusDescription), Does.Not.Contain("sensitive request failure"));
        AssertProviderAttribute(activity, useLatestSemanticConventions);

        var duration = meterListener.GetMeasurements("gen_ai.client.operation.duration").Single();
        Assert.That(duration.tags["error.type"], Is.EqualTo("400"));
        Assert.That(meterListener.GetMeasurements("gen_ai.client.token.usage"), Is.Null);
    }

    [TestCase("incomplete", "max_output_tokens", "length", null)]
    [TestCase("incomplete", "content_filter", "content_filter", null)]
    [TestCase("failed", null, "error", "server_error")]
    [TestCase("cancelled", null, "error", "cancelled")]
    [TestCase("incomplete", "future_reason", "incomplete", null)]
    public void ResponseStatusIsMapped(string status, string incompleteReason, string finishReason, string errorType)
    {
        using var _ = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var _semanticConvention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var activityListener = new TestActivityListener(ActivitySourceName);
        using var meterListener = new TestMeterListener(ActivitySourceName);
        var telemetry = new OpenTelemetrySource(s_endpoint);
        var response = CreateResponseResult(status, incompleteReason);

        using (var scope = telemetry.StartResponsesScope(CreateOptions()))
        {
            scope.RecordResponseResult(response);
        }

        var activity = activityListener.Activities.Single();
        Assert.That(activity.GetTagItem("gen_ai.response.finish_reasons"), Is.EqualTo(new[] { finishReason }));
        Assert.That(activity.GetTagItem("error.type"), Is.EqualTo(errorType));
        Assert.That(activity.Status, Is.EqualTo(errorType is null ? ActivityStatusCode.Unset : ActivityStatusCode.Error));

        var duration = meterListener.GetMeasurements("gen_ai.client.operation.duration").Single();
        Assert.That(duration.tags.TryGetValue("error.type", out var actualErrorType), Is.EqualTo(errorType is not null));
        Assert.That(actualErrorType, Is.EqualTo(errorType));
    }

    [Test]
    public void RequiredAttributesAreAvailableToSampler()
    {
        using var _ = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var _semanticConvention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        IEnumerable<KeyValuePair<string, object>> creationTags = null;
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> options) =>
            {
                creationTags = options.Tags.ToArray();
                return ActivitySamplingResult.AllDataAndRecorded;
            },
        };
        ActivitySource.AddActivityListener(listener);
        var telemetry = new OpenTelemetrySource(s_endpoint);

        using (telemetry.StartResponsesScope(CreateOptions()))
        {
        }

        var tags = creationTags.ToDictionary(tag => tag.Key, tag => tag.Value);
        Assert.That(tags["gen_ai.provider.name"], Is.EqualTo("openai"));
        Assert.That(tags["gen_ai.operation.name"], Is.EqualTo("chat"));
        Assert.That(tags["gen_ai.request.model"], Is.EqualTo(RequestModel));
        Assert.That(tags["openai.api.type"], Is.EqualTo("responses"));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void MissingRequestModelEmitsTelemetry(bool useLatestSemanticConventions)
    {
        using var _ = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var _semanticConvention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(useLatestSemanticConventions);
        using var activityListener = new TestActivityListener(ActivitySourceName);
        using var meterListener = new TestMeterListener(ActivitySourceName);
        var client = CreateClient(CompletedResponseBody);

        client.CreateResponse(new CreateResponseOptions());

        var activity = activityListener.Activities.Single();
        Assert.That(activity.DisplayName, Is.EqualTo("chat"));
        Assert.That(activity.GetTagItem("gen_ai.request.model"), Is.Null);
        Assert.That(activity.GetTagItem("gen_ai.response.model"), Is.EqualTo(ResponseModel));
        AssertProviderAttribute(activity, useLatestSemanticConventions);

        var duration = meterListener.GetMeasurements("gen_ai.client.operation.duration").Single();
        Assert.That(duration.tags.ContainsKey("gen_ai.request.model"), Is.False);
        Assert.That(duration.tags["gen_ai.response.model"], Is.EqualTo(ResponseModel));

        var usage = meterListener.GetMeasurements("gen_ai.client.token.usage");
        Assert.That(usage, Has.Count.EqualTo(2));
        Assert.That(usage.All(measurement => !measurement.tags.ContainsKey("gen_ai.request.model")), Is.True);
        Assert.That(usage.All(measurement => measurement.tags["gen_ai.response.model"].Equals(ResponseModel)), Is.True);
    }

    [Test]
    public void ProtocolMethodIsNotInstrumented()
    {
        using var _ = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var activityListener = new TestActivityListener(ActivitySourceName);
        using var meterListener = new TestMeterListener(ActivitySourceName);
        var client = CreateClient(CompletedResponseBody);

        client.CreateResponse(BinaryContent.Create(BinaryData.FromString("""{"model":"request-model","input":"hello"}""")));

        Assert.That(activityListener.Activities, Is.Empty);
        Assert.That(meterListener.GetMeasurements("gen_ai.client.operation.duration"), Is.Null);
        Assert.That(meterListener.GetMeasurements("gen_ai.client.token.usage"), Is.Null);
    }

    [Test]
    public void ConvenienceMethodEmitsSingleOperation()
    {
        using var _ = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var activityListener = new TestActivityListener(ActivitySourceName);
        using var meterListener = new TestMeterListener(ActivitySourceName);
        var client = CreateClient(CompletedResponseBody);

        client.CreateResponse(RequestModel, SensitiveInput);

        Assert.That(activityListener.Activities, Has.Count.EqualTo(1));
        Assert.That(meterListener.GetMeasurements("gen_ai.client.operation.duration"), Has.Count.EqualTo(1));
        Assert.That(meterListener.GetMeasurements("gen_ai.client.token.usage"), Has.Count.EqualTo(2));
    }

    private static ResponsesClient CreateClient(string responseBody, int status = 200, bool useAsync = false)
    {
        var transport = new MockPipelineTransport(_ => new MockPipelineResponse(status).WithContent(responseBody))
        {
            ExpectSyncPipeline = !useAsync,
        };
        var options = new ResponsesClientOptions
        {
            Endpoint = s_endpoint,
            Transport = transport,
        };
        return new ResponsesClient(new ApiKeyCredential("not-a-real-key"), options);
    }

    private static CreateResponseOptions CreateOptions()
    {
        var options = new CreateResponseOptions(RequestModel, [ResponseItem.CreateUserMessageItem(SensitiveInput)])
        {
            ConversationOptions = new ResponseConversationOptions
            {
                ConversationId = ConversationId,
            },
            EndUserId = "sensitive-user",
            Instructions = "sensitive instructions",
            MaxOutputTokenCount = 100,
            PreviousResponseId = PreviousResponseId,
            ReasoningOptions = new ResponseReasoningOptions
            {
                ReasoningEffortLevel = ResponseReasoningEffortLevel.High,
            },
            SafetyIdentifier = "sensitive-safety-id",
            ServiceTier = ResponseServiceTier.Flex,
            Temperature = 0.4f,
            TextOptions = new ResponseTextOptions
            {
                TextFormat = ResponseTextFormat.CreateJsonObjectFormat(),
            },
            TopP = 0.8f,
        };
        options.Metadata["sensitive-key"] = "sensitive-value";
        return options;
    }

    private static ResponseResult CreateResponseResult(string status, string incompleteReason)
    {
        var error = status == "failed"
            ? "\"error\":{\"code\":\"server_error\",\"message\":\"synthetic failure\",\"param\":null,\"type\":\"server_error\"},"
            : "\"error\":null,";
        var incompleteDetails = incompleteReason is null
            ? "\"incomplete_details\":null,"
            : $"\"incomplete_details\":{{\"reason\":\"{incompleteReason}\"}},";

        return ModelReaderWriter.Read<ResponseResult>(BinaryData.FromString(
            $$"""
            {
              "id": "{{ResponseId}}",
              "object": "response",
              "created_at": 1,
              "status": "{{status}}",
              {{error}}
              {{incompleteDetails}}
              "model": "{{ResponseModel}}",
              "output": [],
              "parallel_tool_calls": false,
              "service_tier": "default",
              "tools": [],
              "usage": {
                "input_tokens": {{InputTokens}},
                "input_tokens_details": {"cached_tokens": 5},
                "output_tokens": {{OutputTokens}},
                "output_tokens_details": {"reasoning_tokens": 7},
                "total_tokens": 46
              }
            }
            """));
    }

    private static void AssertProviderAttribute(Activity activity, bool useLatestSemanticConventions)
    {
        if (useLatestSemanticConventions)
        {
            Assert.That(activity.GetTagItem("gen_ai.provider.name"), Is.EqualTo("openai"));
            Assert.That(activity.GetTagItem("gen_ai.system"), Is.Null);
        }
        else
        {
            Assert.That(activity.GetTagItem("gen_ai.system"), Is.EqualTo("openai"));
            Assert.That(activity.GetTagItem("gen_ai.provider.name"), Is.Null);
        }
    }

    private static void AssertSensitiveDataNotCaptured(Activity activity)
    {
        Assert.That(activity.GetTagItem("gen_ai.input.messages"), Is.Null);
        Assert.That(activity.GetTagItem("gen_ai.output.messages"), Is.Null);
        Assert.That(activity.GetTagItem("gen_ai.system_instructions"), Is.Null);
        Assert.That(activity.GetTagItem("gen_ai.tool.definitions"), Is.Null);
        Assert.That(activity.GetTagItem("user.id"), Is.Null);
        var telemetryValues = activity.TagObjects.Select(tag => tag.Value?.ToString()).Append(activity.StatusDescription);
        Assert.That(telemetryValues, Does.Not.Contain(SensitiveInput));
        Assert.That(telemetryValues, Does.Not.Contain("sensitive output"));
        Assert.That(telemetryValues, Does.Not.Contain("sensitive instructions"));
        Assert.That(telemetryValues, Does.Not.Contain("sensitive-user"));
        Assert.That(telemetryValues, Does.Not.Contain("sensitive-safety-id"));
        Assert.That(telemetryValues, Does.Not.Contain("sensitive-value"));
    }

    private static void AssertMetrics(TestMeterListener meterListener, bool useLatestSemanticConventions)
    {
        var durations = meterListener.GetMeasurements("gen_ai.client.operation.duration");
        Assert.That(durations, Has.Count.EqualTo(1));
        Assert.That(durations[0].tags["gen_ai.operation.name"], Is.EqualTo("chat"));
        Assert.That(durations[0].tags["gen_ai.request.model"], Is.EqualTo(RequestModel));
        Assert.That(durations[0].tags["gen_ai.response.model"], Is.EqualTo(ResponseModel));

        var usage = meterListener.GetMeasurements("gen_ai.client.token.usage");
        Assert.That(usage, Has.Count.EqualTo(2));
        Assert.That(usage.Single(measurement => measurement.tags["gen_ai.token.type"].Equals("input")).value, Is.EqualTo(InputTokens));
        Assert.That(usage.Single(measurement => measurement.tags["gen_ai.token.type"].Equals("output")).value, Is.EqualTo(OutputTokens));

        foreach (var measurement in durations.Concat(usage))
        {
            if (useLatestSemanticConventions)
            {
                Assert.That(measurement.tags["gen_ai.provider.name"], Is.EqualTo("openai"));
                Assert.That(measurement.tags.ContainsKey("openai.api.type"), Is.False);
                Assert.That(measurement.tags["openai.response.service_tier"], Is.EqualTo("default"));
            }
            else
            {
                Assert.That(measurement.tags["gen_ai.system"], Is.EqualTo("openai"));
                Assert.That(measurement.tags.ContainsKey("openai.api.type"), Is.False);
                Assert.That(measurement.tags.ContainsKey("openai.response.service_tier"), Is.False);
            }
        }
    }

    private const string CompletedResponseBody =
        """
        {
          "id": "resp_synthetic",
          "object": "response",
          "created_at": 1,
          "status": "completed",
          "error": null,
          "incomplete_details": null,
          "model": "response-model",
          "output": [
            {
              "id": "msg_synthetic",
              "type": "message",
              "status": "completed",
              "content": [
                {
                  "type": "output_text",
                  "annotations": [],
                  "logprobs": [],
                  "text": "sensitive output"
                }
              ],
              "role": "assistant"
            },
            {
              "id": "cmp_synthetic",
              "type": "compaction",
              "encrypted_content": "sensitive compaction content"
            }
          ],
          "parallel_tool_calls": false,
          "service_tier": "default",
          "tools": [],
          "usage": {
            "input_tokens": 12,
            "input_tokens_details": {"cached_tokens": 5},
            "output_tokens": 34,
            "output_tokens_details": {"reasoning_tokens": 7},
            "total_tokens": 46
          }
        }
        """;

    private const string ErrorResponseBody =
        """
        {
          "error": {
            "message": "sensitive request failure",
            "type": "invalid_request_error",
            "param": null,
            "code": "invalid_request"
          }
        }
        """;
}
