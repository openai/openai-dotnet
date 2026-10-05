using Moq;
using NUnit.Framework;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Net.ServerSentEvents;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Utility;

[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public class SseResponseOwnershipTests
{
    [TestCase(false, "cancel-before-read")]
    [TestCase(true, "cancel-before-read")]
    [TestCase(false, "missing-stream")]
    [TestCase(true, "missing-stream")]
    [TestCase(false, "cancel-after-read")]
    [TestCase(true, "cancel-after-read")]
    [TestCase(false, "complete")]
    [TestCase(true, "complete")]
    public async Task AcquiredResponseIsDisposedEvenWhenEnumerationCannotStart(bool asynchronous, string scenario)
    {
        using CancellationTokenSource cancellation = new();
        using MemoryStream content = new(Encoding.UTF8.GetBytes("data: synthetic\n\ndata: [DONE]\n\n"));
        Mock<PipelineResponse> response = new();
        response.SetupGet(value => value.Status).Returns(200);
        response.SetupGet(value => value.ContentStream).Returns(scenario == "missing-stream" ? null : content);
        int sent = 0;
        int extraDisposals = 0;

        ClientResult Send()
        {
            sent++;
            if (scenario == "cancel-before-read") cancellation.Cancel();
            return ClientResult.FromResponse(response.Object);
        }

        static IEnumerable<string> Deserialize(SseItem<byte[]> item) => [Encoding.UTF8.GetString(item.Data)];

        if (asynchronous)
        {
            AsyncSseUpdateCollection<string> collection = new(() => Task.FromResult(Send()), Deserialize, cancellation.Token);
            collection.AdditionalDisposalActions.Add(() => extraDisposals++);
            IAsyncEnumerator<string> enumerator = collection.GetAsyncEnumerator();
            try
            {
                if (scenario == "cancel-before-read")
                    Assert.ThrowsAsync<OperationCanceledException>(async () => await enumerator.MoveNextAsync());
                else if (scenario == "missing-stream")
                    Assert.ThrowsAsync<InvalidOperationException>(async () => await enumerator.MoveNextAsync());
                else
                {
                    Assert.That(await enumerator.MoveNextAsync(), Is.True);
                    Assert.That(enumerator.Current, Is.EqualTo("synthetic"));
                    if (scenario == "cancel-after-read")
                    {
                        cancellation.Cancel();
                        Assert.ThrowsAsync<OperationCanceledException>(async () => await enumerator.MoveNextAsync());
                    }
                    else Assert.That(await enumerator.MoveNextAsync(), Is.False);
                }
            }
            finally { await enumerator.DisposeAsync(); }
            await enumerator.DisposeAsync();
        }
        else
        {
            SseUpdateCollection<string> collection = new(Send, Deserialize, cancellation.Token);
            collection.AdditionalDisposalActions.Add(() => extraDisposals++);
            IEnumerator<string> enumerator = collection.GetEnumerator();
            try
            {
                if (scenario == "cancel-before-read")
                    Assert.Throws<OperationCanceledException>(() => enumerator.MoveNext());
                else if (scenario == "missing-stream")
                    Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
                else
                {
                    Assert.That(enumerator.MoveNext(), Is.True);
                    Assert.That(enumerator.Current, Is.EqualTo("synthetic"));
                    if (scenario == "cancel-after-read")
                    {
                        cancellation.Cancel();
                        Assert.Throws<OperationCanceledException>(() => enumerator.MoveNext());
                    }
                    else Assert.That(enumerator.MoveNext(), Is.False);
                }
            }
            finally { enumerator.Dispose(); }
            enumerator.Dispose();
        }

        Assert.That(sent, Is.EqualTo(1));
        Assert.That(extraDisposals, Is.EqualTo(1));
        response.Verify(value => value.Dispose(), Times.Once);
    }
}
