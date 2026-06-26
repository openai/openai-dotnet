using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Assistants;
using System.ClientModel;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
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
    public async Task CreateThreadAndRunStreamingKeepsRequestContentAliveUntilEnumeration()
    {
        string requestBody = null;
        MockPipelineResponse response = CreateStreamingRunResponse();
        OpenAIClientOptions options = new()
        {
            Transport = new MockPipelineTransport(message =>
            {
                using MemoryStream stream = new();
                message.Request.Content.WriteTo(stream);
                requestBody = BinaryData.FromBytes(stream.ToArray()).ToString();
                return response;
            })
            {
                ExpectSyncPipeline = !IsAsync
            }
        };

        AssistantClient client = new(s_fakeCredential, options);

        if (IsAsync)
        {
            await foreach (StreamingUpdate _ in client.CreateThreadAndRunStreamingAsync("asst_abc"))
            {
            }
        }
        else
        {
            foreach (StreamingUpdate _ in client.CreateThreadAndRunStreaming("asst_abc"))
            {
            }
        }

        using JsonDocument requestDocument = JsonDocument.Parse(requestBody);
        Assert.That(requestDocument.RootElement.GetProperty("assistant_id").GetString(), Is.EqualTo("asst_abc"));
        Assert.That(requestDocument.RootElement.GetProperty("stream").GetBoolean(), Is.True);
    }

    private static MockPipelineResponse CreateStreamingRunResponse()
        => new MockPipelineResponse(200).WithContent(
            """
            event: thread.run.created
            data: {"id":"run_abc","object":"thread.run","status":"queued"}

            event: done
            data: [DONE]
            """);
}
