using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Telemetry;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static OpenAI.Tests.Telemetry.TestMeterListener;
using static OpenAI.Tests.Telemetry.TestActivityListener;

namespace OpenAI.Tests.Telemetry;

[TestFixture]
[NonParallelizable]
[Category("Telemetry")]
[Category("Smoke")]
public class ChatTelemetryTests
{
    private const string RequestModel = "requestModel";
    private const string Host = "host";
    private const int Port = 42;
    private static readonly string Endpoint = $"https://{Host}:{Port}/path";
    private const string CompletionId = "chatcmpl-9fG9OILMJnKZARXDwxoCnLcvDsDDX";
    private const string CompletionContent = "hello world";
    private const string ResponseModel = "responseModel";
    private const string FinishReason = "stop";
    private const int PromptTokens = 2;
    private const int CompletionTokens = 42;

    [Test]
    public void AllTelemetryOff()
    {
        var telemetry = new OpenTelemetrySource(RequestModel, new Uri(Endpoint));
        Assert.IsNull(telemetry.StartChatScope(new ChatCompletionOptions()));
        Assert.IsNull(Activity.Current);
    }

    [Test]
    public void SwitchOffAllTelemetryOn()
    {
        using var activityListener = new TestActivityListener("OpenAI.ChatClient");
        using var meterListener = new TestMeterListener("OpenAI.ChatClient");
        var telemetry = new OpenTelemetrySource(RequestModel, new Uri(Endpoint));
        Assert.IsNull(telemetry.StartChatScope(new ChatCompletionOptions()));
        Assert.IsNull(Activity.Current);
    }

    [Test]
    public void MetricsOnTracingOff()
    {
        using var _ = TestAppContextSwitchHelper.EnableOpenTelemetry();

        var telemetry = new OpenTelemetrySource(RequestModel, new Uri(Endpoint));

        using var meterListener = new TestMeterListener("OpenAI.ChatClient");

        var elapsedMax = Stopwatch.StartNew();
        using var scope = telemetry.StartChatScope(new ChatCompletionOptions());
        var elapsedMin = Stopwatch.StartNew();

        Assert.Null(Activity.Current);
        Assert.NotNull(scope);

        // so we have some duration to measure
        Thread.Sleep(20);

        elapsedMin.Stop();

        var response = CreateChatCompletion();
        scope.RecordChatCompletion(response);
        scope.Dispose();

        ValidateDuration(meterListener, response, elapsedMin.Elapsed, elapsedMax.Elapsed);
        ValidateUsage(meterListener, response, PromptTokens, CompletionTokens);
    }

    [Test]
    public void MetricsOnTracingOffException()
    {
        using var _ = TestAppContextSwitchHelper.EnableOpenTelemetry();

        var telemetry = new OpenTelemetrySource(RequestModel, new Uri(Endpoint));
        using var meterListener = new TestMeterListener("OpenAI.ChatClient");

        using (var scope = telemetry.StartChatScope(new ChatCompletionOptions()))
        {
            scope.RecordException(new TaskCanceledException());
        }

        ValidateDuration(meterListener, null, TimeSpan.MinValue, TimeSpan.MaxValue);
        Assert.IsNull(meterListener.GetMeasurements("gen_ai.client.token.usage"));
    }

    [Test]
    public void TracingOnMetricsOff()
    {
        using var _ = TestAppContextSwitchHelper.EnableOpenTelemetry();

        var telemetry = new OpenTelemetrySource(RequestModel, new Uri(Endpoint));
        using var listener = new TestActivityListener("OpenAI.ChatClient");

        var chatCompletion = CreateChatCompletion();

        Activity activity = null;
        using (var scope = telemetry.StartChatScope(new ChatCompletionOptions()))
        {
            activity = Activity.Current;
            Assert.IsNull(activity.GetTagItem("gen_ai.request.temperature"));
            Assert.IsNull(activity.GetTagItem("gen_ai.request.top_p"));
            Assert.IsNull(activity.GetTagItem("gen_ai.request.max_tokens"));

            Assert.NotNull(scope);

            scope.RecordChatCompletion(chatCompletion);
        }

        Assert.Null(Activity.Current);
        Assert.That(listener.Activities.Count, Is.EqualTo(1));

        ValidateChatActivity(listener.Activities.Single(), chatCompletion, RequestModel, Host, Port);
    }

