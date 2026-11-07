using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Responses.WebSockets;

[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public class BoundedConcurrentQueueTests
{
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

    [TestCase(false)]
    [TestCase(true)]
    public async Task DisposalIsIdempotentAndRejectsFurtherOperations(bool initiallyFull)
    {
        using BoundedConcurrentQueue<int> queue = new(1);
        if (initiallyFull)
        {
            await queue.EnqueueAsync(1);
        }

        queue.Dispose();
        queue.Dispose();

        Assert.Throws<ObjectDisposedException>(() => queue.TryDequeue(out _));
        await Assert.ThrowsAsync<ObjectDisposedException>(() => queue.EnqueueAsync(2));
    }

    [Test]
    public async Task PendingEnqueueCanBeCanceledAndAwaitedBeforeDisposal()
    {
        using BoundedConcurrentQueue<int> queue = new(1);
        using CancellationTokenSource cancellation = new();
        await queue.EnqueueAsync(1);
        Task pending = queue.EnqueueAsync(2, cancellation.Token);
        Assert.That(pending.IsCompleted, Is.False);

        cancellation.Cancel();
        await Assert.CatchAsync<OperationCanceledException>(async () => await pending.WaitAsync(TestTimeout));
        queue.Dispose();

        Assert.That(pending.IsCanceled, Is.True);
        Assert.Throws<ObjectDisposedException>(() => queue.TryDequeue(out _));
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(int.MinValue)]
    public void ConstructorRejectsNonpositiveLimit(int limit)
    {
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new BoundedConcurrentQueue<int>(limit));

        Assert.That(exception.ParamName, Is.EqualTo("limit"));
    }

    [TestCase(1)]
    [TestCase(3)]
    [TestCase(int.MaxValue)]
    public void LimitMatchesCapacity(int limit)
    {
        using BoundedConcurrentQueue<int> queue = new(limit);

        Assert.That(queue.Limit, Is.EqualTo(limit));
    }

    [Test]
    public void EmptyQueueReturnsFalseAndDefaultValue()
    {
        using BoundedConcurrentQueue<int> queue = new(1);

        Assert.That(queue.TryDequeue(out int item), Is.False);
        Assert.That(item, Is.Zero);
    }

    [Test]
    public async Task ItemsAreDequeuedInFifoOrderAcrossRefills()
    {
        using BoundedConcurrentQueue<int> queue = new(3);
        using CancellationTokenSource timeout = new(TestTimeout);

        for (int item = 0; item < 3; item++)
        {
            await queue.EnqueueAsync(item, timeout.Token);
        }

        for (int expected = 0; expected < 10; expected++)
        {
            Assert.That(queue.TryDequeue(out int item), Is.True);
            Assert.That(item, Is.EqualTo(expected));
            await queue.EnqueueAsync(expected + 3, timeout.Token);
        }

        for (int expected = 10; expected < 13; expected++)
        {
            Assert.That(queue.TryDequeue(out int item), Is.True);
            Assert.That(item, Is.EqualTo(expected));
        }

        Assert.That(queue.TryDequeue(out _), Is.False);
    }

    [Test]
    public async Task NullIsAValidItem()
    {
        using BoundedConcurrentQueue<string> queue = new(1);
        using CancellationTokenSource timeout = new(TestTimeout);

        await queue.EnqueueAsync(null, timeout.Token);

        Assert.That(queue.TryDequeue(out string item), Is.True);
        Assert.That(item, Is.Null);
        Assert.That(queue.TryDequeue(out _), Is.False);
    }

    [Test]
    public async Task FullQueueReturnsPendingTaskUntilAnItemIsDequeued()
    {
        using BoundedConcurrentQueue<int> queue = new(1);
        using CancellationTokenSource timeout = new(TestTimeout);
        await queue.EnqueueAsync(1, timeout.Token);

        Task enqueue = queue.EnqueueAsync(2, timeout.Token);

        Assert.That(enqueue.IsCompleted, Is.False, "A full queue must return a pending task without blocking the caller.");
        Assert.That(queue.TryDequeue(out int first), Is.True);
        Assert.That(first, Is.EqualTo(1));

        await enqueue.WaitAsync(timeout.Token);

        Assert.That(queue.TryDequeue(out int second), Is.True);
        Assert.That(second, Is.EqualTo(2));
        Assert.That(queue.TryDequeue(out _), Is.False);
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task AlreadyCanceledEnqueueDoesNotChangeTheQueue(bool initiallyFull)
    {
        using BoundedConcurrentQueue<int> queue = new(1);
        using CancellationTokenSource timeout = new(TestTimeout);
        using CancellationTokenSource cancellation = new();
        if (initiallyFull)
        {
            await queue.EnqueueAsync(1, timeout.Token);
        }
        cancellation.Cancel();

        Task enqueue = queue.EnqueueAsync(2, cancellation.Token);
        OperationCanceledException exception = await Assert.CatchAsync<OperationCanceledException>(() => enqueue);

        Assert.That(exception.CancellationToken, Is.EqualTo(cancellation.Token));
        Assert.That(enqueue.IsCanceled, Is.True);
        if (initiallyFull)
        {
            Assert.That(queue.TryDequeue(out int item), Is.True);
            Assert.That(item, Is.EqualTo(1));
        }
        Assert.That(queue.TryDequeue(out _), Is.False);

        await queue.EnqueueAsync(3, timeout.Token);
        Assert.That(queue.TryDequeue(out int next), Is.True);
        Assert.That(next, Is.EqualTo(3));
    }

    [Test]
    public async Task CancelingBlockedEnqueueDoesNotEnqueueOrLeakCapacity()
    {
        using BoundedConcurrentQueue<int> queue = new(1);
        using CancellationTokenSource timeout = new(TestTimeout);
        using CancellationTokenSource cancellation = new(TestTimeout);
        await queue.EnqueueAsync(1, timeout.Token);
        Task canceledEnqueue = queue.EnqueueAsync(2, cancellation.Token);
        Assert.That(canceledEnqueue.IsCompleted, Is.False);

        cancellation.Cancel();
        OperationCanceledException exception = await Assert.CatchAsync<OperationCanceledException>(
            async () => await canceledEnqueue.WaitAsync(timeout.Token));

        Assert.That(exception.CancellationToken, Is.EqualTo(cancellation.Token));
        Assert.That(canceledEnqueue.IsCanceled, Is.True);
        Assert.That(queue.TryDequeue(out int first), Is.True);
        Assert.That(first, Is.EqualTo(1));
        Assert.That(queue.TryDequeue(out _), Is.False);

        await queue.EnqueueAsync(3, timeout.Token);
        Task nextEnqueue = queue.EnqueueAsync(4, timeout.Token);
        Assert.That(nextEnqueue.IsCompleted, Is.False, "Cancellation must not create an extra slot.");
        Assert.That(queue.TryDequeue(out int third), Is.True);
        Assert.That(third, Is.EqualTo(3));
        await nextEnqueue.WaitAsync(timeout.Token);
        Assert.That(queue.TryDequeue(out int fourth), Is.True);
        Assert.That(fourth, Is.EqualTo(4));
        Assert.That(queue.TryDequeue(out _), Is.False);
    }

    [Test]
    public async Task CancelingOneProducerDoesNotCancelAnother()
    {
        using BoundedConcurrentQueue<int> queue = new(1);
        using CancellationTokenSource timeout = new(TestTimeout);
        using CancellationTokenSource cancellation = new(TestTimeout);
        await queue.EnqueueAsync(1, timeout.Token);
        Task canceledEnqueue = queue.EnqueueAsync(2, cancellation.Token);
        Task survivingEnqueue = queue.EnqueueAsync(3, timeout.Token);

        cancellation.Cancel();
        await Assert.CatchAsync<OperationCanceledException>(async () => await canceledEnqueue.WaitAsync(timeout.Token));
        Assert.That(survivingEnqueue.IsCompleted, Is.False);
        Assert.That(queue.TryDequeue(out int first), Is.True);
        Assert.That(first, Is.EqualTo(1));
        await survivingEnqueue.WaitAsync(timeout.Token);
        Assert.That(queue.TryDequeue(out int next), Is.True);
        Assert.That(next, Is.EqualTo(3));
        Assert.That(queue.TryDequeue(out _), Is.False);
    }

    [Test]
    public async Task FailedDequeueDoesNotIncreaseCapacity()
    {
        using BoundedConcurrentQueue<int> queue = new(1);
        using CancellationTokenSource timeout = new(TestTimeout);
        Assert.That(queue.TryDequeue(out _), Is.False);
        Assert.That(queue.TryDequeue(out _), Is.False);
        await queue.EnqueueAsync(1, timeout.Token);

        Task enqueue = queue.EnqueueAsync(2, timeout.Token);

        Assert.That(enqueue.IsCompleted, Is.False);
        Assert.That(queue.TryDequeue(out int first), Is.True);
        Assert.That(first, Is.EqualTo(1));
        await enqueue.WaitAsync(timeout.Token);
        Assert.That(queue.TryDequeue(out int second), Is.True);
        Assert.That(second, Is.EqualTo(2));
        Assert.That(queue.TryDequeue(out _), Is.False);
    }

    [TestCase(1)]
    [TestCase(4)]
    public async Task EachDequeueUnblocksOnlyOneWaitingProducer(int limit)
    {
        using BoundedConcurrentQueue<int> queue = new(limit);
        using CancellationTokenSource timeout = new(TestTimeout);
        for (int item = 0; item < limit; item++)
        {
            await queue.EnqueueAsync(item, timeout.Token);
        }
        List<Task> pending = Enumerable.Range(limit, 8)
            .Select(item => queue.EnqueueAsync(item, timeout.Token))
            .ToList();
        List<int> received = new();

        while (pending.Count > 0)
        {
            Assert.That(pending.All(task => !task.IsCompleted), Is.True);
            Assert.That(queue.TryDequeue(out int item), Is.True);
            received.Add(item);
            Task completed = await Task.WhenAny(pending).WaitAsync(timeout.Token);
            await completed;
            Assert.That(pending.Count(task => task.IsCompleted), Is.EqualTo(1));
            pending.Remove(completed);
        }
        while (queue.TryDequeue(out int item))
        {
            received.Add(item);
        }

        Assert.That(received.Take(limit), Is.EqualTo(Enumerable.Range(0, limit)));
        Assert.That(received, Is.EquivalentTo(Enumerable.Range(0, limit + 8)));
    }

    [TestCase(1)]
    [TestCase(4)]
    public async Task ConcurrentEnqueuesCannotExceedCapacity(int limit)
    {
        const int ProducerCount = 32;
        using BoundedConcurrentQueue<int> queue = new(limit);
        using CancellationTokenSource timeout = new(TestTimeout);
        TaskCompletionSource<bool> start = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Task[] enqueues = new Task[ProducerCount];
        Task[] callers = Enumerable.Range(0, ProducerCount).Select(item => Task.Run(async () =>
        {
            await start.Task;
            enqueues[item] = queue.EnqueueAsync(item, timeout.Token);
        })).ToArray();

        try
        {
            start.SetResult(true);
            await Task.WhenAll(callers).WaitAsync(timeout.Token);
            Assert.That(enqueues.Count(task => task.IsCompletedSuccessfully), Is.EqualTo(limit));
            List<Task> pending = enqueues.Where(task => !task.IsCompleted).ToList();
            List<int> received = new();
            while (pending.Count > 0)
            {
                Assert.That(queue.TryDequeue(out int item), Is.True);
                received.Add(item);
                Task completed = await Task.WhenAny(pending).WaitAsync(timeout.Token);
                await completed;
                Assert.That(pending.Count(task => task.IsCompleted), Is.EqualTo(1));
                pending.Remove(completed);
            }
            while (queue.TryDequeue(out int item))
            {
                received.Add(item);
            }

            Assert.That(received, Is.EquivalentTo(Enumerable.Range(0, ProducerCount)));
        }
        finally
        {
            timeout.Cancel();
        }
    }

    [TestCase(1, 1)]
    [TestCase(4, 1)]
    [TestCase(4, 4)]
    public async Task ConcurrentProducersAndConsumersDoNotLoseOrDuplicateItems(int limit, int consumerCount)
    {
        const int ProducerCount = 4;
        const int ItemsPerProducer = 200;
        const int TotalItems = ProducerCount * ItemsPerProducer;
        using BoundedConcurrentQueue<int> queue = new(limit);
        ConcurrentQueue<int> received = new();
        using CancellationTokenSource timeout = new(TestTimeout);
        TaskCompletionSource<bool> start = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Task[] producers = Enumerable.Range(0, ProducerCount).Select(producer => Task.Run(async () =>
        {
            await start.Task;
            for (int sequence = 0; sequence < ItemsPerProducer; sequence++)
            {
                await queue.EnqueueAsync(producer * ItemsPerProducer + sequence, timeout.Token);
            }
        })).ToArray();
        Task[] consumers = Enumerable.Range(0, consumerCount).Select(_ => Task.Run(async () =>
        {
            await start.Task;
            for (int count = 0; count < TotalItems / consumerCount;)
            {
                timeout.Token.ThrowIfCancellationRequested();
                if (queue.TryDequeue(out int item))
                {
                    received.Enqueue(item);
                    count++;
                }
                else
                {
                    await Task.Yield();
                }
            }
        })).ToArray();
        Task all = Task.WhenAll(producers.Concat(consumers));

        try
        {
            start.SetResult(true);
            await all.WaitAsync(timeout.Token);
        }
        finally
        {
            timeout.Cancel();
        }

        Assert.That(received, Is.EquivalentTo(Enumerable.Range(0, TotalItems)));
        Assert.That(queue.TryDequeue(out _), Is.False);
        if (consumerCount == 1)
        {
            for (int producer = 0; producer < ProducerCount; producer++)
            {
                Assert.That(
                    received.Where(item => item / ItemsPerProducer == producer),
                    Is.EqualTo(Enumerable.Range(producer * ItemsPerProducer, ItemsPerProducer)),
                    "A single consumer must observe each producer's items in FIFO order.");
            }
        }
    }
}
