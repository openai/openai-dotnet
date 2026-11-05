using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Responses.WebSockets;

[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public class MessageReceivedSubscriberTests
{
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

    private static TaskCompletionSource<bool> CreateSignal()
        => new(TaskCreationOptions.RunContinuationsAsynchronously);

    [Test]
    public async Task AsyncDisposalWaitsForHandlerAndBlockedProducers()
    {
        TaskCompletionSource<bool> entered = CreateSignal();
        TaskCompletionSource<bool> release = CreateSignal();
        using CancellationTokenSource timeout = new(TestTimeout);
        await using MessageReceivedSubscriber<int> subscriber = new(async _ =>
        {
            entered.TrySetResult(true);
            await release.Task.WaitAsync(timeout.Token);
        });
        await subscriber.EnqueueAsync(-1, timeout.Token);
        await entered.Task.WaitAsync(timeout.Token);
        for (int item = 0; item < 100; item++)
        {
            await subscriber.EnqueueAsync(item, timeout.Token);
        }
        Task[] blocked = Enumerable.Range(100, 32)
            .Select(item => subscriber.EnqueueAsync(item, timeout.Token)).ToArray();
        Task disposal = subscriber.DisposeAsync().AsTask();

        Assert.That(disposal.IsCompleted, Is.False);
        foreach (Task enqueue in blocked)
        {
            await Assert.CatchAsync<OperationCanceledException>(async () => await enqueue.WaitAsync(timeout.Token));
        }
        release.SetResult(true);
        await disposal.WaitAsync(timeout.Token);
        await subscriber.DisposeAsync();
        subscriber.Dispose();

        Assert.That(subscriber.Completion.IsCompletedSuccessfully, Is.True);
        await Assert.ThrowsAsync<ObjectDisposedException>(() => subscriber.EnqueueAsync(200));
    }

    [Test]
    public async Task ConcurrentDisposalAndEnqueuesDoNotRaceResourceCleanup()
    {
        using CancellationTokenSource timeout = new(TestTimeout);
        for (int iteration = 0; iteration < 50; iteration++)
        {
            await using MessageReceivedSubscriber<int> subscriber = new(_ => Task.CompletedTask);
            TaskCompletionSource<bool> start = CreateSignal();
            Task[] producers = Enumerable.Range(0, 8).Select(item => Task.Run(async () =>
            {
                await start.Task;
                try
                {
                    await subscriber.EnqueueAsync(item, timeout.Token);
                }
                catch (ObjectDisposedException exception)
                {
                    Assert.That(exception.ObjectName, Is.EqualTo("MessageReceivedSubscriber"));
                }
                catch (OperationCanceledException)
                {
                    Assert.That(timeout.IsCancellationRequested, Is.False);
                }
            })).ToArray();
            Task[] disposals = Enumerable.Range(0, 4).Select(_ => Task.Run(async () =>
            {
                await start.Task;
                subscriber.Dispose();
                await subscriber.DisposeAsync();
            })).ToArray();

            start.SetResult(true);
            await Task.WhenAll(producers.Concat(disposals)).WaitAsync(timeout.Token);
            Assert.That(subscriber.Completion.IsCompletedSuccessfully, Is.True);
        }
    }

    [Test]
    public async Task AsyncDisposalPropagatesHandlerFailure()
    {
        TaskCompletionSource<bool> entered = CreateSignal();
        TaskCompletionSource<bool> release = CreateSignal();
        using CancellationTokenSource timeout = new(TestTimeout);
        InvalidOperationException failure = new("Handler failed during shutdown.");
        using MessageReceivedSubscriber<int> subscriber = new(async _ =>
        {
            entered.SetResult(true);
            await release.Task.WaitAsync(timeout.Token);
            throw failure;
        });
        await subscriber.EnqueueAsync(1, timeout.Token);
        await entered.Task.WaitAsync(timeout.Token);
        Task disposal = subscriber.DisposeAsync().AsTask();
        release.SetResult(true);

        InvalidOperationException actual = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await disposal.WaitAsync(timeout.Token));

        Assert.That(actual, Is.SameAs(failure));
        await Assert.ThrowsAsync<ObjectDisposedException>(() => subscriber.EnqueueAsync(2));
        subscriber.Dispose();
    }

    [Test]
    public void NullHandlerIsRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new MessageReceivedSubscriber<int>(null));
    }

    [Test]
    public async Task IdleSubscriberStopsOnRepeatedDisposal()
    {
        int calls = 0;
        MessageReceivedSubscriber<int> subscriber = new(_ =>
        {
            Interlocked.Increment(ref calls);
            return Task.CompletedTask;
        });

        Assert.That(subscriber.Completion.IsCompleted, Is.False);
        subscriber.Dispose();
        subscriber.Dispose();
        await subscriber.Completion.WaitAsync(TestTimeout);

        Assert.That(calls, Is.Zero);
        await Assert.ThrowsAsync<ObjectDisposedException>(() => subscriber.EnqueueAsync(1));
    }

    [Test]
    public async Task MessagesAreHandledSequentiallyInFifoOrder()
    {
        TaskCompletionSource<bool> entered = CreateSignal();
        TaskCompletionSource<bool> release = CreateSignal();
        TaskCompletionSource<bool> finished = CreateSignal();
        ConcurrentQueue<int> received = new();
        using CancellationTokenSource timeout = new(TestTimeout);
        using MessageReceivedSubscriber<int> subscriber = new(async item =>
        {
            received.Enqueue(item);
            if (item == 0)
            {
                entered.SetResult(true);
                await release.Task.WaitAsync(timeout.Token);
            }
            if (item == 20)
            {
                finished.SetResult(true);
            }
        });

        await subscriber.EnqueueAsync(0, timeout.Token);
        await entered.Task.WaitAsync(timeout.Token);
        for (int item = 1; item <= 20; item++)
        {
            await subscriber.EnqueueAsync(item, timeout.Token);
        }
        Assert.That(received, Is.EqualTo(new[] { 0 }), "Handlers must not overlap.");

        release.SetResult(true);
        await finished.Task.WaitAsync(timeout.Token);
        subscriber.Dispose();
        await subscriber.Completion.WaitAsync(timeout.Token);

        Assert.That(received, Is.EqualTo(Enumerable.Range(0, 21)));
    }

    [Test]
    public async Task EmptyQueueWakesForEachNewMessage()
    {
        TaskCompletionSource<bool>[] handled = Enumerable.Range(0, 50).Select(_ => CreateSignal()).ToArray();
        using CancellationTokenSource timeout = new(TestTimeout);
        using MessageReceivedSubscriber<int> subscriber = new(item =>
        {
            handled[item].SetResult(true);
            return Task.CompletedTask;
        });

        for (int item = 0; item < handled.Length; item++)
        {
            await subscriber.EnqueueAsync(item, timeout.Token);
            await handled[item].Task.WaitAsync(timeout.Token);
        }

        subscriber.Dispose();
        await subscriber.Completion.WaitAsync(timeout.Token);
    }

    [Test]
    public async Task DisposalWaitsForCurrentHandlerAndDiscardsPendingMessages()
    {
        TaskCompletionSource<bool> entered = CreateSignal();
        TaskCompletionSource<bool> release = CreateSignal();
        ConcurrentQueue<int> received = new();
        using CancellationTokenSource timeout = new(TestTimeout);
        using MessageReceivedSubscriber<int> subscriber = new(async item =>
        {
            received.Enqueue(item);
            entered.TrySetResult(true);
            await release.Task.WaitAsync(timeout.Token);
        });
        await subscriber.EnqueueAsync(1, timeout.Token);
        await entered.Task.WaitAsync(timeout.Token);
        await subscriber.EnqueueAsync(2, timeout.Token);

        subscriber.Dispose();

        Assert.That(subscriber.Completion.IsCompleted, Is.False, "Disposal must not interrupt an active handler.");
        await Assert.ThrowsAsync<ObjectDisposedException>(() => subscriber.EnqueueAsync(3));
        release.SetResult(true);
        await subscriber.Completion.WaitAsync(timeout.Token);
        Assert.That(received, Is.EqualTo(new[] { 1 }));
    }

    [Test]
    public async Task DisposalCancelsProducersBlockedOnAFullQueue()
    {
        TaskCompletionSource<bool> entered = CreateSignal();
        TaskCompletionSource<bool> release = CreateSignal();
        using CancellationTokenSource timeout = new(TestTimeout);
        using MessageReceivedSubscriber<int> subscriber = new(async _ =>
        {
            entered.TrySetResult(true);
            await release.Task.WaitAsync(timeout.Token);
        });
        await subscriber.EnqueueAsync(-1, timeout.Token);
        await entered.Task.WaitAsync(timeout.Token);
        for (int item = 0; item < 100; item++)
        {
            await subscriber.EnqueueAsync(item, timeout.Token);
        }
        Task blocked = subscriber.EnqueueAsync(100, timeout.Token);
        Assert.That(blocked.IsCompleted, Is.False);

        subscriber.Dispose();

        await Assert.CatchAsync<OperationCanceledException>(async () => await blocked.WaitAsync(timeout.Token));
        Assert.That(blocked.IsCanceled, Is.True);
        release.SetResult(true);
        await subscriber.Completion.WaitAsync(timeout.Token);
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task HandlerFailureFaultsCompletionAndCancelsBlockedProducers(bool synchronousFailure)
    {
        TaskCompletionSource<bool> entered = CreateSignal();
        TaskCompletionSource<bool> release = CreateSignal();
        InvalidOperationException failure = new("Handler failed.");
        using CancellationTokenSource timeout = new(TestTimeout);
        int calls = 0;
        using MessageReceivedSubscriber<int> subscriber = new(item =>
        {
            Interlocked.Increment(ref calls);
            if (item == -1)
            {
                entered.SetResult(true);
                return release.Task.WaitAsync(timeout.Token);
            }
            if (synchronousFailure)
            {
                throw failure;
            }
            return Task.FromException(failure);
        });
        await subscriber.EnqueueAsync(-1, timeout.Token);
        await entered.Task.WaitAsync(timeout.Token);
        for (int item = 0; item < 100; item++)
        {
            await subscriber.EnqueueAsync(item, timeout.Token);
        }
        Task[] blocked = Enumerable.Range(100, 4).Select(item => subscriber.EnqueueAsync(item, timeout.Token)).ToArray();

        release.SetResult(true);
        InvalidOperationException actual = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await subscriber.Completion.WaitAsync(timeout.Token));

        Assert.That(actual, Is.SameAs(failure));
        Assert.That(calls, Is.EqualTo(2));
        foreach (Task enqueue in blocked)
        {
            // Dequeuing the failed message can admit one producer before shutdown cancels the rest.
            if (!enqueue.IsCompletedSuccessfully)
            {
                await Assert.CatchAsync<OperationCanceledException>(async () => await enqueue.WaitAsync(timeout.Token));
            }
        }
        Assert.That(blocked.Count(task => task.IsCanceled), Is.GreaterThanOrEqualTo(3));
        await Assert.ThrowsAsync<ObjectDisposedException>(() => subscriber.EnqueueAsync(200));
    }

    [Test]
    public async Task ProducerCancellationDoesNotStopDispatch()
    {
        TaskCompletionSource<bool> entered = CreateSignal();
        TaskCompletionSource<bool> release = CreateSignal();
        TaskCompletionSource<bool> finished = CreateSignal();
        ConcurrentQueue<int> received = new();
        using CancellationTokenSource timeout = new(TestTimeout);
        using CancellationTokenSource cancellation = new();
        using MessageReceivedSubscriber<int> subscriber = new(async item =>
        {
            received.Enqueue(item);
            if (item == -1)
            {
                entered.SetResult(true);
                await release.Task.WaitAsync(timeout.Token);
            }
            if (item == 101)
            {
                finished.SetResult(true);
            }
        });
        await subscriber.EnqueueAsync(-1, timeout.Token);
        await entered.Task.WaitAsync(timeout.Token);
        for (int item = 0; item < 100; item++)
        {
            await subscriber.EnqueueAsync(item, timeout.Token);
        }
        Task canceled = subscriber.EnqueueAsync(100, cancellation.Token);
        Assert.That(canceled.IsCompleted, Is.False);
        cancellation.Cancel();
        await Assert.CatchAsync<OperationCanceledException>(async () => await canceled.WaitAsync(timeout.Token));

        release.SetResult(true);
        await subscriber.EnqueueAsync(101, timeout.Token);
        await finished.Task.WaitAsync(timeout.Token);
        subscriber.Dispose();
        await subscriber.Completion.WaitAsync(timeout.Token);

        Assert.That(received, Is.EqualTo(Enumerable.Range(-1, 101).Append(101)));
    }

    [Test]
    public async Task ConcurrentProducersDoNotLoseOrDuplicateMessages()
    {
        const int ProducerCount = 4;
        const int ItemsPerProducer = 250;
        const int TotalItems = ProducerCount * ItemsPerProducer;
        ConcurrentQueue<int> received = new();
        TaskCompletionSource<bool> finished = CreateSignal();
        using CancellationTokenSource timeout = new(TestTimeout);
        using MessageReceivedSubscriber<int> subscriber = new(item =>
        {
            received.Enqueue(item);
            if (received.Count == TotalItems)
            {
                finished.SetResult(true);
            }
            return Task.CompletedTask;
        });
        Task[] producers = Enumerable.Range(0, ProducerCount).Select(producer => Task.Run(async () =>
        {
            for (int sequence = 0; sequence < ItemsPerProducer; sequence++)
            {
                await subscriber.EnqueueAsync(producer * ItemsPerProducer + sequence, timeout.Token);
            }
        })).ToArray();
        await Task.WhenAll(producers).WaitAsync(timeout.Token);
        await finished.Task.WaitAsync(timeout.Token);
        subscriber.Dispose();
        await subscriber.Completion.WaitAsync(timeout.Token);

        Assert.That(received, Is.EquivalentTo(Enumerable.Range(0, TotalItems)));
        for (int producer = 0; producer < ProducerCount; producer++)
        {
            Assert.That(received.Where(item => item / ItemsPerProducer == producer),
                Is.EqualTo(Enumerable.Range(producer * ItemsPerProducer, ItemsPerProducer)));
        }
    }
}
