using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Responses;
using OpenAI.Telemetry;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Telemetry;

[NonParallelizable]
[Category("Telemetry")]
[Category("Smoke")]
public class ResponseStreamTelemetryTests
{
    private const string SourceName = "OpenAI.ResponsesClient";

    [Test]
    public async Task RawBodyCompletesOnceAtItsObservedBoundary(
        [Values] bool useAsync,
        [Values("eof", "dispose", "read-error", "dispose-error", "cancel")] string ending,
        [Values] bool latest)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(latest);
        using var activities = new TestActivityListener(SourceName);
        using var metrics = new TestMeterListener(SourceName);
        using var parent = new Activity("request-parent").Start();
        var inner = new ProbeStream(ending);
        var response = new MockPipelineResponse(200) { ContentStream = inner };
        var lifecycle = new OpenTelemetrySource(new Uri("https://example.invalid"))
            .StartResponsesStreamingScope(new CreateResponseOptions { Model = "model" });
        lifecycle.OnResponse(response);
        lifecycle.Complete(SseCompletionKind.RawResponse);
        Assert.That(inner.Reads, Is.Zero);
        Assert.That(inner.Disposals, Is.Zero);
        Assert.That(activities.Activities.Count, Is.EqualTo(latest ? 0 : 1));

        using var reader = new Activity("reader-parent").Start();
        var stream = response.ContentStream;
        var buffer = new byte[10];
        async Task Read()
        {
            if (useAsync)
            {
                while (await stream.ReadAsync(buffer.AsMemory()) != 0) { }
            }
            else
            {
                while (stream.Read(buffer) != 0) { }
            }
        }
        async Task Dispose()
        {
            if (useAsync)
            {
                await stream.DisposeAsync();
            }
            else
            {
                stream.Dispose();
            }
        }
        if (ending == "read-error")
        {
            Assert.ThrowsAsync<IOException>(Read);
        }
        else if (ending == "cancel")
        {
            Assert.ThrowsAsync<OperationCanceledException>(Read);
        }
        else if (ending == "eof")
        {
            await Read();
        }
        if (ending == "dispose-error")
        {
            Assert.ThrowsAsync<IOException>(Dispose);
        }
        else
        {
            await Dispose();
        }

