using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Assistants;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

    [TestCase(false)]
    [TestCase(true)]
    public async Task ModifyAssistantRespectsTheCancellationToken(bool canceled)
    {
        using CancellationTokenSource cancellationSource = new();
        if (canceled)
        {
            cancellationSource.Cancel();
        }

        AssistantClientOptions clientOptions = new()
        {
            Transport = new MockPipelineTransport(_ => new MockPipelineResponse(200).WithContent("""
                {"id":"asst_abc","object":"assistant","created_at":0,"model":"gpt-4o","name":"updated","tools":[]}
                """))
            {
                ExpectSyncPipeline = !IsAsync
            }
        };
        AssistantClient client = new(s_fakeCredential, clientOptions);
        AssistantModificationOptions modificationOptions = new() { Name = "updated" };

        if (canceled)
        {
            if (IsAsync)
            {
                Assert.That(async () => await client.ModifyAssistantAsync("asst_abc", modificationOptions, cancellationSource.Token),
                    Throws.InstanceOf<OperationCanceledException>());
            }
            else
            {
                Assert.That(() => client.ModifyAssistant("asst_abc", modificationOptions, cancellationSource.Token),
                    Throws.InstanceOf<OperationCanceledException>());
            }
        }
        else
        {
            Assistant assistant = IsAsync
                ? await client.ModifyAssistantAsync("asst_abc", modificationOptions, cancellationSource.Token)
                : client.ModifyAssistant("asst_abc", modificationOptions, cancellationSource.Token);
            Assert.That(assistant.Name, Is.EqualTo("updated"));
        }
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task CreateThreadAndRunRespectsTheCancellationToken(bool canceled)
    {
        using CancellationTokenSource cancellationSource = new();
        if (canceled)
        {
            cancellationSource.Cancel();
        }

        AssistantClientOptions clientOptions = new()
        {
            Transport = new MockPipelineTransport(_ => new MockPipelineResponse(200).WithContent("""
                {"id":"run_abc","object":"thread.run","status":"queued"}
                """))
            {
                ExpectSyncPipeline = !IsAsync
            }
        };
        AssistantClient client = new(s_fakeCredential, clientOptions);

        if (canceled)
        {
            if (IsAsync)
            {
                Assert.That(async () => await client.CreateThreadAndRunAsync("asst_abc", new ThreadCreationOptions(), cancellationToken: cancellationSource.Token),
                    Throws.InstanceOf<OperationCanceledException>());
            }
            else
            {
                Assert.That(() => client.CreateThreadAndRun("asst_abc", new ThreadCreationOptions(), cancellationToken: cancellationSource.Token),
                    Throws.InstanceOf<OperationCanceledException>());
            }
        }
        else
        {
            ThreadRun run = IsAsync
                ? await client.CreateThreadAndRunAsync("asst_abc", new ThreadCreationOptions(), cancellationToken: cancellationSource.Token)
                : client.CreateThreadAndRun("asst_abc", new ThreadCreationOptions(), cancellationToken: cancellationSource.Token);
            Assert.That(run.Id, Is.EqualTo("run_abc"));
        }
    }

    [AsyncOnly]
    [Test]
    public async Task CreateThreadAndRunStreamingDoesNotBufferTheResponse()
    {
        bool? bufferResponse = null;
        AssistantClientOptions clientOptions = new()
        {
            Transport = new MockPipelineTransport(message =>
            {
                bufferResponse = message.BufferResponse;
                return new MockPipelineResponse(200).WithContent("""
                    event: thread.run.created
                    data: {"id":"run_abc","object":"thread.run","status":"queued"}

                    event: done
                    data: [DONE]
                    """);
            })
            {
                ExpectSyncPipeline = !IsAsync
            }
        };
        AssistantClient client = new(s_fakeCredential, clientOptions);

        List<StreamingUpdate> updates = new();
        await foreach (StreamingUpdate update in await client.CreateThreadAndRunStreamingAsync("asst_abc", new ThreadCreationOptions()))
        {
            updates.Add(update);
        }

        Assert.That(updates, Has.Count.EqualTo(1));
        Assert.That(bufferResponse, Is.False);
    }

    [AsyncOnly]
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

        AssistantClientOptions options = new()
        {
            Transport = new MockPipelineTransport(_ => response)
            {
                ExpectSyncPipeline = !IsAsync
            }
        };

        AssistantClient client = new(s_fakeCredential, options);

        int updateCount = 0;

        // The run.created event surfaces normally, then the error event throws.
        Assert.That(
            async () =>
            {
                await foreach (StreamingUpdate update in await client.CreateRunStreamingAsync("thread_abc", "asst_abc"))
                {
                    updateCount++;
                }
            },
            Throws.TypeOf<ClientResultException>()
                .With.Message.Contains("server_error")
                .And.Message.Contains("The server had an error processing your request."));

        Assert.That(updateCount, Is.EqualTo(1));
    }

    [AsyncOnly]
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

        AssistantClientOptions options = new()
        {
            Transport = new MockPipelineTransport(_ => response)
            {
                ExpectSyncPipeline = !IsAsync
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

        AssistantClientOptions options = new()
        {
            Transport = new MockPipelineTransport(_ => response)
            {
                ExpectSyncPipeline = !IsAsync
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

    [AsyncOnly]
    [Test]
    public async Task StreamingTextAnnotationOfUnknownTypeReportsIndices()
    {
        // Annotation types that the library does not model keep their raw properties, from which
        // the start and end indices are read.
        MockPipelineResponse response = new MockPipelineResponse(200).WithContent(
            """
            event: thread.message.delta
            data: {"id":"msg_abc","object":"thread.message.delta","delta":{"content":[{"index":0,"type":"text","text":{"value":"abc","annotations":[{"index":0,"type":"some_future_annotation","text":"abc","start_index":5,"end_index":12}]}}]}}

            event: done
            data: [DONE]
            """);

        AssistantClientOptions options = new()
        {
            Transport = new MockPipelineTransport(_ => response)
            {
                ExpectSyncPipeline = !IsAsync
            }
        };

        AssistantClient client = new(s_fakeCredential, options);

        List<StreamingUpdate> updates = new();

        await foreach (StreamingUpdate update in await client.CreateRunStreamingAsync("thread_abc", "asst_abc"))
        {
            updates.Add(update);
        }

        TextAnnotationUpdate annotation = null;
        foreach (StreamingUpdate update in updates)
        {
            if (update is MessageContentUpdate contentUpdate && contentUpdate.TextAnnotation is not null)
            {
                annotation = contentUpdate.TextAnnotation;
            }
        }

        Assert.That(annotation, Is.Not.Null);
        Assert.That(annotation.ContentIndex, Is.EqualTo(0));
        Assert.That(annotation.StartIndex, Is.EqualTo(5));
        Assert.That(annotation.EndIndex, Is.EqualTo(12));
    }

    [Test]
    public async Task CreateThreadAndRunWithoutThreadOptions()
    {
        string requestBody = null;
        AssistantClientOptions clientOptions = new()
        {
            Transport = new MockPipelineTransport(message =>
            {
                using MemoryStream stream = new();
                message.Request.Content.WriteTo(stream);
                requestBody = Encoding.UTF8.GetString(stream.ToArray());
                return new MockPipelineResponse(200).WithContent("""
                    {"id":"run_abc","object":"thread.run","status":"queued"}
                    """);
            })
            {
                ExpectSyncPipeline = !IsAsync
            }
        };
        AssistantClient client = new(s_fakeCredential, clientOptions);

        ThreadRun run = IsAsync
            ? await client.CreateThreadAndRunAsync("asst_abc")
            : client.CreateThreadAndRun("asst_abc");

        Assert.That(run.Id, Is.EqualTo("run_abc"));
        Assert.That(requestBody, Does.Contain("\"assistant_id\":\"asst_abc\""));
        Assert.That(requestBody, Does.Not.Contain("\"thread\""));
    }

    [AsyncOnly]
    [Test]
    public async Task CreateThreadAndRunStreamingWithoutThreadOptions()
    {
        AssistantClientOptions clientOptions = new()
        {
            Transport = new MockPipelineTransport(_ => new MockPipelineResponse(200).WithContent("""
                event: thread.run.created
                data: {"id":"run_abc","object":"thread.run","status":"queued"}

                event: done
                data: [DONE]
                """))
            {
                ExpectSyncPipeline = !IsAsync
            }
        };
        AssistantClient client = new(s_fakeCredential, clientOptions);

        List<StreamingUpdate> updates = new();
        await foreach (StreamingUpdate update in await client.CreateThreadAndRunStreamingAsync("asst_abc"))
        {
            updates.Add(update);
        }

        Assert.That(updates, Has.Count.EqualTo(1));
    }
}