    [Test]
    public void ChatTracingAllAttributes()
    {
        using var _ = TestAppContextSwitchHelper.EnableOpenTelemetry();
        var telemetry = new OpenTelemetrySource(RequestModel, new Uri(Endpoint));
        using var listener = new TestActivityListener("OpenAI.ChatClient");
        var options = new ChatCompletionOptions()
        {
            Temperature = 0.42f,
            MaxOutputTokenCount = 200,
            TopP = 0.9f
        };
        SetMessages(options, new UserChatMessage("hello"));

        var chatCompletion = CreateChatCompletion();

        using (var scope = telemetry.StartChatScope(options))
        {
            Assert.That((float)Activity.Current.GetTagItem("gen_ai.request.temperature"), Is.EqualTo(options.Temperature.Value).Within(0.01));
            Assert.That((float)Activity.Current.GetTagItem("gen_ai.request.top_p"), Is.EqualTo(options.TopP.Value).Within(0.01));
            Assert.That(Activity.Current.GetTagItem("gen_ai.request.max_tokens"), Is.EqualTo(options.MaxOutputTokenCount.Value));
            scope.RecordChatCompletion(chatCompletion);
        }
        Assert.Null(Activity.Current);

        ValidateChatActivity(listener.Activities.Single(), chatCompletion, RequestModel, Host, Port);
    }

    [Test]
    public void ChatTracingException()
    {
        using var _ = TestAppContextSwitchHelper.EnableOpenTelemetry();

        var telemetry = new OpenTelemetrySource(RequestModel, new Uri(Endpoint));
        using var listener = new TestActivityListener("OpenAI.ChatClient");

        var error = new SocketException(42, "test error");
        using (var scope = telemetry.StartChatScope(new ChatCompletionOptions()))
        {
            scope.RecordException(error);
        }

        Assert.Null(Activity.Current);

        ValidateChatActivity(listener.Activities.Single(), error, RequestModel, Host, Port);
    }

    [Test]
    [Ignore("Temporarily disabling for reliability.")]
    public async Task ChatTracingAndMetricsMultiple()
    {
        using var _ = TestAppContextSwitchHelper.EnableOpenTelemetry();
        var source = new OpenTelemetrySource(RequestModel, new Uri(Endpoint));

        using var activityListener = new TestActivityListener("OpenAI.ChatClient");
        using var meterListener = new TestMeterListener("OpenAI.ChatClient");

        var options = new ChatCompletionOptions();

        var tasks = new Task[5];
        int numberOfSuccessfulResponses = 3;
        int totalPromptTokens = 0, totalCompletionTokens = 0;
        for (int i = 0; i < tasks.Length; i++)
        {
            int t = i;
            // don't let Activity.Current escape the scope
            tasks[i] = Task.Run(async () =>
            {
                using var scope = source.StartChatScope(options);
                await Task.Delay(10);
                if (t < numberOfSuccessfulResponses)
                {
                    var promptTokens = Random.Shared.Next(100);
                    var completionTokens = Random.Shared.Next(100);

                    var completion = CreateChatCompletion(promptTokens, completionTokens);
                    totalPromptTokens += promptTokens;
                    totalCompletionTokens += completionTokens;
                    scope.RecordChatCompletion(completion);
                }
                else
                {
                    scope.RecordException(new TaskCanceledException());
                }
            });
        }

        await Task.WhenAll(tasks);

        Assert.That(activityListener.Activities.Count, Is.EqualTo(tasks.Length));

        var durations = meterListener.GetMeasurements("gen_ai.client.operation.duration");
        Assert.That(durations.Count, Is.EqualTo(tasks.Length));
        Assert.That(durations.Count(d => !d.tags.ContainsKey("error.type")), Is.EqualTo(numberOfSuccessfulResponses));

        var usages = meterListener.GetMeasurements("gen_ai.client.token.usage");
        // we don't report usage if there was no response
        Assert.That(usages.Count, Is.EqualTo(numberOfSuccessfulResponses * 2));
        Assert.IsEmpty(usages.Where(u => u.tags.ContainsKey("error.type")));

        Assert.That(usages
            .Where(u => u.tags.Contains(new KeyValuePair<string, object>("gen_ai.token.type", "input")))
            .Sum(u => (long)u.value), Is.EqualTo(totalPromptTokens));
        Assert.That(usages
            .Where(u => u.tags.Contains(new KeyValuePair<string, object>("gen_ai.token.type", "output")))
            .Sum(u => (long)u.value), Is.EqualTo(totalCompletionTokens));
    }

