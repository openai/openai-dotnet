using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Net.ServerSentEvents;
using System.Text;
using System.Threading.Tasks;

namespace OpenAI.Tests.Utility;

[Parallelizable(ParallelScope.All)]
public class SseStreamingResultTests
{
    [Test]
    public void ResponseIsReleasedWhenReleasingTheRequestResourceFails()
    {
        MockPipelineResponse response = CreateResponse();
        RecordingLifecycle lifecycle = new();

        Assert.That(
            async () => await SseStreamingResult.CreateAsync<string>(
                () => Task.FromResult(ClientResult.FromResponse(response)),
                ReadData,
                default,
                () => lifecycle,
                requestResource: new ThrowingDisposable()),
            Throws.InvalidOperationException.With.Message.EqualTo(ThrowingDisposable.Message));
        Assert.That(response.IsDisposed, Is.True);
        Assert.That(lifecycle.Exception?.Message, Is.EqualTo(ThrowingDisposable.Message));
    }

    [Test]
    public void ResponseIsReleasedWhenRecordingAFailureFails()
    {
        MockPipelineResponse response = CreateResponse();

        Assert.That(
            async () => await SseStreamingResult.CreateAsync<string>(
                () => Task.FromResult(ClientResult.FromResponse(response)),
                ReadData,
                default,
                () => new ThrowingLifecycle(),
                (_, _, _) => throw new InvalidOperationException("The result could not be created.")),
            Throws.Exception);
        Assert.That(response.IsDisposed, Is.True);
    }

    private static MockPipelineResponse CreateResponse()
        => new(200, "OK") { ContentStream = new TrackingStream(Encoding.UTF8.GetBytes("data: update\n\n")) };

    private static IEnumerable<string> ReadData(SseItem<byte[]> item) => [Encoding.UTF8.GetString(item.Data)];

    private sealed class ThrowingDisposable : IDisposable
    {
        public const string Message = "The request resource could not be released.";

        public void Dispose() => throw new InvalidOperationException(Message);
    }

    private class RecordingLifecycle : SseLifecycle<string>
    {
        public Exception Exception { get; private set; }

        public override IDisposable Enter() => null;

        public override void OnEvent()
        {
        }

        public override void OnUpdate(string update)
        {
        }

        public override void OnException(Exception exception) => Exception = exception;

        public override void Complete(SseCompletionKind completionKind)
        {
        }
    }

    private sealed class ThrowingLifecycle : RecordingLifecycle
    {
        public override void OnException(Exception exception) => throw new NotSupportedException("The failure could not be recorded.");
    }
}
