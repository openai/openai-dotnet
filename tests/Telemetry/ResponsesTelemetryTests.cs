using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Responses;
using OpenAI.Telemetry;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
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

    [Test]
    public void ManuallyPopulatedResponseCountsAreAvailableWithoutInventingZeros()
    {
        var usage = ModelReaderWriter.Read<ResponseTokenUsage>(BinaryData.FromString("{}"));
        usage.InputTokenCount = 12;
        usage.OutputTokenCount = 34;
        usage.InputTokenDetails = ModelReaderWriter.Read<ResponseInputTokenUsageDetails>(BinaryData.FromString("{}"));
        usage.InputTokenDetails.CachedTokenCount = 5;
        usage.OutputTokenDetails = ModelReaderWriter.Read<ResponseOutputTokenUsageDetails>(BinaryData.FromString("{}"));
        usage.OutputTokenDetails.ReasoningTokenCount = 7;

        var normalized = OpenTelemetryTokenUsage.FromResponse(usage);
        Assert.That(normalized.InputTokens, Is.EqualTo(12L));
        Assert.That(normalized.OutputTokens, Is.EqualTo(34L));
        Assert.That(normalized.CacheReadInputTokens, Is.EqualTo(5L));
        Assert.That(normalized.ReasoningOutputTokens, Is.EqualTo(7L));
        Assert.That(normalized.CacheWriteInputTokens, Is.Null);
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

    [TestCase(ActivityIdFormat.W3C)]
    [TestCase(ActivityIdFormat.Hierarchical)]
    public void ActivityPreservesAmbientParent(ActivityIdFormat parentIdFormat)
    {
        using var _ = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var activityListener = new TestActivityListener(ActivitySourceName);
        using var parent = new Activity("parent")
            .SetIdFormat(parentIdFormat)
            .AddBaggage("baggage-key", "baggage-value")
            .Start();
        var telemetry = new OpenTelemetrySource(s_endpoint);

        using (var scope = telemetry.StartResponsesScope(CreateOptions()))
        {
            Assert.That(scope, Is.Not.Null);
            Assert.That(Activity.Current?.Parent, Is.SameAs(parent));
        }

        Assert.That(Activity.Current, Is.SameAs(parent));
        var activity = activityListener.Activities.Single();
        Assert.That(activity.Parent, Is.SameAs(parent));
        Assert.That(activity.GetBaggageItem("baggage-key"), Is.EqualTo("baggage-value"));
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

        var usage = GetTotalUsageMeasurements(meterListener, useLatestSemanticConventions);
        Assert.That(usage, Has.Count.EqualTo(2));
        Assert.That(usage.All(measurement => !measurement.tags.ContainsKey("gen_ai.request.model")), Is.True);
        Assert.That(usage.All(measurement => measurement.tags["gen_ai.response.model"].Equals(ResponseModel)), Is.True);
    }

    [Test]
    public void UnknownResponseErrorCodeUsesStableFallback()
    {
        using var _ = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var activityListener = new TestActivityListener(ActivitySourceName);
        using var meterListener = new TestMeterListener(ActivitySourceName);
        var telemetry = new OpenTelemetrySource(s_endpoint);
        var response = CreateResponseResult("failed", null, "request-specific-error");

        using (var scope = telemetry.StartResponsesScope(CreateOptions()))
        {
            scope.RecordResponseResult(response);
        }

        var activity = activityListener.Activities.Single();
        Assert.That(activity.GetTagItem("error.type"), Is.EqualTo("failed"));
        var duration = meterListener.GetMeasurements("gen_ai.client.operation.duration").Single();
        Assert.That(duration.tags["error.type"], Is.EqualTo("failed"));
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

    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task StreamingIsLazyAndPreservesAmbientActivity(bool useAsync, bool latest)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(latest);
        using var activities = new TestActivityListener(ActivitySourceName);
        using var metrics = new TestMeterListener(ActivitySourceName);
        var children = new List<Activity>();
        var client = CreateStreamingClient(
            SseEvent("""{"type":"response.future_event","sequence_number":0}""")
            + SseEvent("""{"type":"response.output_text.delta","sequence_number":1,"output_index":0,"content_index":0,"item_id":"message","delta":"sensitive output","logprobs":[]}""")
            + SseEvent("""{"type":"response.output_text.delta","sequence_number":2,"output_index":0,"content_index":0,"item_id":"message","delta":"sensitive output","logprobs":[]}""")
            + TerminalEvent(), useAsync, () =>
            {
                using var child = new Activity("synthetic HTTP").Start();
                children.Add(child);
                Assert.That(child.GetBaggageItem("caller-baggage"), Is.EqualTo("retained"));
            });
        var options = CreateOptions();
        options.StreamingEnabled = true;
        var sync = useAsync ? null : client.CreateResponseStreaming(options);
        var async = useAsync ? client.CreateResponseStreamingAsync(options) : null;
        Assert.That(activities.Activities, Is.Empty);
        Assert.That(children, Is.Empty);
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration"), Is.Null);

        using var parent = new Activity("parent").AddBaggage("caller-baggage", "retained").Start();

        for (var attempt = 0; attempt < 2; attempt++)
        {
            if (useAsync)
            {
                await using var enumerator = async.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync())
                {
                    Assert.That(Activity.Current, Is.SameAs(parent));
                }
            }
            else
            {
                using var enumerator = sync.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    Assert.That(Activity.Current, Is.SameAs(parent));
                }
            }

            Assert.That(Activity.Current, Is.SameAs(parent));
        }

        Assert.That(activities.Activities, Has.Count.EqualTo(2));

        foreach (var activity in activities.Activities)
        {
            Assert.That(activity.ParentId, Is.EqualTo(parent.Id));
            Assert.That(activity.GetBaggageItem("caller-baggage"), Is.EqualTo("retained"));
            Assert.That(activity.GetTagItem("gen_ai.request.stream"), Is.EqualTo(latest ? true : null));
            Assert.That(activity.GetTagItem("gen_ai.response.id"), Is.EqualTo(ResponseId));
            Assert.That(activity.GetTagItem("gen_ai.response.finish_reasons"), Is.EqualTo(new[] { "stop" }));
            Assert.That(children.Any(child => child.ParentId == activity.Id), Is.True);
            AssertProviderAttribute(activity, latest);
            AssertSensitiveDataNotCaptured(activity);
        }

        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration"), Has.Count.EqualTo(2));
        Assert.That(GetTotalUsageMeasurements(metrics, latest), Has.Count.EqualTo(4));
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.time_to_first_chunk")?.Count ?? 0, Is.EqualTo(latest ? 2 : 0));
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.time_per_output_chunk")?.Count ?? 0, Is.EqualTo(latest ? 2 : 0));
    }

    [TestCase(false, "completed", null, "stop", null)]
    [TestCase(true, "completed", null, "stop", null)]
    [TestCase(false, "failed", null, "error", "server_error")]
    [TestCase(true, "failed", null, "error", "server_error")]
    [TestCase(false, "incomplete", "max_output_tokens", "length", null)]
    [TestCase(true, "incomplete", "content_filter", "content_filter", null)]
    public async Task StreamingTerminalUpdatesAreRecordedExactlyOnce(bool useAsync, string status, string reason, string finish, string error)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var activities = new TestActivityListener(ActivitySourceName);
        using var metrics = new TestMeterListener(ActivitySourceName);
        var body = CreateResponseBody(status, reason);
        var terminal = SseEvent($$"""{"type":"response.{{status}}","sequence_number":1,"response":{{body}}}""");
        var client = CreateStreamingClient(terminal + terminal, useAsync);
        await ConsumeStream(client, useAsync);

        var activity = activities.Activities.Single();
        Assert.That(activity.GetTagItem("gen_ai.response.finish_reasons"), Is.EqualTo(new[] { finish }));
        Assert.That(activity.GetTagItem("error.type"), Is.EqualTo(error));
        Assert.That(activity.GetTagItem("gen_ai.usage.input_tokens"), Is.EqualTo(InputTokens));
        Assert.That(activity.GetTagItem("gen_ai.usage.output_tokens"), Is.EqualTo(OutputTokens));
        Assert.That(activity.GetTagItem("gen_ai.usage.cache_read.input_tokens"), Is.EqualTo(5));
        Assert.That(activity.GetTagItem("gen_ai.usage.reasoning.output_tokens"), Is.EqualTo(7));
        AssertMetrics(metrics, true);
    }

    [TestCase(false, "eof", "incomplete_stream")]
    [TestCase(true, "eof", "incomplete_stream")]
    [TestCase(false, "done", "incomplete_stream")]
    [TestCase(true, "done", "incomplete_stream")]
    [TestCase(false, "error", "stream_error")]
    [TestCase(true, "error", "stream_error")]
    [TestCase(false, "malformed", "System.Text.Json.JsonReaderException")]
    [TestCase(true, "malformed", "System.Text.Json.JsonReaderException")]
    public async Task StreamingMissingTerminalDoesNotInventUsage(bool useAsync, string ending, string errorType)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var activities = new TestActivityListener(ActivitySourceName);
        using var metrics = new TestMeterListener(ActivitySourceName);
        var content = ending switch
        {
            "done" => "data: [DONE]\n\n",
            "error" => SseEvent("""{"type":"error","sequence_number":1,"code":"sensitive code","message":"sensitive output","param":"sensitive input"}"""),
            "malformed" => "data: {invalid\n\n",
            _ => SseEvent("""{"type":"response.future_event","sequence_number":1}"""),
        };
        var client = CreateStreamingClient(content, useAsync);

        if (ending == "malformed")
        {
            Assert.That(async () => await ConsumeStream(client, useAsync), Throws.InstanceOf<JsonException>());
        }
        else
        {
            await ConsumeStream(client, useAsync);
        }

        var activity = activities.Activities.Single();
        Assert.That(activity.GetTagItem("error.type"), Is.EqualTo(errorType));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.StatusDescription, Is.Null);
        Assert.That(activity.GetTagItem("gen_ai.response.finish_reasons"), Is.EqualTo(new[] { "error" }));
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration"), Has.Count.EqualTo(1));
        Assert.That(metrics.GetMeasurements("gen_ai.client.token.usage"), Is.Null);
        AssertSensitiveDataNotCaptured(activity);
    }

    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task StreamingDisposalAndCancellationPreserveParent(bool useAsync, bool cancel)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var activities = new TestActivityListener(ActivitySourceName);
        using var metrics = new TestMeterListener(ActivitySourceName);
        using var cancellation = new CancellationTokenSource();
        using var parent = new Activity("parent").Start();
        var content = SseEvent("""{"type":"response.future_event","sequence_number":1}""") + TerminalEvent();
        var client = CreateStreamingClient(content, useAsync);
        var options = CreateOptions();
        options.StreamingEnabled = true;

        if (useAsync)
        {
            var enumerator = client.CreateResponseStreamingAsync(options, cancellation.Token).GetAsyncEnumerator();
            Assert.That(await enumerator.MoveNextAsync(), Is.True);

            if (cancel)
            {
                cancellation.Cancel();
                Assert.ThrowsAsync<OperationCanceledException>(async () => await enumerator.MoveNextAsync());
            }
            await enumerator.DisposeAsync();
            await enumerator.DisposeAsync();
        }
        else
        {
            var enumerator = client.CreateResponseStreaming(options, cancellation.Token).GetEnumerator();
            Assert.That(enumerator.MoveNext(), Is.True);

            if (cancel)
            {
                cancellation.Cancel();
                Assert.Throws<OperationCanceledException>(() => enumerator.MoveNext());
            }

            enumerator.Dispose();
            enumerator.Dispose();
        }

        Assert.That(Activity.Current, Is.SameAs(parent));
        Assert.That(activities.Activities.Single().GetTagItem("error.type"),
            Is.EqualTo(cancel ? typeof(OperationCanceledException).FullName : "cancelled"));
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration"), Has.Count.EqualTo(1));
        Assert.That(metrics.GetMeasurements("gen_ai.client.token.usage"), Is.Null);
    }

    [Test]
    public async Task RawStreamingActivityAndDurationCoverBodyWithoutTokenOrTimingMeasurements(
        [Values] bool useAsync, [Values] bool consumeBody, [Values] bool disposeEarly, [Values] bool latest)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(latest);
        using var activities = new TestActivityListener(ActivitySourceName);
        using var metrics = new TestMeterListener(ActivitySourceName);
        var client = CreateStreamingClient(TerminalEvent(), useAsync);
        var options = CreateOptions();
        options.StreamingEnabled = true;
        using var parent = new Activity("raw-caller").Start();
        PipelineResponse retainedResponse = null;

        try
        {
            if (useAsync)
            {
                await foreach (var page in client.CreateResponseStreamingAsync(options).GetRawPagesAsync())
                {
                    retainedResponse = page.GetRawResponse();
                    Assert.That(Activity.Current, Is.SameAs(parent));
                    Assert.That(retainedResponse.ContentStream.Position, Is.Zero);

                    if (consumeBody)
                    {
                        await retainedResponse.ContentStream.CopyToAsync(Stream.Null);
                    }

                    if (disposeEarly)
                    {
                        break;
                    }
                }
            }
            else
            {
                foreach (var page in client.CreateResponseStreaming(options).GetRawPages())
                {
                    retainedResponse = page.GetRawResponse();
                    Assert.That(Activity.Current, Is.SameAs(parent));
                    Assert.That(retainedResponse.ContentStream.Position, Is.Zero);

                    if (consumeBody)
                    {
                        retainedResponse.ContentStream.CopyTo(Stream.Null);
                    }

                    if (disposeEarly)
                    {
                        break;
                    }
                }
            }

            Assert.That(Activity.Current, Is.SameAs(parent));
            Assert.That(retainedResponse, Is.Not.Null);
            Assert.That(activities.Activities, Has.Count.EqualTo(((latest) && (!consumeBody)) ? 0 : 1));
            Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration")?.Count ?? 0,
                Is.EqualTo(((latest) && (consumeBody)) ? 1 : 0));
            AssertNoStreamingTokenOrTimingMeasurements(metrics);

            if (!consumeBody)
            {
                Assert.That(retainedResponse.ContentStream.Position, Is.Zero);

                if (useAsync)
                {
                    await retainedResponse.ContentStream.CopyToAsync(Stream.Null);
                }
                else
                {
                    retainedResponse.ContentStream.CopyTo(Stream.Null);
                }
            }

            var activity = activities.Activities.Single();
            Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Unset));
            Assert.That(activity.GetTagItem("error.type"), Is.Null);
            Assert.That(activity.GetTagItem("gen_ai.response.finish_reasons"), Is.Null);
            Assert.That(activity.GetTagItem("gen_ai.response.time_to_first_chunk"), Is.Null);
            Assert.That(activity.ParentId, Is.EqualTo(parent.Id));
            Assert.That(Activity.Current, Is.SameAs(parent));
            Assert.That(retainedResponse.ContentStream.Position, Is.EqualTo(retainedResponse.ContentStream.Length));
        }
        finally
        {
            retainedResponse?.Dispose();
        }

        Assert.That(Activity.Current, Is.SameAs(parent));
        Assert.That(activities.Activities, Has.Count.EqualTo(1));
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration")?.Count ?? 0, Is.EqualTo(latest ? 1 : 0));

        if (latest)
        {
            Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration").Single().tags.ContainsKey("error.type"), Is.False);
        }

        AssertNoStreamingTokenOrTimingMeasurements(metrics);
    }

    [Test]
    public async Task RawAndTypedEnumerationsHaveIndependentLifetimes([Values] bool useAsync, [Values] bool rawFirst)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var activities = new TestActivityListener(ActivitySourceName);
        using var metrics = new TestMeterListener(ActivitySourceName);
        using var parent = new Activity("caller").Start();
        var client = CreateStreamingClient(TerminalEvent(), useAsync);
        var options = CreateOptions();
        options.StreamingEnabled = true;
        PipelineResponse raw = null;

        try
        {
            if (useAsync)
            {
                var collection = client.CreateResponseStreamingAsync(options);
                async Task ReadRaw()
                {
                    await foreach (var page in collection.GetRawPagesAsync())
                    {
                        raw = page.GetRawResponse();
                    }
                }

                if (rawFirst)
                {
                    await ReadRaw();
                    Assert.That(activities.Activities, Is.Empty);
                }

                await foreach (var update in collection)
                {
                }

                if (!rawFirst)
                {
                    await ReadRaw();
                }
            }
            else
            {
                var collection = client.CreateResponseStreaming(options);

                if (rawFirst)
                {
                    raw = collection.GetRawPages().Single().GetRawResponse();
                    Assert.That(activities.Activities, Is.Empty);
                }

                collection.ToList();

                if (!rawFirst)
                {
                    raw = collection.GetRawPages().Single().GetRawResponse();
                }
            }

            Assert.That(Activity.Current, Is.SameAs(parent));
            Assert.That(activities.Activities, Has.Count.EqualTo(1));
            Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration"), Has.Count.EqualTo(1));
            Assert.That(raw.ContentStream.Position, Is.Zero);
            raw.Dispose();
            Assert.That(activities.Activities, Has.Count.EqualTo(2));
            Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration"), Has.Count.EqualTo(2));
            Assert.That(activities.Activities.All(activity => activity.ParentId == parent.Id), Is.True);
            Assert.That(Activity.Current, Is.SameAs(parent));
        }
        finally
        {
            raw?.Dispose();
        }
    }

    [TestCase(false)]
    [TestCase(true)]
    public void RawStreamingSendFailureRecordsOneErrorAndRestoresAmbientActivity(bool useAsync)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var activities = new TestActivityListener(ActivitySourceName);
        using var metrics = new TestMeterListener(ActivitySourceName);
        using var parent = new Activity("raw-caller").Start();
        var client = CreateStreamingClient(TerminalEvent(), useAsync,
            () => throw new IOException("sensitive send failure"), maxRetries: 0);
        var options = CreateOptions();
        options.StreamingEnabled = true;

        if (useAsync)
        {
            Assert.ThrowsAsync<IOException>(async () =>
            {
                await foreach (var page in client.CreateResponseStreamingAsync(options).GetRawPagesAsync())
                {
                    page.GetRawResponse().Dispose();
                }
            });
        }
        else
        {
            Assert.Throws<IOException>(() =>
            {
                foreach (var page in client.CreateResponseStreaming(options).GetRawPages())
                {
                    page.GetRawResponse().Dispose();
                }
            });
        }

        Assert.That(Activity.Current, Is.SameAs(parent));
        var activity = activities.Activities.Single();
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.GetTagItem("error.type"), Is.EqualTo(typeof(IOException).FullName));
        Assert.That(activity.GetTagItem("gen_ai.response.finish_reasons"), Is.Null);
        Assert.That(activity.GetTagItem("gen_ai.response.time_to_first_chunk"), Is.Null);
        Assert.That(activity.StatusDescription, Is.Null);
        var duration = metrics.GetMeasurements("gen_ai.client.operation.duration").Single();
        Assert.That(duration.tags["error.type"], Is.EqualTo(typeof(IOException).FullName));
        AssertNoStreamingTokenOrTimingMeasurements(metrics);
    }

    private static void AssertNoStreamingTokenOrTimingMeasurements(TestMeterListener metrics)
    {
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.time_to_first_chunk"), Is.Null);
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.time_per_output_chunk"), Is.Null);
        Assert.That(metrics.GetMeasurements("gen_ai.client.token.usage"), Is.Null);

        foreach (var name in new[]
        {
            "usage.input_tokens", "usage.output_tokens",
            "usage.cache_read.input_tokens", "usage.cache_write.input_tokens", "usage.reasoning.output_tokens",
            "operation.input_tokens", "operation.output_tokens",
        })
        {
            Assert.That(metrics.GetMeasurements($"gen_ai.client.inference.{name}"), Is.Null);
        }
    }

    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task StreamingSignalsAreIndependent(bool trace, bool measure)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var activities = trace ? new TestActivityListener(ActivitySourceName) : null;
        using var metrics = measure ? new TestMeterListener(ActivitySourceName) : null;
        await ConsumeStream(CreateStreamingClient(TerminalEvent(), true), true);

        if (trace)
        {
            Assert.That(activities.Activities, Has.Count.EqualTo(1));
        }

        if (measure)
        {
            AssertMetrics(metrics, true);
        }

        Assert.That(Activity.Current, Is.Null);
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task StreamingFeatureSwitchOffSuppressesAllSignals(bool useAsync)
    {
        using var activities = new TestActivityListener(ActivitySourceName);
        using var metrics = new TestMeterListener(ActivitySourceName);
        await ConsumeStream(CreateStreamingClient(TerminalEvent(), useAsync), useAsync);
        Assert.That(activities.Activities, Is.Empty);
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration"), Is.Null);
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.time_to_first_chunk"), Is.Null);
        Assert.That(metrics.GetMeasurements("gen_ai.client.token.usage"), Is.Null);
    }

    [TestCase(false, ActivityIdFormat.W3C)]
    [TestCase(true, ActivityIdFormat.W3C)]
    [TestCase(false, ActivityIdFormat.Hierarchical)]
    [TestCase(true, ActivityIdFormat.Hierarchical)]
    public async Task StreamingWithoutModelPreservesParentAndSamplerTags(bool useAsync, ActivityIdFormat format)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var metrics = new TestMeterListener(ActivitySourceName);
        var sampledTags = new List<KeyValuePair<string, object>>();
        Activity activity = null;
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> creation) =>
            {
                sampledTags.AddRange(creation.Tags);

                return ActivitySamplingResult.AllDataAndRecorded;
            },
            SampleUsingParentId = (ref ActivityCreationOptions<string> creation) =>
            {
                sampledTags.AddRange(creation.Tags);

                return ActivitySamplingResult.AllDataAndRecorded;
            },
            ActivityStopped = stopped => activity = stopped,
        };
        ActivitySource.AddActivityListener(listener);
        using var parent = new Activity("parent").SetIdFormat(format).Start();
        var client = CreateStreamingClient(TerminalEvent(), useAsync);
        var options = CreateOptions();
        options.Model = null;
        options.StreamingEnabled = true;

        if (useAsync)
        {
            await foreach (var update in client.CreateResponseStreamingAsync(options))
            {
            }
        }
        else
        {
            foreach (var update in client.CreateResponseStreaming(options))
            {
            }
        }

        Assert.That(Activity.Current, Is.SameAs(parent));
        Assert.That(activity.ParentId, Is.EqualTo(parent.Id));
        Assert.That(activity.DisplayName, Is.EqualTo("chat"));
        Assert.That(activity.GetTagItem("gen_ai.request.model"), Is.Null);
        var tags = sampledTags.ToDictionary(tag => tag.Key, tag => tag.Value);
        Assert.That(tags["gen_ai.request.stream"], Is.EqualTo(true));
        Assert.That(tags["gen_ai.provider.name"], Is.EqualTo("openai"));
        Assert.That(tags["gen_ai.operation.name"], Is.EqualTo("chat"));
        Assert.That(tags["openai.api.type"], Is.EqualTo("responses"));
        Assert.That(tags["server.address"], Is.EqualTo(Host));
        Assert.That(tags["server.port"], Is.EqualTo(Port));
        Assert.That(tags.ContainsKey("gen_ai.request.model"), Is.False);
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration").Single().tags.ContainsKey("gen_ai.request.model"), Is.False);
    }

    [TestCase(false, "send")]
    [TestCase(true, "send")]
    [TestCase(false, "read")]
    [TestCase(true, "read")]
    [TestCase(false, "cancel")]
    [TestCase(true, "cancel")]
    public void StreamingTransportFailuresAndPreCancellationEndTelemetry(bool useAsync, string phase)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var activities = new TestActivityListener(ActivitySourceName);
        using var metrics = new TestMeterListener(ActivitySourceName);
        using var parent = new Activity("parent").Start();
        using var cancellation = new CancellationTokenSource();

        if (phase == "cancel")
        {
            cancellation.Cancel();
        }

        var client = CreateStreamingClient(TerminalEvent(), useAsync,
            onSend: () =>
            {
                if (phase == "send")
                {
                    throw new InvalidOperationException("sensitive transport failure");
                }
            },
            streamFactory: phase == "read" ? () => new ThrowingReadStream() : null);
        Assert.That(async () => await ConsumeStream(client, useAsync, cancellation.Token), Throws.Exception);
        Assert.That(Activity.Current, Is.SameAs(parent));
        var activity = activities.Activities.Single();
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.StatusDescription, Is.Null);
        Assert.That(activity.GetTagItem("error.type"), Is.Not.Null);
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration"), Has.Count.EqualTo(1));
        Assert.That(metrics.GetMeasurements("gen_ai.client.token.usage"), Is.Null);
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task TerminalWithoutUsageDoesNotManufactureTokenCounts(bool useAsync)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var activities = new TestActivityListener(ActivitySourceName);
        using var metrics = new TestMeterListener(ActivitySourceName);
        var body = System.Text.Json.Nodes.JsonNode.Parse(CompletedResponseBody);
        body["usage"] = null;
        var content = SseEvent($$"""{"type":"response.completed","sequence_number":1,"response":{{body.ToJsonString()}}}""");
        var client = CreateStreamingClient(content, useAsync);
        await ConsumeStream(client, useAsync);
        Assert.That(activities.Activities.Single().GetTagItem("gen_ai.usage.input_tokens"), Is.Null);
        Assert.That(activities.Activities.Single().GetTagItem("gen_ai.usage.output_tokens"), Is.Null);
        Assert.That(metrics.GetMeasurements("gen_ai.client.token.usage"), Is.Null);
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration"), Has.Count.EqualTo(1));
    }

    [Test]
    public async Task CancellationDuringAsyncReadRestoresCallerActivity()
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var activities = new TestActivityListener(ActivitySourceName);
        using var metrics = new TestMeterListener(ActivitySourceName);
        using var cancellation = new CancellationTokenSource();
        using var parent = new Activity("parent").Start();
        using var stream = new CancellableReadStream();
        var client = CreateStreamingClient("", true, streamFactory: () => stream);
        var options = CreateOptions();
        options.StreamingEnabled = true;
        await using var enumerator = client.CreateResponseStreamingAsync(options, cancellation.Token).GetAsyncEnumerator();
        var pendingMove = enumerator.MoveNextAsync().AsTask();
        await stream.Started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.That(Activity.Current, Is.SameAs(parent));
        cancellation.Cancel();
        Assert.ThrowsAsync<TaskCanceledException>(async () => await pendingMove);
        await enumerator.DisposeAsync();
        Assert.That(Activity.Current, Is.SameAs(parent));
        Assert.That(activities.Activities.Single().Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration"), Has.Count.EqualTo(1));
        Assert.That(metrics.GetMeasurements("gen_ai.client.token.usage"), Is.Null);
    }

    [Test]
    public async Task TimingOnlyListenerEnablesStreamingScope()
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        var measurements = new List<double>();
        using var listener = new MeterListener
        {
            InstrumentPublished = (instrument, meterListener) =>
            {
                if ((instrument.Meter.Name == ActivitySourceName)
                    && (instrument.Name == "gen_ai.client.operation.time_to_first_chunk"))
                {
                    meterListener.EnableMeasurementEvents(instrument);
                }
            },
        };
        listener.SetMeasurementEventCallback<double>((_, value, _, _) => measurements.Add(value));
        listener.Start();
        await ConsumeStream(CreateStreamingClient(TerminalEvent(), true), true);
        Assert.That(measurements, Has.Count.EqualTo(1));
        Assert.That(measurements.Single(), Is.GreaterThanOrEqualTo(0));
    }

    [TestCase("response.audio_transcript.delta")]
    [TestCase("response.audio.transcript.delta")]
    public void OutputChunkTimingUsesProtocolArrivalTimesWithoutContent(string transcriptKind)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var metrics = new TestMeterListener(ActivitySourceName);
        var telemetry = new OpenTelemetrySource(s_endpoint);
        using var scope = telemetry.StartResponsesScope(CreateOptions());
        var now = 1.0;
        var lifecycle = scope.CreateStreamingLifecycle(() => now);
        lifecycle.OnEvent();

        foreach (var kind in new[]
        {
            "response.output_text.delta", "response.refusal.delta", "response.reasoning_text.delta",
            "response.reasoning_summary_text.delta", "response.function_call_arguments.delta",
            "response.custom_tool_call_input.delta", "response.mcp_call_arguments.delta",
            "response.code_interpreter_call_code.delta", "response.image_generation_call.partial_image",
            "response.audio.delta", transcriptKind,
        })
        {
            now += 2;
            lifecycle.OnEvent();
            var update = ModelReaderWriter.Read<StreamingResponseUpdate>(BinaryData.FromString(
                $$"""{"type":"{{kind}}","sequence_number":1,"output_index":0,"delta":"c2Vuc2l0aXZlIG91dHB1dA==","partial_image_b64":"c2Vuc2l0aXZlIG91dHB1dA=="}"""));

            if (kind == "response.audio_transcript.delta")
            {
                Assert.That(update.Kind, Is.EqualTo(StreamingResponseUpdateKind.ResponseAudioTranscriptDelta));
            }

            Assert.That(update.Kind.ToString(), Is.EqualTo(kind));
            lifecycle.OnUpdate(update);
        }
        now += 2;
        lifecycle.OnEvent();
        lifecycle.OnUpdate(ModelReaderWriter.Read<StreamingResponseUpdate>(BinaryData.FromString(
            """{"type":"response.output_item.done","sequence_number":2,"output_index":0,"item":{"type":"function_call","id":"f","call_id":"c","name":"sensitive tool","arguments":"sensitive arguments"}}""")));
        lifecycle.Complete(SseCompletionKind.EndOfStream);
        lifecycle.Complete(SseCompletionKind.Disposed);

        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.time_to_first_chunk").Single().value, Is.EqualTo(1.0));
        var intervals = metrics.GetMeasurements("gen_ai.client.operation.time_per_output_chunk");
        Assert.That(intervals, Has.Count.EqualTo(10));
        Assert.That(intervals.All(measurement => (double)measurement.value == 2.0), Is.True);
        Assert.That(intervals.SelectMany(measurement => measurement.tags.Values), Does.Not.Contain("sensitive output"));
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration"), Has.Count.EqualTo(1));
        Assert.That(metrics.GetInstrument("gen_ai.client.operation.time_to_first_chunk").Unit, Is.EqualTo("s"));
        Assert.That(((Histogram<double>)metrics.GetInstrument("gen_ai.client.operation.time_per_output_chunk"))
            .Advice.HistogramBucketBoundaries, Is.EqualTo(new[] { 0.01, 0.02, 0.04, 0.08, 0.16, 0.32, 0.64, 1.28, 2.56, 5.12, 10.24, 20.48, 40.96, 81.92 }));
    }

    [TestCase("response.audio_transcript.delta")]
    [TestCase("response.audio.transcript.delta")]
    public void TranscriptOnlyStreamRecordsOutputChunkIntervals(string kind)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var metrics = new TestMeterListener(ActivitySourceName);
        var telemetry = new OpenTelemetrySource(s_endpoint);
        using var scope = telemetry.StartResponsesScope(CreateOptions());
        var now = 1.0;
        var lifecycle = scope.CreateStreamingLifecycle(() => now);

        for (var sequence = 0; sequence < 3; sequence++)
        {
            lifecycle.OnEvent();
            var update = ModelReaderWriter.Read<StreamingResponseUpdate>(BinaryData.FromString(
                $$"""{"type":"{{kind}}","response_id":"resp_audio","sequence_number":{{sequence}},"delta":"sensitive transcript"}"""));
            Assert.That(update.Kind.ToString(), Is.EqualTo(kind));
            lifecycle.OnUpdate(update);
            now += 2;
        }

        lifecycle.Complete(SseCompletionKind.EndOfStream);

        var intervals = metrics.GetMeasurements("gen_ai.client.operation.time_per_output_chunk");
        Assert.That(intervals, Has.Count.EqualTo(2));
        Assert.That(intervals.All(measurement => (double)measurement.value == 2.0), Is.True);
        Assert.That(intervals.SelectMany(measurement => measurement.tags.Values), Does.Not.Contain("sensitive transcript"));
    }

    [TestCase("response.created")]
    [TestCase("response.in_progress")]
    [TestCase("response.queued")]
    public void StreamingTimingsIncludeModelReportedByFirstEvent(string kind)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var metrics = new TestMeterListener(ActivitySourceName);
        using var scope = new OpenTelemetrySource(s_endpoint).StartResponsesScope(CreateOptions());
        var now = 1.0;
        var lifecycle = scope.CreateStreamingLifecycle(() => now);
        lifecycle.OnEvent();
        lifecycle.OnUpdate(ModelReaderWriter.Read<StreamingResponseUpdate>(BinaryData.FromString(
            $$$"""{"type":"{{{kind}}}","sequence_number":0,"response":{"id":"resp_metadata","created_at":1,"model":"{{{ResponseModel}}}","service_tier":"default","output":[],"parallel_tool_calls":false}}""")));

        for (var sequence = 1; sequence <= 2; sequence++)
        {
            now += 2;
            lifecycle.OnEvent();
            lifecycle.OnUpdate(ModelReaderWriter.Read<StreamingResponseUpdate>(BinaryData.FromString(
                $$"""{"type":"response.output_text.delta","sequence_number":{{sequence}},"output_index":0,"content_index":0,"item_id":"message","delta":"sensitive output","logprobs":[]}""")));
        }

        lifecycle.Complete(SseCompletionKind.Disposed);

        var first = metrics.GetMeasurements("gen_ai.client.operation.time_to_first_chunk").Single();
        var output = metrics.GetMeasurements("gen_ai.client.operation.time_per_output_chunk").Single();
        Assert.That(first.value, Is.EqualTo(1.0));
        Assert.That(output.value, Is.EqualTo(2.0));

        foreach (var measurement in new[] { first, output })
        {
            Assert.That(measurement.tags["gen_ai.response.model"], Is.EqualTo(ResponseModel));
            Assert.That(measurement.tags.ContainsKey("openai.response.service_tier"), Is.False);
            Assert.That(measurement.tags.ContainsKey("openai.response.system_fingerprint"), Is.False);
        }

        var duration = metrics.GetMeasurements("gen_ai.client.operation.duration").Single();
        Assert.That(duration.tags["gen_ai.response.model"], Is.EqualTo(ResponseModel));
        Assert.That(duration.tags["openai.response.service_tier"], Is.EqualTo("default"));
    }

    [Test]
    public void StreamingTimingsDoNotApplyFutureResponseModelToEarlierMeasurements()
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var metrics = new TestMeterListener(ActivitySourceName);
        using var scope = new OpenTelemetrySource(s_endpoint).StartResponsesScope(CreateOptions());
        var now = 1.0;
        var lifecycle = scope.CreateStreamingLifecycle(() => now);
        var events = new[]
        {
            """{"type":"response.output_text.delta","sequence_number":0,"output_index":0,"delta":"first"}""",
            """{"type":"response.output_text.delta","sequence_number":1,"output_index":0,"delta":"second"}""",
            """{"type":"response.in_progress","sequence_number":2,"response":{"id":"resp_metadata","created_at":1,"model":"model-known-later","output":[],"parallel_tool_calls":false}}""",
            """{"type":"response.output_text.delta","sequence_number":3,"output_index":0,"delta":"third"}""",
            """{"type":"response.completed","sequence_number":4,"response":{"id":"resp_metadata","created_at":1,"model":"terminal-model","status":"completed","output":[],"parallel_tool_calls":false}}""",
        };

        foreach (var json in events)
        {
            lifecycle.OnEvent();
            lifecycle.OnUpdate(ModelReaderWriter.Read<StreamingResponseUpdate>(BinaryData.FromString(json)));
            now++;
        }

        lifecycle.Complete(SseCompletionKind.EndOfStream);

        var first = metrics.GetMeasurements("gen_ai.client.operation.time_to_first_chunk").Single();
        Assert.That(first.tags.ContainsKey("gen_ai.response.model"), Is.False);
        var intervals = metrics.GetMeasurements("gen_ai.client.operation.time_per_output_chunk");
        Assert.That(intervals, Has.Count.EqualTo(2));
        Assert.That(intervals[0].tags.ContainsKey("gen_ai.response.model"), Is.False);
        Assert.That(intervals[1].tags["gen_ai.response.model"], Is.EqualTo("model-known-later"));
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration").Single().tags["gen_ai.response.model"], Is.EqualTo("terminal-model"));
    }

    [Test]
    public void StreamingExceptionDurationRetainsPreviouslyReportedResponseMetadata()
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var metrics = new TestMeterListener(ActivitySourceName);
        using var scope = new OpenTelemetrySource(s_endpoint).StartResponsesScope(CreateOptions());
        var lifecycle = scope.CreateStreamingLifecycle(() => 1.0);
        lifecycle.OnEvent();
        lifecycle.OnUpdate(ModelReaderWriter.Read<StreamingResponseUpdate>(BinaryData.FromString(
            $$$"""{"type":"response.created","sequence_number":0,"response":{"id":"resp_metadata","created_at":1,"model":"{{{ResponseModel}}}","service_tier":"default","output":[],"parallel_tool_calls":false}}""")));
        lifecycle.OnException(new IOException("sensitive read failure"));
        lifecycle.Complete(SseCompletionKind.Disposed);

        var duration = metrics.GetMeasurements("gen_ai.client.operation.duration").Single();
        Assert.That(duration.tags["gen_ai.response.model"], Is.EqualTo(ResponseModel));
        Assert.That(duration.tags["openai.response.service_tier"], Is.EqualTo("default"));
        Assert.That(duration.tags["error.type"], Is.EqualTo(typeof(IOException).FullName));
        Assert.That(duration.tags.Values, Does.Not.Contain("sensitive read failure"));
        Assert.That(metrics.GetMeasurements("gen_ai.client.inference.usage.input_tokens"), Is.Null);
    }

    private static string SseEvent(string json)
    {
        using var document = JsonDocument.Parse(json);

        return $"data: {JsonSerializer.Serialize(document.RootElement)}\n\n";
    }

    private static string TerminalEvent()
        => SseEvent($$"""{"type":"response.completed","sequence_number":3,"response":{{CompletedResponseBody}}}""");

    private static ResponsesClient CreateStreamingClient(string content, bool useAsync, Action onSend = null,
        Func<Stream> streamFactory = null, int? maxRetries = null)
    {
        var transport = new MockPipelineTransport(_ =>
        {
            onSend?.Invoke();

            return new MockPipelineResponse(200)
            {
                ContentStream = streamFactory?.Invoke() ?? new MemoryStream(Encoding.UTF8.GetBytes(content)),
            };
        })
        {
            ExpectSyncPipeline = !useAsync,
        };
        var options = new ResponsesClientOptions
        {
            Endpoint = s_endpoint,
            Transport = transport,
        };

        if (maxRetries.HasValue)
        {
            options.RetryPolicy = new ClientRetryPolicy(maxRetries.Value);
        }

        return new ResponsesClient(new ApiKeyCredential("not-a-real-key"), options);
    }

    private sealed class ThrowingReadStream : MemoryStream
    {
        public override int Read(byte[] buffer, int offset, int count) => throw new IOException("sensitive read failure");

        public override int Read(Span<byte> buffer) => throw new IOException("sensitive read failure");

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            throw new IOException("sensitive read failure");
        }
    }

    private sealed class CancellableReadStream : Stream
    {
        public TaskCompletionSource<bool> Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            Started.TrySetResult(true);
            await Task.Delay(Timeout.Infinite, cancellationToken);

            return 0;
        }

        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush() => throw new NotSupportedException();

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

    private static async Task ConsumeStream(ResponsesClient client, bool useAsync, CancellationToken cancellationToken = default)
    {
        var options = CreateOptions();
        options.StreamingEnabled = true;

        if (useAsync)
        {
            await foreach (var update in client.CreateResponseStreamingAsync(options, cancellationToken))
            {
            }
        }
        else
        {
            foreach (var update in client.CreateResponseStreaming(options, cancellationToken))
            {
            }
        }
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

    private static ResponseResult CreateResponseResult(string status, string incompleteReason, string errorCode = "server_error")
        => ModelReaderWriter.Read<ResponseResult>(BinaryData.FromString(CreateResponseBody(status, incompleteReason, errorCode)));

    private static string CreateResponseBody(string status, string incompleteReason, string errorCode = "server_error")
    {
        var error = status == "failed"
            ? $"\"error\":{{\"code\":\"{errorCode}\",\"message\":\"synthetic failure\",\"param\":null,\"type\":\"server_error\"}},"
            : "\"error\":null,";
        var incompleteDetails = incompleteReason is null
            ? "\"incomplete_details\":null,"
            : $"\"incomplete_details\":{{\"reason\":\"{incompleteReason}\"}},";

        return $$"""
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
            """;
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

        var usage = GetTotalUsageMeasurements(meterListener, useLatestSemanticConventions);
        Assert.That(usage, Has.Count.EqualTo(2));

        if (useLatestSemanticConventions)
        {
            Assert.That(meterListener.GetMeasurements("gen_ai.client.token.usage"), Is.Null);
            Assert.That(usage[0].value, Is.EqualTo(InputTokens));
            Assert.That(usage[1].value, Is.EqualTo(OutputTokens));
            Assert.That(usage.All(measurement => measurement.tags["gen_ai.token.modality"].Equals("unknown")), Is.True);
            Assert.That(meterListener.GetMeasurements("gen_ai.client.inference.operation.input_tokens").Single().value, Is.EqualTo(InputTokens));
            Assert.That(meterListener.GetMeasurements("gen_ai.client.inference.operation.output_tokens").Single().value, Is.EqualTo(OutputTokens));
            Assert.That(meterListener.GetMeasurements("gen_ai.client.inference.usage.cache_read.input_tokens").Single().value, Is.EqualTo(5L));
            Assert.That(meterListener.GetMeasurements("gen_ai.client.inference.usage.reasoning.output_tokens").Single().value, Is.EqualTo(7L));
            Assert.That(meterListener.GetMeasurements("gen_ai.client.inference.usage.cache_write.input_tokens"), Is.Null);
        }
        else
        {
            Assert.That(usage.Single(measurement => measurement.tags["gen_ai.token.type"].Equals("input")).value, Is.EqualTo(InputTokens));
            Assert.That(usage.Single(measurement => measurement.tags["gen_ai.token.type"].Equals("output")).value, Is.EqualTo(OutputTokens));
        }

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

    private static List<TestMeterListener.TestMeasurement> GetTotalUsageMeasurements(TestMeterListener listener, bool latest)
        => latest
            ? listener.GetMeasurements("gen_ai.client.inference.usage.input_tokens")
                .Concat(listener.GetMeasurements("gen_ai.client.inference.usage.output_tokens")).ToList()
            : listener.GetMeasurements("gen_ai.client.token.usage");

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
            null,
            {
              "id": "cmp_synthetic",
              "type": "COMPACTION",
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