    private void SetMessages(ChatCompletionOptions options, params ChatMessage[] messages)
    {
        var messagesProperty = typeof(ChatCompletionOptions).GetProperty("Messages", BindingFlags.Instance | BindingFlags.NonPublic);
        messagesProperty.SetValue(options, messages.ToList());
    }

    private void ValidateDuration(TestMeterListener listener, ChatCompletion response, TimeSpan durationMin, TimeSpan durationMax)
    {
        var duration = listener.GetInstrument("gen_ai.client.operation.duration");
        Assert.IsNotNull(duration);
        Assert.IsInstanceOf<Histogram<double>>(duration);

        var measurements = listener.GetMeasurements("gen_ai.client.operation.duration");
        Assert.IsNotNull(measurements);
        Assert.That(measurements.Count, Is.EqualTo(1));

        var measurement = measurements[0];
        Assert.IsInstanceOf<double>(measurement.value);
        Assert.GreaterOrEqual((double)measurement.value, durationMin.TotalSeconds);
        Assert.LessOrEqual((double)measurement.value, durationMax.TotalSeconds);

        ValidateChatMetricTags(measurement, response, RequestModel, Host, Port);
    }

    private void ValidateUsage(TestMeterListener listener, ChatCompletion response, int inputTokens, int outputTokens)
    {
        var usage = listener.GetInstrument("gen_ai.client.token.usage");
        Assert.IsNotNull(usage);
        Assert.IsInstanceOf<Histogram<long>>(usage);

        var measurements = listener.GetMeasurements("gen_ai.client.token.usage");
        Assert.IsNotNull(measurements);
        Assert.That(measurements.Count, Is.EqualTo(2));

        foreach (var measurement in measurements)
        {
            Assert.IsInstanceOf<long>(measurement.value);
            ValidateChatMetricTags(measurement, response, RequestModel, Host, Port);
        }

        Assert.True(measurements[0].tags.TryGetValue("gen_ai.token.type", out var type));
        Assert.IsInstanceOf<string>(type);

        TestMeasurement input = (type is "input") ? measurements[0] : measurements[1];
        TestMeasurement output = (type is "input") ? measurements[1] : measurements[0];

        Assert.That(input.value, Is.EqualTo(inputTokens));
        Assert.That(output.value, Is.EqualTo(outputTokens));
    }

    private static ChatCompletion CreateChatCompletion(int promptTokens = PromptTokens, int completionTokens = CompletionTokens)
    {
        var completion = BinaryData.FromString(
            $$"""
            {
              "id": "{{CompletionId}}",
              "created": 1719621282,
              "choices": [
                {
                  "message": {
                    "role": "assistant",
                    "content": "{{CompletionContent}}"
                  },
                  "logprobs": null,
                  "index": 0,
                  "finish_reason": "{{FinishReason}}"
                }
              ],
              "model": "{{ResponseModel}}",
              "system_fingerprint": "fp_7ec89fabc6",
              "usage": {
                "completion_tokens": {{completionTokens}},
                "prompt_tokens": {{promptTokens}},
                "total_tokens": 42
              }
            }
            """);

        return ModelReaderWriter.Read<ChatCompletion>(completion);
    }
}
