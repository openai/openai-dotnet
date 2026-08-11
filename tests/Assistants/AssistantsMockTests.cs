using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Assistants;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Assistants;

#pragma warning disable OPENAI001

[Parallelizable(ParallelScope.All)]
[Category("Assistants")]
[Category("Smoke")]
public class AssistantsMockTests : ClientTestBase
{
    private static readonly ApiKeyCredential s_fakeCredential = new ApiKeyCredential("key");

    public AssistantsMockTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    public async Task ModifyAssistantForwardsTheCancellationToken()
    {
        // Every convenience method on this client hands its CancellationToken to the
        // protocol layer via ToRequestOptions, which is what puts the token on the
        // outgoing PipelineMessage. The synchronous ModifyAssistant overload passed a
        // literal null instead, so the caller's token never reached the pipeline and
        // the request could not be cancelled. Both overloads must forward it.
        MockPipelineResponse response = new MockPipelineResponse(200).WithContent(
            """
            {"id":"asst_abc","object":"assistant"}
            """);

        OpenAIClientOptions options = new()
        {
            Transport = new MockPipelineTransport(_ => response)
            {
                ExpectSyncPipeline = !IsAsync
            }
        };

        AssistantClient client = CreateProxyFromClient(new AssistantClient(s_fakeCredential, options));

        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        // Without the token the mock transport answers 200 and the call succeeds, so
        // reaching this assertion at all depends on the token being forwarded.
        Assert.That(
            async () => await client.ModifyAssistantAsync("asst_abc", new AssistantModificationOptions(), cancellationSource.Token),
            Throws.InstanceOf<OperationCanceledException>());

        await Task.CompletedTask;
    }

    [Test]
    public void StreamingRunSurfacesErrorEventAsException()
    {
        // The service can emit an "error" event mid-stream (for example, when an
        // account is out of quota or the server fails while generating). The SDK
        // does not model that event as a typed update, but the failure must not be
        // silently dropped: a truncated stream would otherwise look like a clean
        // completion. The "error" event is deserialized and surfaced through the
        // SDK's standard ClientResultException, the same way non-streaming service
        // errors are reported.
        MockPipelineResponse response = new MockPipelineResponse(200).WithContent(
            """
            event: thread.run.created
            data: {"id":"run_abc","object":"thread.run","status":"queued"}

            event: error
            data: {"error":{"message":"The server had an error processing your request.","type":"server_error","param":null,"code":null}}

            event: done
            data: [DONE]
            """);

        OpenAIClientOptions options = new()
        {
            Transport = new MockPipelineTransport(_ => response)
            {
                ExpectSyncPipeline = !IsAsync
            }
        };

        AssistantClient client = new(s_fakeCredential, options);

        int updateCount = 0;

        // The run.created event surfaces normally, then the error event throws.
        ClientResultException exception = IsAsync
            ? Assert.ThrowsAsync<ClientResultException>(async () =>
            {
                await foreach (StreamingUpdate update in client.CreateRunStreamingAsync("thread_abc", "asst_abc"))
                {
                    updateCount++;
                }
            })
            : Assert.Throws<ClientResultException>(() =>
            {
                foreach (StreamingUpdate update in client.CreateRunStreaming("thread_abc", "asst_abc"))
                {
                    updateCount++;
                }
            });

        Assert.That(updateCount, Is.EqualTo(1));
        Assert.That(exception, Is.Not.Null);
        Assert.That(exception!.Message, Does.Contain("server_error"));
        Assert.That(exception.Message, Does.Contain("The server had an error processing your request."));
    }

    [Test]
    public async Task StreamingRunIgnoresBenignUnknownEvent()
    {
        // An unmodeled but benign event (one the SDK does not recognize and that is
        // not the error channel) must not throw and must not cause a
        // NullReferenceException. It simply yields no updates, so only the modeled
        // run.created event surfaces.
        MockPipelineResponse response = new MockPipelineResponse(200).WithContent(
            """
            event: thread.run.created
            data: {"id":"run_abc","object":"thread.run","status":"queued"}

            event: thread.run.some_future_event
            data: {"id":"run_abc","object":"thread.run","status":"queued"}

            event: done
            data: [DONE]
            """);

        OpenAIClientOptions options = new()
        {
            Transport = new MockPipelineTransport(_ => response)
            {
                ExpectSyncPipeline = !IsAsync
            }
        };

        AssistantClient client = new(s_fakeCredential, options);

        List<StreamingUpdate> updates = new();

        if (IsAsync)
        {
            await foreach (StreamingUpdate update in client.CreateRunStreamingAsync("thread_abc", "asst_abc"))
            {
                updates.Add(update);
            }
        }
        else
        {
            foreach (StreamingUpdate update in client.CreateRunStreaming("thread_abc", "asst_abc"))
            {
                updates.Add(update);
            }
        }

        Assert.That(updates, Has.Count.EqualTo(1));
        Assert.That(updates[0].UpdateKind, Is.EqualTo(StreamingUpdateReason.RunCreated));
    }

    [Test]
    public async Task StreamingRunContinuesAfterBenignUnknownEvent()
    {
        // An unmodeled event yields zero updates. When such an event appears in the
        // middle of a stream, the enumerator must skip it and keep reading, so that
        // every later modeled event still surfaces. Placing the unknown event last
        // cannot detect this, because there is nothing after it to lose.
        MockPipelineResponse response = new MockPipelineResponse(200).WithContent(
            """
            event: thread.run.created
            data: {"id":"run_abc","object":"thread.run","status":"queued"}

            event: thread.run.some_future_event
            data: {"id":"run_abc","object":"thread.run","status":"queued"}

            event: thread.run.in_progress
            data: {"id":"run_abc","object":"thread.run","status":"in_progress"}

            event: thread.run.another_future_event
            data: {"id":"run_abc","object":"thread.run","status":"in_progress"}

            event: thread.run.completed
            data: {"id":"run_abc","object":"thread.run","status":"completed"}

            event: done
            data: [DONE]
            """);

        OpenAIClientOptions options = new()
        {
            Transport = new MockPipelineTransport(_ => response)
            {
                ExpectSyncPipeline = !IsAsync
            }
        };

        AssistantClient client = new(s_fakeCredential, options);

        List<StreamingUpdate> updates = new();

        if (IsAsync)
        {
            await foreach (StreamingUpdate update in client.CreateRunStreamingAsync("thread_abc", "asst_abc"))
            {
                updates.Add(update);
            }
        }
        else
        {
            foreach (StreamingUpdate update in client.CreateRunStreaming("thread_abc", "asst_abc"))
            {
                updates.Add(update);
            }
        }

        // All three modeled events must arrive. Before the fix, the stream ended at the
        // first unmodeled event and only the run.created update was ever produced.
        Assert.That(updates, Has.Count.EqualTo(3));
        Assert.That(updates[0].UpdateKind, Is.EqualTo(StreamingUpdateReason.RunCreated));
        Assert.That(updates[1].UpdateKind, Is.EqualTo(StreamingUpdateReason.RunInProgress));
        Assert.That(updates[2].UpdateKind, Is.EqualTo(StreamingUpdateReason.RunCompleted));
    }
}
