using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Responses.WebSockets;

[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public class ResponsesWebSocketClientTests
{
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

    [Test]
    public async Task SeparateRegistrationsDispatchInParallel()
    {
        using ResponsesWebSocketClient client = new TestClient();
        TaskCompletionSource<bool> firstEntered = CreateSignal();
        TaskCompletionSource<bool> secondEntered = CreateSignal();
        TaskCompletionSource<bool> release = CreateSignal();
        TaskCompletionSource<bool> firstFinished = CreateSignal();
        TaskCompletionSource<bool> secondFinished = CreateSignal();

        client.MessageReceivedAsync += async _ =>
        {
            firstEntered.SetResult(true);
            await release.Task.WaitAsync(TestTimeout);
            firstFinished.SetResult(true);
        };
        client.MessageReceivedAsync += async _ =>
        {
            secondEntered.SetResult(true);
            await release.Task.WaitAsync(TestTimeout);
            secondFinished.SetResult(true);
        };

        await DispatchAsync(client).WaitAsync(TestTimeout);
        await Task.WhenAll(firstEntered.Task, secondEntered.Task).WaitAsync(TestTimeout);
        release.SetResult(true);
        await Task.WhenAll(firstFinished.Task, secondFinished.Task).WaitAsync(TestTimeout);
        await client.DisposeAsync();
    }

    [Test]
    public async Task RemovingRegistrationStopsFutureDispatch()
    {
        using ResponsesWebSocketClient client = new TestClient();
        int removedCalls = 0;
        TaskCompletionSource<bool> retainedCalled = CreateSignal();
        Func<ResponsesWebSocketMessageReceivedEventArgs, Task> removed = _ =>
        {
            Interlocked.Increment(ref removedCalls);
            return Task.CompletedTask;
        };
        Func<ResponsesWebSocketMessageReceivedEventArgs, Task> retained = _ =>
        {
            retainedCalled.SetResult(true);
            return Task.CompletedTask;
        };
        client.MessageReceivedAsync += removed;
        client.MessageReceivedAsync += retained;
        client.MessageReceivedAsync -= removed;

        await DispatchAsync(client).WaitAsync(TestTimeout);
        await retainedCalled.Task.WaitAsync(TestTimeout);
        await client.DisposeAsync();

        Assert.That(removedCalls, Is.Zero);
    }

    [Test]
    public async Task RemovingDuplicateRegistrationRemovesOnlyLastMatch()
    {
        using ResponsesWebSocketClient client = new TestClient();
        int calls = 0;
        Func<ResponsesWebSocketMessageReceivedEventArgs, Task> handler = _ =>
        {
            Interlocked.Increment(ref calls);
            return Task.CompletedTask;
        };
        client.MessageReceivedAsync += handler;
        client.MessageReceivedAsync += handler;
        client.MessageReceivedAsync -= handler;

        await DispatchAsync(client).WaitAsync(TestTimeout);
        await WaitUntilAsync(() => Volatile.Read(ref calls) == 1);
        await client.DisposeAsync();

        Assert.That(calls, Is.EqualTo(1));
    }

    [Test]
    public async Task MulticastRegistrationAwaitsHandlersInParallel()
    {
        using ResponsesWebSocketClient client = new TestClient();
        TaskCompletionSource<bool> firstEntered = CreateSignal();
        TaskCompletionSource<bool> secondEntered = CreateSignal();
        TaskCompletionSource<bool> release = CreateSignal();
        TaskCompletionSource<bool> firstFinished = CreateSignal();
        TaskCompletionSource<bool> secondFinished = CreateSignal();
        Func<ResponsesWebSocketMessageReceivedEventArgs, Task> handlers = async _ =>
        {
            firstEntered.SetResult(true);
            await release.Task.WaitAsync(TestTimeout);
            firstFinished.SetResult(true);
        };
        handlers += async _ =>
        {
            secondEntered.SetResult(true);
            await release.Task.WaitAsync(TestTimeout);
            secondFinished.SetResult(true);
        };
        client.MessageReceivedAsync += handlers;

        await DispatchAsync(client).WaitAsync(TestTimeout);
        await Task.WhenAll(firstEntered.Task, secondEntered.Task).WaitAsync(TestTimeout);
        release.SetResult(true);
        await Task.WhenAll(firstFinished.Task, secondFinished.Task).WaitAsync(TestTimeout);
        await client.DisposeAsync();
    }

    [Test]
    public async Task ErrorRegistrationsDispatchInParallelAndAreRemovedIndependently()
    {
        using ResponsesWebSocketClient client = new TestClient();
        TaskCompletionSource<bool> firstEntered = CreateSignal();
        TaskCompletionSource<bool> secondEntered = CreateSignal();
        TaskCompletionSource<bool> release = CreateSignal();
        TaskCompletionSource<bool> firstFinished = CreateSignal();
        int removedCalls = 0;
        Func<ResponsesWebSocketErrorEventArgs, Task> first = async _ =>
        {
            firstEntered.SetResult(true);
            await release.Task.WaitAsync(TestTimeout);
            firstFinished.SetResult(true);
        };
        Func<ResponsesWebSocketErrorEventArgs, Task> removed = _ =>
        {
            Interlocked.Increment(ref removedCalls);
            return Task.CompletedTask;
        };
        Func<ResponsesWebSocketErrorEventArgs, Task> second = async _ =>
        {
            secondEntered.SetResult(true);
            await release.Task.WaitAsync(TestTimeout);
        };
        client.ErrorOccurredAsync += first;
        client.ErrorOccurredAsync += removed;
        client.ErrorOccurredAsync += second;
        client.ErrorOccurredAsync -= removed;

        await DispatchErrorAsync(client).WaitAsync(TestTimeout);
        await Task.WhenAll(firstEntered.Task, secondEntered.Task).WaitAsync(TestTimeout);
        release.SetResult(true);
        await firstFinished.Task.WaitAsync(TestTimeout);
        await client.DisposeAsync();

        Assert.That(removedCalls, Is.Zero);
    }

    [Test]
    public async Task ErrorHandlerFailureIsNotDispatchedRecursively()
    {
        using ResponsesWebSocketClient client = new TestClient();
        InvalidOperationException failure = new("Error handler failed.");
        TaskCompletionSource<bool> entered = CreateSignal();
        int calls = 0;
        client.ErrorOccurredAsync += _ =>
        {
            Interlocked.Increment(ref calls);
            entered.TrySetResult(true);
            return Task.FromException(failure);
        };

        await DispatchErrorAsync(client).WaitAsync(TestTimeout);
        await entered.Task.WaitAsync(TestTimeout);
        InvalidOperationException actual = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await client.DisposeAsync().AsTask().WaitAsync(TestTimeout));

        Assert.That(actual, Is.SameAs(failure));
        Assert.That(calls, Is.EqualTo(1));
    }

    [Test]
    public async Task ClientDisposalCancelsBothHandlerTokens()
    {
        ResponsesWebSocketClient client = new TestClient();
        TaskCompletionSource<CancellationToken> messageToken = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<CancellationToken> errorToken = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<bool> callbacksCanceled = CreateSignal();
        int canceledCallbacks = 0;
        client.MessageReceivedAsync += async args =>
        {
            messageToken.TrySetResult(args.CancellationToken);
            try
            {
                await Task.Delay(Timeout.Infinite, args.CancellationToken);
            }
            finally
            {
                if (args.CancellationToken.IsCancellationRequested && Interlocked.Increment(ref canceledCallbacks) == 2)
                {
                    callbacksCanceled.TrySetResult(true);
                }
            }
        };
        client.ErrorOccurredAsync += async args =>
        {
            errorToken.TrySetResult(args.CancellationToken);
            try
            {
                await Task.Delay(Timeout.Infinite, args.CancellationToken);
            }
            finally
            {
                if (args.CancellationToken.IsCancellationRequested && Interlocked.Increment(ref canceledCallbacks) == 2)
                {
                    callbacksCanceled.TrySetResult(true);
                }
            }
        };

        await Task.WhenAll(DispatchAsync(client), DispatchErrorAsync(client)).WaitAsync(TestTimeout);
        CancellationToken messageCancellationToken = await messageToken.Task.WaitAsync(TestTimeout);
        CancellationToken errorCancellationToken = await errorToken.Task.WaitAsync(TestTimeout);
        Assert.That(messageCancellationToken.CanBeCanceled, Is.True);
        Assert.That(errorCancellationToken.CanBeCanceled, Is.True);
        client.Dispose();
        await callbacksCanceled.Task.WaitAsync(TestTimeout);
        Assert.That(messageCancellationToken.IsCancellationRequested, Is.True);
        Assert.That(errorCancellationToken.IsCancellationRequested, Is.True);
        await client.DisposeAsync();
    }

    private static TaskCompletionSource<bool> CreateSignal()
        => new(TaskCreationOptions.RunContinuationsAsynchronously);

    private static Task DispatchAsync(ResponsesWebSocketClient client)
    {
        MethodInfo dispatch = typeof(ResponsesWebSocketClient).GetMethod(
            "DispatchMessageAsync", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(dispatch, Is.Not.Null);
        CancellationToken token = GetLifetimeToken(client);
        return (Task)dispatch.Invoke(client, new object[] { new TestMessageReceivedEventArgs(token), token });
    }

    private static Task DispatchErrorAsync(ResponsesWebSocketClient client)
    {
        MethodInfo dispatch = typeof(ResponsesWebSocketClient).GetMethod(
            "DispatchErrorAsync", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(dispatch, Is.Not.Null);
        CancellationToken token = GetLifetimeToken(client);
        return (Task)dispatch.Invoke(client, new object[] { new TestErrorEventArgs(token), token });
    }

    private static CancellationToken GetLifetimeToken(ResponsesWebSocketClient client)
    {
        FieldInfo lifetimeSourceField = typeof(ResponsesWebSocketClient).GetField(
            "_lifetimeSource", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(lifetimeSourceField, Is.Not.Null);
        CancellationTokenSource lifetimeSource = (CancellationTokenSource)lifetimeSourceField.GetValue(client);
        return lifetimeSource.Token;
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        using CancellationTokenSource timeout = new(TestTimeout);
        while (!condition())
        {
            await Task.Delay(1, timeout.Token);
        }
    }

    private sealed class TestClient : ResponsesWebSocketClient
    {
        public TestClient()
            : base(new ResponsesClient("test-key"), new ResponsesWebSocketClientOptions())
        {
        }
    }

    private sealed class TestErrorEventArgs : ResponsesWebSocketErrorEventArgs
    {
        private readonly CancellationToken _cancellationToken;
        public TestErrorEventArgs(CancellationToken cancellationToken) => _cancellationToken = cancellationToken;
        public override Exception Exception => new InvalidOperationException("Test transport error.");
        public override bool IsTerminal => false;
        public override CancellationToken CancellationToken => _cancellationToken;
    }
    private sealed class TestMessageReceivedEventArgs : ResponsesWebSocketMessageReceivedEventArgs
    {
        private readonly CancellationToken _cancellationToken;
        public TestMessageReceivedEventArgs(CancellationToken cancellationToken) => _cancellationToken = cancellationToken;
        public override ResponsesWebSocketMessage Message => null;
        public override CancellationToken CancellationToken => _cancellationToken;
    }
}
