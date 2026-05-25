using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Assistants;
using System.ClientModel;
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
    public async Task StreamingRunSurfacesErrorEventWithoutThrowing()
    {
        // The service can emit an "error" event mid-stream (for example, when an
        // account is out of quota or the server fails while generating). The SDK
        // does not model that event as a typed update, so the streaming machinery
        // must simply produce no updates for it rather than throwing.
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

        // The "error" and terminal "done" events yield no updates, so only the
        // single run.created event should surface.
        if (IsAsync)
        {
            await foreach (StreamingUpdate update in client.CreateRunStreamingAsync("thread_abc", "asst_abc"))
            {
                updateCount++;
            }
        }
        else
        {
            foreach (StreamingUpdate update in client.CreateRunStreaming("thread_abc", "asst_abc"))
            {
                updateCount++;
            }
        }

        Assert.That(updateCount, Is.EqualTo(1));
    }
}
