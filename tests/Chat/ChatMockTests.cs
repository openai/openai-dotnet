using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Chat;

[Parallelizable(ParallelScope.All)]
[Category("Chat")]
[Category("Smoke")]
public class ChatMockTests : ClientTestBase
{
    private static readonly ApiKeyCredential s_fakeCredential = new ApiKeyCredential("key");

    public ChatMockTests(bool isAsync) : base(isAsync)
    {
    }

    private static readonly List<ChatMessage> s_messages = new()
    {
        new UserChatMessage("Message content.")
    };

    [Test]
    public async Task CompleteChatDeserializesId()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "id": "chat_id"
        }
        """);
        ChatClient client = CreateProxyFromClient(new ChatClient("model", s_fakeCredential, clientOptions));

        ChatCompletion chatCompletion = await client.CompleteChatAsync(s_messages);

        Assert.That(chatCompletion.Id, Is.EqualTo("chat_id"));
    }

    [Test]
    public async Task CompleteChatDeserializesCreatedAt()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "created": 1704096000
        }
        """);
        ChatClient client = CreateProxyFromClient(new ChatClient("model", s_fakeCredential, clientOptions));

        ChatCompletion chatCompletion = await client.CompleteChatAsync(s_messages);

        Assert.That(chatCompletion.CreatedAt.ToUnixTimeSeconds(), Is.EqualTo(1704096000));
    }

    [Test]
    public async Task CompleteChatDeserializesModel()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "model": "model_name"
        }
        """);
        ChatClient client = CreateProxyFromClient(new ChatClient("model", s_fakeCredential, clientOptions));

        ChatCompletion chatCompletion = await client.CompleteChatAsync(s_messages);

        Assert.That(chatCompletion.Model, Is.EqualTo("model_name"));
    }

    [Test]
    public async Task CompleteChatDeserializesSystemFingerprint()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "system_fingerprint": "fingerprint_value"
        }
        """);
        ChatClient client = CreateProxyFromClient(new ChatClient("model", s_fakeCredential, clientOptions));

        ChatCompletion chatCompletion = await client.CompleteChatAsync(s_messages);

        Assert.That(chatCompletion.SystemFingerprint, Is.EqualTo("fingerprint_value"));
    }

    [Test]
    public async Task CompleteChatDeserializesUsage()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "usage": {
                "prompt_tokens": 10,
                "completion_tokens": 20,
                "total_tokens": 30
            }
        }
        """);
        ChatClient client = CreateProxyFromClient(new ChatClient("model", s_fakeCredential, clientOptions));

        ChatCompletion chatCompletion = await client.CompleteChatAsync(s_messages);

        Assert.That(chatCompletion.Usage.InputTokenCount, Is.EqualTo(10));
        Assert.That(chatCompletion.Usage.OutputTokenCount, Is.EqualTo(20));
        Assert.That(chatCompletion.Usage.TotalTokenCount, Is.EqualTo(30));
    }

    [Test]
    [TestCase("stop", ChatFinishReason.Stop)]
    [TestCase("length", ChatFinishReason.Length)]
    [TestCase("content_filter", ChatFinishReason.ContentFilter)]
    [TestCase("tool_calls", ChatFinishReason.ToolCalls)]
    [TestCase("function_call", ChatFinishReason.FunctionCall)]
    public async Task CompleteChatDeserializesFinishReason(string stringReason, ChatFinishReason expectedReason)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, $$"""
        {
            "choices": [
                {
                    "finish_reason": "{{stringReason}}"
                }
            ]
        }
        """);
        ChatClient client = CreateProxyFromClient(new ChatClient("model", s_fakeCredential, clientOptions));

        ChatCompletion chatCompletion = await client.CompleteChatAsync(s_messages);

        Assert.That(chatCompletion.FinishReason, Is.EqualTo(expectedReason));
    }

    [Test]
    [TestCase("system", ChatMessageRole.System)]
    [TestCase("user", ChatMessageRole.User)]
    [TestCase("assistant", ChatMessageRole.Assistant)]
    [TestCase("tool", ChatMessageRole.Tool)]
    [TestCase("function", ChatMessageRole.Function)]
    public async Task CompleteChatDeserializesRole(string stringRole, ChatMessageRole expectedRole)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, $$"""
        {
            "choices": [
                {
                    "message": {
                        "role": "{{stringRole}}"
                    }
                }
            ]
        }
        """);
        ChatClient client = CreateProxyFromClient(new ChatClient("model", s_fakeCredential, clientOptions));

        ChatCompletion chatCompletion = await client.CompleteChatAsync(s_messages);

        Assert.That(chatCompletion.Role, Is.EqualTo(expectedRole));
    }

    [Test]
    public async Task CompleteChatDeserializesTextContent()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "choices": [
                {
                    "message": {
                        "content": "This is the content."
                    }
                }
            ]
        }
        """);
        ChatClient client = CreateProxyFromClient(new ChatClient("model", s_fakeCredential, clientOptions));

        ChatCompletion chatCompletion = await client.CompleteChatAsync(s_messages);
        ChatMessageContentPart contentPart = chatCompletion.Content.Single();

        Assert.That(contentPart.Kind, Is.EqualTo(ChatMessageContentPartKind.Text));
        Assert.That(contentPart.Text, Is.EqualTo("This is the content."));
    }

    [Test]
    public void CompleteChatRespectsTheCancellationToken()
    {
        ChatClient client = CreateProxyFromClient(new ChatClient("model", s_fakeCredential));
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        Assert.That(async () => await client.CompleteChatAsync(s_messages, cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
    }

    [Test]
    public void CompleteChatStreamingAsyncRespectsTheCancellationToken()
    {
        ChatClient client = CreateProxyFromClient(new ChatClient("model", s_fakeCredential));
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        IAsyncEnumerator<StreamingChatCompletionUpdate> enumerator = client
            .CompleteChatStreamingAsync(s_messages, cancellationToken: cancellationSource.Token)
            .GetAsyncEnumerator();

        Assert.That(async () => await enumerator.MoveNextAsync(), Throws.InstanceOf<OperationCanceledException>());
    }

    [SyncOnly]
    [Test]
    public void StreamingChatCanBeCancelled()
    {
        MockPipelineResponse response = new MockPipelineResponse(200).WithContent("""
            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"role":"assistant","content":"","refusal":null},"logprobs":null,"finish_reason":null}],"usage":null}

            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"content":"The"},"logprobs":null,"finish_reason":null}],"usage":null}

            data: [DONE]
            """);

        OpenAIClientOptions options = new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(_ => response)
        };

        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.CancelAfter(1000);

        ChatClient client = CreateProxyFromClient(GetTestClient<ChatClient>(TestScenario.Chat, options: options));
        IEnumerable<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];

        CollectionResult<StreamingChatCompletionUpdate> streamingResult = client.CompleteChatStreaming(messages, cancellationToken: cancellationTokenSource.Token);
        IEnumerator<StreamingChatCompletionUpdate> enumerator = streamingResult.GetEnumerator();

        enumerator.MoveNext();
        StreamingChatCompletionUpdate firstUpdate = enumerator.Current;

        Assert.That(firstUpdate, Is.Not.Null);
        Assert.That(cancellationTokenSource.IsCancellationRequested, Is.False);

        Thread.Sleep(1000);

        Assert.Throws<OperationCanceledException>(() =>
        {
            // Should throw for the second update.
            Assert.That(cancellationTokenSource.IsCancellationRequested);
            Assert.That(cancellationTokenSource.Token.IsCancellationRequested);
            enumerator.MoveNext();
            enumerator.MoveNext();
        });
    }

    [AsyncOnly]
    [Test]
    public async Task StreamingChatCanBeCancelledAsync()
    {
        MockPipelineResponse response = new MockPipelineResponse(200).WithContent("""
            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"role":"assistant","content":"","refusal":null},"logprobs":null,"finish_reason":null}],"usage":null}

            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"content":"The"},"logprobs":null,"finish_reason":null}],"usage":null}

            data: [DONE]
            """);

        OpenAIClientOptions options = new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(_ => response)
            {
                ExpectSyncPipeline = !IsAsync
            }
        };

        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.CancelAfter(1000);

        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, options: options);
        IEnumerable<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];

        AsyncCollectionResult<StreamingChatCompletionUpdate> streamingResult = client.CompleteChatStreamingAsync(messages, cancellationToken: cancellationTokenSource.Token);
        IAsyncEnumerator<StreamingChatCompletionUpdate> enumerator = streamingResult.GetAsyncEnumerator();

        await enumerator.MoveNextAsync();
        StreamingChatCompletionUpdate firstUpdate = enumerator.Current;

        Assert.That(firstUpdate, Is.Not.Null);
        Assert.That(cancellationTokenSource.IsCancellationRequested, Is.False);

        await Task.Delay(1000);

        Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            // Should throw for the second update.
            Assert.That(cancellationTokenSource.IsCancellationRequested);
            Assert.That(cancellationTokenSource.Token.IsCancellationRequested);
            await enumerator.MoveNextAsync();
            await enumerator.MoveNextAsync();
        });
    }

    [Test]
    public async Task CompleteChatStreamingClosesNetworkStream()
    {
        MockPipelineResponse response = new MockPipelineResponse(200).WithContent("""
            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"role":"assistant","content":"","refusal":null},"logprobs":null,"finish_reason":null}],"usage":null}

            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"content":"The"},"logprobs":null,"finish_reason":null}],"usage":null}

            data: [DONE]
            """);

        OpenAIClientOptions options = new()
        {
            Transport = new MockPipelineTransport(_ => response)
            {
                ExpectSyncPipeline = !IsAsync
            }
        };

        ChatClient client = CreateProxyFromClient(GetTestClient<ChatClient>(TestScenario.Chat, options: options));
        IEnumerable<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];

        int updateCount = 0;
        TimeSpan? firstTokenReceiptTime = null;
        TimeSpan? latestTokenReceiptTime = null;
        Stopwatch stopwatch = Stopwatch.StartNew();
        AsyncCollectionResult<StreamingChatCompletionUpdate> streamingResult = client.CompleteChatStreamingAsync(messages);

        Assert.That(streamingResult, Is.InstanceOf<AsyncCollectionResult<StreamingChatCompletionUpdate>>());
        Assert.That(response.IsDisposed, Is.False);

        await foreach (StreamingChatCompletionUpdate chatUpdate in streamingResult)
        {
            firstTokenReceiptTime ??= stopwatch.Elapsed;
            latestTokenReceiptTime = stopwatch.Elapsed;
            updateCount++;

            Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
        }

        stopwatch.Stop();

        Assert.That(response.IsDisposed);
    }

    [Test]
    public void GetChatCompletionMessagesWithInvalidParameters()
    {
        ChatClient client = CreateProxyFromClient(GetTestClient<ChatClient>(scenario: TestScenario.Chat));

        // Test with null completion ID
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await foreach (var message in client.GetChatCompletionMessagesAsync(null))
            {
                // Should not reach here
            }
        });

        // Test with empty completion ID
        Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await foreach (var message in client.GetChatCompletionMessagesAsync(""))
            {
                // Should not reach here
            }
        });
    }

    private OpenAIClientOptions GetClientOptionsWithMockResponse(int status, string content)
    {
        MockPipelineResponse response = new MockPipelineResponse(status).WithContent(content);

        return new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(_ => response)
            {
                ExpectSyncPipeline = !IsAsync
            }
        };
    }
}