        Assert.That(Activity.Current, Is.SameAs(reader));
        Assert.That(activities.Activities, Has.Count.EqualTo(1));
        var activity = activities.Activities[0];
        Assert.That(activity.ParentId, Is.EqualTo(parent.Id));
        var failed = ending is "read-error" or "dispose-error" or "cancel";
        Assert.That(activity.Status, Is.EqualTo((latest && failed) ? ActivityStatusCode.Error : ActivityStatusCode.Unset));
        Assert.That(activity.StatusDescription, Is.Null);
        Assert.That(activity.GetTagItem("gen_ai.response.finish_reasons"), Is.Null);
        Assert.That(activity.GetTagItem("gen_ai.response.time_to_first_chunk"), Is.Null);
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.duration"), Is.Null);
        Assert.That(metrics.GetMeasurements("gen_ai.client.inference.usage.input_tokens"), Is.Null);
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.time_to_first_chunk"), Is.Null);
        Assert.That(metrics.GetMeasurements("gen_ai.client.operation.time_per_output_chunk"), Is.Null);
        if (latest)
        {
            Assert.That(inner.CompletionContext, Is.SameAs(activity));
            await Dispose();
            lifecycle.Complete(SseCompletionKind.RawResponse);
            Assert.That(inner.Disposals, Is.EqualTo(1));
            Assert.That(activities.Activities, Has.Count.EqualTo(1));
        }
    }

    [TestCase("array")]
    [TestCase("span")]
    [TestCase("async-array")]
    [TestCase("async-memory")]
    [TestCase("byte")]
    public async Task ZeroLengthReadsDoNotCompleteAndEofDoes(string readApi)
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var activities = new TestActivityListener(SourceName);
        using var parent = new Activity("parent").Start();
        using var response = new MockPipelineResponse(200) { ContentStream = new MemoryStream() };
        var lifecycle = new OpenTelemetrySource(new Uri("https://example.invalid"))
            .StartResponsesStreamingScope(new CreateResponseOptions { Model = "model" });
        lifecycle.OnResponse(response);
        lifecycle.Complete(SseCompletionKind.RawResponse);
        var stream = response.ContentStream;
        Assert.That(stream.Read(Array.Empty<byte>(), 0, 0), Is.Zero);
        Assert.That(stream.Read(Span<byte>.Empty), Is.Zero);
        Assert.That(await stream.ReadAsync(Array.Empty<byte>(), 0, 0), Is.Zero);
        Assert.That(await stream.ReadAsync(Memory<byte>.Empty), Is.Zero);
        Assert.That(activities.Activities, Is.Empty);

        var buffer = new byte[1];
        switch (readApi)
        {
            case "array": Assert.That(stream.Read(buffer, 0, 1), Is.Zero); break;
            case "span": Assert.That(stream.Read(buffer.AsSpan()), Is.Zero); break;
            case "async-array": Assert.That(await stream.ReadAsync(buffer, 0, 1), Is.Zero); break;
            case "async-memory": Assert.That(await stream.ReadAsync(buffer.AsMemory()), Is.Zero); break;
            case "byte": Assert.That(stream.ReadByte(), Is.EqualTo(-1)); break;
        }
        Assert.That(activities.Activities, Has.Count.EqualTo(1));
        Assert.That(Activity.Current, Is.SameAs(parent));
        response.Dispose();
        Assert.That(activities.Activities, Has.Count.EqualTo(1));
    }

    [Test]
    public void TypedConsumptionRestoresOriginalStreamBeforeParsing()
    {
        using var enabled = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using var convention = TestSemanticConventionOptIn.SetLatestGenAiSemanticConvention(true);
        using var activities = new TestActivityListener(SourceName);
        var inner = new MemoryStream();
        using var response = new MockPipelineResponse(200) { ContentStream = inner };
        var lifecycle = new OpenTelemetrySource(new Uri("https://example.invalid"))
            .StartResponsesStreamingScope(new CreateResponseOptions { Model = "model" });
        lifecycle.OnResponse(response);
        Assert.That(response.ContentStream, Is.Not.SameAs(inner));
        lifecycle.OnTypedResponse();
        Assert.That(response.ContentStream, Is.SameAs(inner));
        Assert.That(inner.ReadByte(), Is.EqualTo(-1));
        Assert.That(activities.Activities, Is.Empty);
        lifecycle.Complete(SseCompletionKind.EndOfStream);
        Assert.That(activities.Activities, Has.Count.EqualTo(1));
        Assert.That(activities.Activities[0].GetTagItem("error.type"), Is.EqualTo("incomplete_stream"));
    }

    private sealed class ProbeStream(string ending) : MemoryStream(new byte[] { 1, 2, 3 })
    {
        public int Reads { get; private set; }
        public int Disposals { get; private set; }
        public Activity CompletionContext { get; private set; }

        private void BeforeRead()
        {
            Reads++;
            if (ending == "read-error")
            {
                CompletionContext = Activity.Current;
                throw new IOException("sensitive read failure");
            }
            if (ending == "cancel")
            {
                CompletionContext = Activity.Current;
                throw new OperationCanceledException("sensitive cancellation");
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            BeforeRead();
            var read = base.Read(buffer, offset, count);
            if (read == 0)
            {
                CompletionContext = Activity.Current;
            }
            return read;
        }

        public override int Read(Span<byte> buffer)
        {
            BeforeRead();
            var read = base.Read(buffer);
            if (read == 0)
            {
                CompletionContext = Activity.Current;
            }
            return read;
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();
            return Read(buffer.Span);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposals++;
                if (ending is "dispose" or "dispose-error")
                {
                    CompletionContext = Activity.Current;
                }
                if (ending == "dispose-error")
                {
                    throw new IOException("sensitive disposal failure");
                }
            }
            base.Dispose(disposing);
        }
    }
}
