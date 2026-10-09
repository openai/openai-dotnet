using Microsoft.Extensions.Logging;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Responses;
using OpenAI.Telemetry;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Telemetry;

[NonParallelizable]
[Category("Telemetry")]
[Category("Smoke")]
public class ExceptionLoggingTests
{
    private static readonly Uri s_endpoint = new("https://example.invalid");

    [Test]
    public void LoggerBridgeExportsTypeOnlyExceptionWithOperationContext()
    {
        using var capture = new OtlpLogCapture();
        using var activity = new Activity("operation").Start();
        var logger = capture.Factory.CreateLogger("OpenAI.ResponsesClient.Operations");
        logger.Log(LogLevel.Warning, new EventId(1, "gen_ai.client.operation.exception"),
            new KeyValuePair<string, object>[] { new("exception.type", typeof(IOException).FullName) },
            null, static (_, _) => "GenAI operation failed.");

        var record = capture.Records.Single();
        Assert.That(record.EventName, Is.EqualTo("gen_ai.client.operation.exception"));
        Assert.That(record.Severity, Is.EqualTo(13));
        Assert.That(record.Body, Is.EqualTo("GenAI operation failed."));
        Assert.That(record.Attributes, Is.EquivalentTo(new Dictionary<string, string>
        {
            ["exception.type"] = typeof(IOException).FullName,
        }));
        Assert.That(record.TraceId, Is.EqualTo(activity.TraceId.ToHexString()));
        Assert.That(record.SpanId, Is.EqualTo(activity.SpanId.ToHexString()));
    }

    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public void LoggingOnlyDoesNotCreateAnActivity(bool responses, bool ambient)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var capture = new OtlpLogCapture();
        using var parent = ambient ? new Activity("caller").Start() : null;
        var options = new ClientLoggingOptions { LoggerFactory = capture.Factory };
        var source = responses ? new OpenTelemetrySource(s_endpoint, options) : new OpenTelemetrySource("model", s_endpoint, options);
        using var scope = responses ? source.StartResponsesScope(new CreateResponseOptions { Model = "model" })
            : source.StartChatScope(new ChatCompletionOptions());
        Assert.That(scope, Is.Not.Null);
        Assert.That(Activity.Current, Is.SameAs(parent));
        scope.RecordException(new IOException("sensitive body"));
        scope.RecordException(new IOException("second sensitive body"));
        scope.Dispose();
        Assert.That(Activity.Current, Is.SameAs(parent));
        var record = capture.Records.Single();
        Assert.That(record.TraceId, Is.EqualTo(parent?.TraceId.ToHexString()));
        Assert.That(record.SpanId, Is.EqualTo(parent?.SpanId.ToHexString()));
        AssertPrivate(record, typeof(IOException));
        Assert.DoesNotThrow(() => capture.Factory.CreateLogger("Ownership"));
    }

    [TestCase(ActivitySamplingResult.None)]
    [TestCase(ActivitySamplingResult.PropagationData)]
    [TestCase(ActivitySamplingResult.AllDataAndRecorded)]
    public void ExceptionLogUsesTheAvailableOperationContext(ActivitySamplingResult sampling)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var capture = new OtlpLogCapture();
        using var parent = new Activity("caller").Start();
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "OpenAI.ResponsesClient",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => sampling,
        };
        ActivitySource.AddActivityListener(listener);
        var source = new OpenTelemetrySource(s_endpoint, new ClientLoggingOptions { LoggerFactory = capture.Factory });
        using var scope = source.StartResponsesScope(new CreateResponseOptions { Model = "model" });
        var current = Activity.Current;
        scope.RecordException(new IOException("sensitive body"));
        scope.Dispose();
        var record = capture.Records.Single();
        Assert.That(record.TraceId, Is.EqualTo(current.TraceId.ToHexString()));
        Assert.That(record.SpanId, Is.EqualTo(current.SpanId.ToHexString()));
        Assert.That(current == parent, Is.EqualTo(sampling == ActivitySamplingResult.None));
        Assert.That(Activity.Current, Is.SameAs(parent));
    }

    [TestCase(false, true, true, LogLevel.Trace)]
    [TestCase(true, false, true, LogLevel.Trace)]
    [TestCase(true, true, false, LogLevel.Trace)]
    [TestCase(true, true, true, LogLevel.Error)]
    public void LoggingHonorsAllEnablementGates(bool instrumentation, bool latest, bool logging, LogLevel minimum)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();

        if (!instrumentation)
        {
            AppContext.SetSwitch("OpenAI.Experimental.EnableOpenTelemetry", false);
        }

        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(latest);
        using var capture = new OtlpLogCapture(minimum);
        var source = new OpenTelemetrySource(s_endpoint, new ClientLoggingOptions
        {
            LoggerFactory = capture.Factory,
            EnableLogging = logging,
        });
        using var scope = source.StartResponsesScope(new CreateResponseOptions { Model = "model" });
        Assert.That(scope, Is.Null);
        Assert.That(capture.Records, Is.Empty);
    }

    [Test]
    public void MissingFactoryDoesNotEnableLogging()
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        var source = new OpenTelemetrySource(s_endpoint, new ClientLoggingOptions { EnableLogging = true });
        Assert.That(source.StartResponsesScope(new CreateResponseOptions { Model = "model" }), Is.Null);
    }

    [Test]
    public void LoggerEnablementFailureDoesNotEscape()
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var factory = new ThrowingLoggerFactory("is-enabled");
        var source = new OpenTelemetrySource(s_endpoint, new ClientLoggingOptions { LoggerFactory = factory });

        Assert.That(source.StartResponsesScope(new CreateResponseOptions { Model = "model" }), Is.Null);
    }

    [TestCase("is-enabled")]
    [TestCase("log")]
    public void LoggerFailuresDoNotReplaceOperationException(string phase)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var activities = new TestActivityListener("OpenAI.ResponsesClient");
        using var factory = new ThrowingLoggerFactory(phase);
        var operationException = new IOException("operation failure");
        var client = new ResponsesClient(new ApiKeyCredential("not-a-key"), new ResponsesClientOptions
        {
            Endpoint = s_endpoint,
            Transport = new MockPipelineTransport(_ => throw operationException),
            RetryPolicy = new ClientRetryPolicy(0),
            ClientLoggingOptions = new ClientLoggingOptions
            {
                LoggerFactory = factory,
                EnableMessageLogging = false,
            },
        });

        Assert.That(
            () => client.CreateResponse(new CreateResponseOptions { Model = "model" }),
            Throws.Exception.SameAs(operationException));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void TopLevelClientPreservesOperationLoggingConfiguration(bool responses)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var capture = new OtlpLogCapture();
        var client = new OpenAIClient(new ApiKeyCredential("not-a-key"), new OpenAIClientOptions
        {
            Endpoint = s_endpoint,
            Transport = new MockPipelineTransport(_ => throw new IOException("sensitive transport body")),
            RetryPolicy = new ClientRetryPolicy(0),
            ClientLoggingOptions = new ClientLoggingOptions { LoggerFactory = capture.Factory, EnableMessageLogging = false },
        });

        if (responses)
        {
            Assert.Throws<IOException>(() => client.GetResponsesClient().CreateResponse(new CreateResponseOptions { Model = "model" }));
        }
        else
        {
            Assert.Throws<IOException>(() => client.GetChatClient("model").CompleteChat([new UserChatMessage("input")]));
        }

        AssertPrivate(capture.Records.Single(), typeof(IOException));
    }

    [TestCase("status")]
    [TestCase("wrapped-status")]
    [TestCase("aggregate-status")]
    [TestCase("wrapped-io")]
    [TestCase("aggregate-mixed")]
    [TestCase("cancel")]
    public void LogsEscapingApiExceptionsAndWrappersWithoutSensitiveDetails(string scenario)
    {
        using var capture = new OtlpLogCapture();
        var status = new ClientResultException("sensitive status", new MockPipelineResponse(400));
        Exception exception = scenario switch
        {
            "status" => status,
            "wrapped-status" => new ClientResultException("sensitive wrapper", new MockPipelineResponse(400), status),
            "aggregate-status" => new AggregateException(status),
            "wrapped-io" => new ClientResultException("sensitive wrapper", new MockPipelineResponse(400), new IOException("sensitive IO")),
            "aggregate-mixed" => new AggregateException(status, new IOException("sensitive IO")),
            "cancel" => new OperationCanceledException("sensitive cancellation"),
            _ => throw new ArgumentOutOfRangeException(nameof(scenario)),
        };
        OpenTelemetryExceptionLogger.Record(capture.Factory.CreateLogger("Test.Operations"), exception);
        AssertPrivate(capture.Records.Single(), exception.GetType());
    }

    private static IEnumerable<TestCaseData> ApiStatusRetryCases()
    {
        foreach (var api in new[] { "chat", "response", "stream" })
        {
            foreach (var useAsync in new[] { false, true })
            {
                foreach (var status in new[] { 429, 500 })
                {
                    foreach (var recover in new[] { false, true })
                    {
                        foreach (var latest in new[] { false, true })
                        {
                            yield return new TestCaseData(api, useAsync, status, recover, latest);
                        }
                    }
                }
            }
        }
    }

    [TestCaseSource(nameof(ApiStatusRetryCases))]
    public async Task ApiStatusRetriesLogOnlyTheFinalEscapingException(string api, bool useAsync, int status, bool recover, bool latest)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(latest);
        using var capture = new OtlpLogCapture();
        using var activities = new TestActivityListener(api == "chat" ? "OpenAI.ChatClient" : "OpenAI.ResponsesClient");
        using var parent = new Activity("status-retry-caller").Start();
        var sends = 0;
        const string responseBody = """{"id":"response-id","created_at":1,"status":"completed","model":"model","output":[],"parallel_tool_calls":false}""";
        const string chatBody = """{"id":"chat-id","created":1,"model":"model","choices":[{"index":0,"finish_reason":"stop","message":{"role":"assistant","content":"sensitive output"}}]}""";
        const string errorBody = """{"error":{"message":"sensitive API response body","type":"sensitive-error-type","code":"sensitive-error-code"}}""";
        var transport = new MockPipelineTransport(_ =>
        {
            sends++;
            Assert.That(capture.Records, Is.Empty, "A retry attempt must not produce an operation exception event.");

            if ((!recover) || (sends == 1))
            {
                return new MockPipelineResponse(status).WithContent(errorBody);
            }

            if (api == "stream")
            {
                var streamBody = "data: {\"type\":\"response.completed\",\"sequence_number\":1,\"response\":" + responseBody + "}\n\n";

                return new MockPipelineResponse(200) { ContentStream = new MemoryStream(Encoding.UTF8.GetBytes(streamBody)) };
            }

            return new MockPipelineResponse(200).WithContent(api == "chat" ? chatBody : responseBody);
        })
        {
            ExpectSyncPipeline = !useAsync,
        };
        var logging = new ClientLoggingOptions { LoggerFactory = capture.Factory, EnableMessageLogging = false };
        async Task Execute()
        {
            if (api == "chat")
            {
                var client = new ChatClient("model", new ApiKeyCredential("not-a-key"), new OpenAIClientOptions
                {
                    Endpoint = s_endpoint,
                    Transport = transport,
                    RetryPolicy = new ImmediateRetryPolicy(),
                    ClientLoggingOptions = logging,
                });

                if (useAsync)
                {
                    await client.CompleteChatAsync([new UserChatMessage("sensitive input")]);
                }
                else
                {
                    client.CompleteChat([new UserChatMessage("sensitive input")]);
                }

                return;
            }

            var responses = new ResponsesClient(new ApiKeyCredential("not-a-key"), new ResponsesClientOptions
            {
                Endpoint = s_endpoint,
                Transport = transport,
                RetryPolicy = new ImmediateRetryPolicy(),
                ClientLoggingOptions = logging,
            });
            var request = new CreateResponseOptions { Model = "model", StreamingEnabled = api == "stream" };

            if (api == "stream")
            {
                if (useAsync)
                {
                    await foreach (var update in responses.CreateResponseStreamingAsync(request))
                    {
                    }
                }
                else
                {
                    foreach (var update in responses.CreateResponseStreaming(request))
                    {
                    }
                }
            }
            else if (useAsync)
            {
                await responses.CreateResponseAsync(request);
            }
            else
            {
                responses.CreateResponse(request);
            }
        }

        if (recover)
        {
            await Execute();
        }
        else
        {
            var exception = Assert.ThrowsAsync<ClientResultException>(Execute);
            Assert.That(exception.Status, Is.EqualTo(status));
        }

        Assert.That(sends, Is.EqualTo(recover ? 2 : 3));
        Assert.That(Activity.Current, Is.SameAs(parent));
        var activity = activities.Activities.Single();
        Assert.That(activity.ParentId, Is.EqualTo(parent.Id));
        Assert.That(activity.Status, Is.EqualTo(recover ? ActivityStatusCode.Unset : ActivityStatusCode.Error));
        Assert.That(activity.GetTagItem("error.type"), Is.EqualTo(recover ? null : status.ToString()));

        if ((latest) && (!recover))
        {
            var record = capture.Records.Single();
            AssertPrivate(record, typeof(ClientResultException));
            Assert.That(record.TraceId, Is.EqualTo(activity.TraceId.ToHexString()));
            Assert.That(record.SpanId, Is.EqualTo(activity.SpanId.ToHexString()));
        }
        else
        {
            Assert.That(capture.Records, Is.Empty);
        }
    }

    private sealed class ImmediateRetryPolicy : ClientRetryPolicy
    {
        public ImmediateRetryPolicy() : base(maxRetries: 2) { }

        protected override TimeSpan GetNextDelay(PipelineMessage message, int tryCount) => TimeSpan.Zero;
    }

    [TestCase("chat", false, false)]
    [TestCase("chat", true, false)]
    [TestCase("chat", false, true)]
    [TestCase("chat", true, true)]
    [TestCase("response", false, false)]
    [TestCase("response", true, false)]
    [TestCase("response", false, true)]
    [TestCase("response", true, true)]
    [TestCase("stream", false, false)]
    [TestCase("stream", true, false)]
    [TestCase("stream", false, true)]
    [TestCase("stream", true, true)]
    public void ClientFailuresLogOnceAfterRetries(string api, bool useAsync, bool suppliedPipeline)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var capture = new OtlpLogCapture();
        using var parent = new Activity("caller").Start();
        var sends = 0;
        var transport = new MockPipelineTransport(_ =>
        {
            sends++;

            throw new IOException("sensitive transport body");
        })
        { ExpectSyncPipeline = !useAsync };
        var logging = new ClientLoggingOptions { LoggerFactory = capture.Factory, EnableMessageLogging = false };

        if (api == "chat")
        {
            var options = new OpenAIClientOptions
            {
                Endpoint = s_endpoint,
                Transport = transport,
                RetryPolicy = new ClientRetryPolicy(1),
                ClientLoggingOptions = logging,
            };
            var client = suppliedPipeline
                ? new SuppliedChatClient(ClientPipeline.Create(options), options)
                : new ChatClient("model", new ApiKeyCredential("not-a-key"), options);

            if (useAsync)
            {
                Assert.ThrowsAsync<AggregateException>(async () => await client.CompleteChatAsync([new UserChatMessage("input")]));
            }
            else
            {
                Assert.Throws<AggregateException>(() => client.CompleteChat([new UserChatMessage("input")]));
            }
        }
        else
        {
            var options = new ResponsesClientOptions
            {
                Endpoint = s_endpoint,
                Transport = transport,
                RetryPolicy = new ClientRetryPolicy(1),
                ClientLoggingOptions = logging,
            };
            var client = suppliedPipeline
                ? new SuppliedResponsesClient(ClientPipeline.Create(options), options)
                : new ResponsesClient(new ApiKeyCredential("not-a-key"), options);
            var request = new CreateResponseOptions { Model = "model", StreamingEnabled = api == "stream" };

            if (api == "stream")
            {
                if (useAsync)
                {
                    Assert.ThrowsAsync<AggregateException>(async () =>
                    {
                        await foreach (var update in client.CreateResponseStreamingAsync(request))
                        {
                        }
                    });
                }
                else
                {
                    Assert.Throws<AggregateException>(() => client.CreateResponseStreaming(request).ToList());
                }
            }
            else if (useAsync)
            {
                Assert.ThrowsAsync<AggregateException>(async () => await client.CreateResponseAsync(request));
            }
            else
            {
                Assert.Throws<AggregateException>(() => client.CreateResponse(request));
            }
        }

        Assert.That(sends, Is.EqualTo(2));
        AssertPrivate(capture.Records.Single(), typeof(AggregateException));
        Assert.That(Activity.Current, Is.SameAs(parent));
    }

    private sealed class SuppliedChatClient(ClientPipeline pipeline, OpenAIClientOptions options)
        : ChatClient(pipeline, "model", options);

    private sealed class SuppliedResponsesClient(ClientPipeline pipeline, ResponsesClientOptions options)
        : ResponsesClient(pipeline, options);

    [TestCase(false, false, false)]
    [TestCase(false, false, true)]
    [TestCase(false, true, true)]
    [TestCase(true, false, false)]
    [TestCase(true, false, true)]
    [TestCase(true, true, true)]
    public async Task StreamLogsOnlyObservedReadExceptions(bool useAsync, bool raw, bool failRead)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var capture = new OtlpLogCapture();
        using var parent = new Activity("caller").Start();
        var activities = new List<Activity>();
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "OpenAI.ResponsesClient",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activities.Add,
        };
        ActivitySource.AddActivityListener(listener);
        using var response = new MockPipelineResponse(200) { ContentStream = failRead ? new ThrowingReadStream() : new MemoryStream() };
        var client = new ResponsesClient(new ApiKeyCredential("not-a-key"), new ResponsesClientOptions
        {
            Endpoint = s_endpoint,
            Transport = new MockPipelineTransport(_ => response) { ExpectSyncPipeline = !useAsync },
            ClientLoggingOptions = new ClientLoggingOptions { LoggerFactory = capture.Factory, EnableMessageLogging = false },
        });
        var options = new CreateResponseOptions { Model = "model", StreamingEnabled = true };

        if (raw)
        {
            if (useAsync)
            {
                await foreach (var page in client.CreateResponseStreamingAsync(options).GetRawPagesAsync())
                {
                }
            }
            else
            {
                client.CreateResponseStreaming(options).GetRawPages().ToList();
            }
        }
        else if (useAsync)
        {
            async Task Consume()
            {
                await foreach (var update in client.CreateResponseStreamingAsync(options))
                {
                }
            }

            if (failRead)
            {
                Assert.ThrowsAsync<IOException>(Consume);
            }
            else
            {
                await Consume();
            }
        }
        else if (failRead)
        {
            Assert.Throws<IOException>(() => client.CreateResponseStreaming(options).ToList());
        }
        else
        {
            client.CreateResponseStreaming(options).ToList();
        }

        Assert.That(Activity.Current, Is.SameAs(parent));

        if ((!raw) && (failRead))
        {
            var record = capture.Records.Single();
            AssertPrivate(record, typeof(IOException));
            Assert.That(record.TraceId, Is.EqualTo(activities.Single().TraceId.ToHexString()));
            Assert.That(record.SpanId, Is.EqualTo(activities.Single().SpanId.ToHexString()));
        }
        else
        {
            Assert.That(capture.Records, Is.Empty);
        }
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task RetainedRawBodyReadFailureExportsOneExceptionWithSavedOperationContext(bool useAsync)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var capture = new OtlpLogCapture();
        using var activities = new TestActivityListener("OpenAI.ResponsesClient");
        using var metrics = new TestMeterListener("OpenAI.ResponsesClient");
        using var parent = new Activity("raw-body-caller").Start();
        using var response = new MockPipelineResponse(200) { ContentStream = new ThrowingReadStream() };
        var client = new ResponsesClient(new ApiKeyCredential("not-a-key"), new ResponsesClientOptions
        {
            Endpoint = s_endpoint,
            Transport = new MockPipelineTransport(_ => response) { ExpectSyncPipeline = !useAsync },
            ClientLoggingOptions = new ClientLoggingOptions { LoggerFactory = capture.Factory, EnableMessageLogging = false },
        });
        var options = new CreateResponseOptions { Model = "model", StreamingEnabled = true };
        PipelineResponse retained = null;

        if (useAsync)
        {
            await foreach (var page in client.CreateResponseStreamingAsync(options).GetRawPagesAsync())
            {
                retained = page.GetRawResponse();
                Assert.That(Activity.Current, Is.SameAs(parent));
            }
        }
        else
        {
            foreach (var page in client.CreateResponseStreaming(options).GetRawPages())
            {
                retained = page.GetRawResponse();
                Assert.That(Activity.Current, Is.SameAs(parent));
            }
        }

        Assert.That(retained, Is.SameAs(response));
        Assert.That(Activity.Current, Is.SameAs(parent));
        Assert.That(capture.Records, Is.Empty);

        var buffer = new byte[1];

        if (useAsync)
        {
            Assert.ThrowsAsync<IOException>(async () => await retained.ContentStream.ReadExactlyAsync(buffer.AsMemory()));
        }
        else
        {
            Assert.Throws<IOException>(() => retained.ContentStream.ReadExactly(buffer, 0, buffer.Length));
        }

        Assert.That(Activity.Current, Is.SameAs(parent));
        var activity = activities.Activities.Single();
        var record = capture.Records.Single();
        AssertPrivate(record, typeof(IOException));
        Assert.That(record.TraceId, Is.EqualTo(activity.TraceId.ToHexString()));
        Assert.That(record.SpanId, Is.EqualTo(activity.SpanId.ToHexString()));
        Assert.That(activity.ParentId, Is.EqualTo(parent.Id));
        var durations = metrics.GetMeasurements("gen_ai.client.operation.duration");
        Assert.That(durations, Has.Count.EqualTo(1));
        Assert.That(durations[0].tags["error.type"], Is.EqualTo(activity.GetTagItem("error.type")));
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.time_to_first_chunk"), Is.Null);

        retained.Dispose();
        Assert.That(capture.Records, Has.Count.EqualTo(1));
        Assert.That(activities.Activities, Has.Count.EqualTo(1));
        Assert.That(Activity.Current, Is.SameAs(parent));
    }

    private sealed class ThrowingReadStream : MemoryStream
    {
        public override int Read(byte[] buffer, int offset, int count) => throw new IOException("sensitive read");

        public override int Read(Span<byte> buffer) => throw new IOException("sensitive read");

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => Task.FromException<int>(new IOException("sensitive read"));

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            => ValueTask.FromException<int>(new IOException("sensitive read"));
    }

    private sealed class ThrowingLoggerFactory(string phase) : ILoggerFactory
    {
        public void AddProvider(ILoggerProvider provider)
        {
        }

        public ILogger CreateLogger(string categoryName) => new ThrowingLogger(phase);

        public void Dispose()
        {
        }

        private sealed class ThrowingLogger(string phase) : ILogger
        {
            public IDisposable BeginScope<TState>(TState state) => null;

            public bool IsEnabled(LogLevel logLevel)
                => phase == "is-enabled" ? throw new InvalidOperationException("logger enablement failure") : true;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception exception,
                Func<TState, Exception, string> formatter)
                => throw new InvalidOperationException("logger emission failure");
        }
    }

    private static void AssertPrivate(ExportedLog record, Type exceptionType)
    {
        Assert.That(record.Attributes, Is.EquivalentTo(new Dictionary<string, string> { ["exception.type"] = exceptionType.FullName }));
        Assert.That(record.EventName, Is.EqualTo("gen_ai.client.operation.exception"));
        Assert.That(record.Severity, Is.EqualTo(13));
        Assert.That(record.Body, Is.EqualTo("GenAI operation failed."));
    }

    private sealed class OtlpLogCapture : IDisposable
    {
        private readonly CaptureHandler _handler = new();

        public OtlpLogCapture(LogLevel minimum = LogLevel.Trace)
        {
            Factory = LoggerFactory.Create(builder => builder
                .SetMinimumLevel(minimum)
                .AddFilter((category, level) => (category?.EndsWith(".Operations", StringComparison.Ordinal) == true) && (level >= minimum))
                .AddOpenTelemetry(options => options.AddOtlpExporter((exporter, processor) =>
                {
                    exporter.Protocol = OtlpExportProtocol.HttpProtobuf;
                    exporter.Endpoint = new Uri("https://example.invalid/v1/logs");
                    exporter.HttpClientFactory = () => new HttpClient(_handler, disposeHandler: false);
                    processor.ExportProcessorType = ExportProcessorType.Simple;
                })));
        }

        public ILoggerFactory Factory { get; }

        public IReadOnlyList<ExportedLog> Records => _handler.Payloads
            .SelectMany(payload => Fields(payload).Where(item => item.Number == 1))
            .SelectMany(resource => Fields(resource.Data).Where(item => item.Number == 2))
            .SelectMany(scope => Fields(scope.Data).Where(item => item.Number == 2))
            .Select(record => new ExportedLog(record.Data))
            .ToArray();

        public void Dispose()
        {
            Factory.Dispose();
            _handler.Dispose();
        }
    }

    private sealed class ExportedLog
    {
        public ExportedLog(byte[] data)
        {
            var fields = Fields(data);
            EventName = Text(fields.Single(field => field.Number == 12).Data);
            Severity = fields.Single(field => field.Number == 2).Value;
            Body = Text(Fields(fields.Single(field => field.Number == 5).Data).Single(field => field.Number == 1).Data);
            TraceId = Hex(fields.SingleOrDefault(field => field.Number == 9)?.Data);
            SpanId = Hex(fields.SingleOrDefault(field => field.Number == 10)?.Data);
            Attributes = fields.Where(field => field.Number == 6)
                .Select(field => Fields(field.Data))
                .ToDictionary(attribute => Text(attribute.Single(field => field.Number == 1).Data),
                    attribute => Text(Fields(attribute.Single(field => field.Number == 2).Data).Single(field => field.Number == 1).Data));
        }

        public string EventName { get; }

        public ulong Severity { get; }

        public string Body { get; }

        public string TraceId { get; }

        public string SpanId { get; }

        public Dictionary<string, string> Attributes { get; }

        private static string Text(byte[] bytes) => Encoding.UTF8.GetString(bytes);

        private static string Hex(byte[] bytes) => bytes is null ? null : Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private sealed class CaptureHandler : HttpMessageHandler
    {
        public List<byte[]> Payloads { get; } = [];

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Payloads.Add(request.Content.ReadAsByteArrayAsync(cancellationToken).GetAwaiter().GetResult());

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent([]) };
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Payloads.Add(await request.Content.ReadAsByteArrayAsync(cancellationToken));

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent([]) };
        }
    }

    private sealed record ProtoField(int Number, ulong Value, byte[] Data);

    // Decode the OTLP envelope so the event-name assertion covers wire export, not just ILogger.EventId.
    private static List<ProtoField> Fields(ReadOnlySpan<byte> bytes)
    {
        var fields = new List<ProtoField>();

        while (!bytes.IsEmpty)
        {
            var tag = ReadVarint(ref bytes);
            var number = checked((int)(tag >> 3));

            switch (tag & 7)
            {
                case 0:
                    fields.Add(new ProtoField(number, ReadVarint(ref bytes), null));
                    break;
                case 1:
                    bytes = bytes.Slice(8);
                    break;
                case 2:
                    var length = checked((int)ReadVarint(ref bytes));
                    fields.Add(new ProtoField(number, 0, bytes.Slice(0, length).ToArray()));
                    bytes = bytes.Slice(length);
                    break;
                case 5:
                    bytes = bytes.Slice(4);
                    break;
                default:
                    throw new InvalidDataException($"Unexpected protobuf wire type {tag & 7}.");
            }
        }

        return fields;
    }

    private static ulong ReadVarint(ref ReadOnlySpan<byte> bytes)
    {
        var value = 0UL;

        for (var shift = 0; shift < 64; shift += 7)
        {
            var next = bytes[0];
            bytes = bytes.Slice(1);
            value |= (ulong)(next & 0x7f) << shift;

            if ((next & 0x80) == 0)
            {
                return value;
            }
        }

        throw new InvalidDataException("Invalid protobuf varint.");
    }
}
