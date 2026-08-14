using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Assistants;
using System.ClientModel;
using System.Collections.Generic;
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

    [AsyncOnly]
    [Test]
    public async Task StreamingRunSurfacesErrorEventAsException()
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
                ExpectSyncPipeline = false
            }
        };

        AssistantClient client = new(s_fakeCredential, options);

        int updateCount = 0;

        ClientResultException exception = Assert.ThrowsAsync<ClientResultException>(async () =>
            {
                await foreach (StreamingUpdate update in await client.CreateRunStreamingAsync("thread_abc", "asst_abc"))
                {
                    updateCount++;
                }
            });

        Assert.That(updateCount, Is.EqualTo(1));
        Assert.That(exception, Is.Not.Null);
        Assert.That(exception!.Message, Does.Contain("server_error"));
        Assert.That(exception.Message, Does.Contain("The server had an error processing your request."));
    }

    [AsyncOnly]
    [Test]
    public async Task StreamingRunIgnoresBenignUnknownEvent()
    {
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
                ExpectSyncPipeline = false
            }
        };

        AssistantClient client = new(s_fakeCredential, options);

        List<StreamingUpdate> updates = new();

        await foreach (StreamingUpdate update in await client.CreateRunStreamingAsync("thread_abc", "asst_abc"))
        {
            updates.Add(update);
        }

        Assert.That(updates, Has.Count.EqualTo(1));
        Assert.That(updates[0].UpdateKind, Is.EqualTo(StreamingUpdateReason.RunCreated));
    }

    [AsyncOnly]
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
                ExpectSyncPipeline = false
            }
        };

        AssistantClient client = new(s_fakeCredential, options);

        List<StreamingUpdate> updates = new();

        await foreach (StreamingUpdate update in await client.CreateRunStreamingAsync("thread_abc", "asst_abc"))
        {
            updates.Add(update);
        }

        // All three modeled events must arrive. Before the fix, the stream ended at the
        // first unmodeled event and only the run.created update was ever produced.
        Assert.That(updates, Has.Count.EqualTo(3));
        Assert.That(updates[0].UpdateKind, Is.EqualTo(StreamingUpdateReason.RunCreated));
        Assert.That(updates[1].UpdateKind, Is.EqualTo(StreamingUpdateReason.RunInProgress));
        Assert.That(updates[2].UpdateKind, Is.EqualTo(StreamingUpdateReason.RunCompleted));
    }
}
