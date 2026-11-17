using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests.Assistants;

#pragma warning disable OPENAI001

[Parallelizable(ParallelScope.All)]
[Category("Assistants")]
[Category("Smoke")]
public class AssistantsStreamingTests
{
    private const string RunEvents = """
        event: thread.run.created
        data: {"id":"run_abc","object":"thread.run","thread_id":"thread_abc","assistant_id":"asst_abc","status":"queued"}

        event: thread.run.completed
        data: {"id":"run_abc","object":"thread.run","thread_id":"thread_abc","assistant_id":"asst_abc","status":"completed"}

        event: done
        data: [DONE]


        """;

    public enum Operation
    {
        CreateRun,
        CreateThreadAndRun,
        SubmitToolOutputsToRun,
    }

    [Test]
    public async Task StreamingRunMethodsReturnAnEstablishedResult([Values] Operation operation)
    {
        StreamingTestServer server = new(RunEvents);
        AssistantClient client = new(new ApiKeyCredential("key"), new AssistantClientOptions { Transport = server.Transport });

        await using AsyncStreamingResult<StreamingUpdate> result = operation switch
        {
            Operation.CreateRun => await client.CreateRunStreamingAsync("thread_abc", "asst_abc"),
            Operation.CreateThreadAndRun => await client.CreateThreadAndRunStreamingAsync("asst_abc", new ThreadCreationOptions()),
            _ => await client.SubmitToolOutputsToRunStreamingAsync("thread_abc", "run_abc", [new ToolOutput("call_abc", "42")]),
        };

        Assert.That(server.RequestCount, Is.EqualTo(1));
        Assert.That(server.LastBufferResponse, Is.False);
        Assert.That(server.LastRequestJson.GetProperty("stream").GetBoolean(), Is.True);
        Assert.That(server.LastRequestUri.AbsolutePath, Does.EndWith(operation switch
        {
            Operation.CreateRun => "/threads/thread_abc/runs",
            Operation.CreateThreadAndRun => "/threads/runs",
            _ => "/threads/thread_abc/runs/run_abc/submit_tool_outputs",
        }));
        Assert.That(result.Status, Is.EqualTo(200));
        Assert.That(result.Headers.TryGetValue("x-request-id", out string requestId), Is.True);
        Assert.That(requestId, Is.EqualTo(StreamingTestServer.RequestId));
        Assert.That(server.LastResponseStream.BytesRead, Is.Zero);

        List<StreamingUpdate> updates = [];

        await foreach (StreamingUpdate update in result)
        {
            updates.Add(update);
        }

        Assert.That(updates.Select(update => update.UpdateKind),
            Is.EqualTo(new[] { StreamingUpdateReason.RunCreated, StreamingUpdateReason.RunCompleted }));
        Assert.That(updates.OfType<RunUpdate>().Select(update => update.Value.Id), Is.All.EqualTo("run_abc"));
        Assert.That(server.RequestCount, Is.EqualTo(1));
        Assert.That(server.LastResponseStream.IsDisposed, Is.True);
    }

    [Test]
    public async Task StreamingRunErrorEventFailsEnumerationAndReleasesTheResponse()
    {
        StreamingTestServer server = new("""
            event: error
            data: {"error":{"code":"server_error","message":"The run failed."}}


            """);
        AssistantClient client = new(new ApiKeyCredential("key"), new AssistantClientOptions { Transport = server.Transport });
        AsyncStreamingResult<StreamingUpdate> result = await client.CreateRunStreamingAsync("thread_abc", "asst_abc");

        Assert.That(
            async () =>
            {
                await foreach (StreamingUpdate update in result)
                {
                }
            },
            Throws.InstanceOf<ClientResultException>().With.Message.Contains("The run failed."));
        Assert.That(server.LastResponseStream.IsDisposed, Is.True);
    }
}
