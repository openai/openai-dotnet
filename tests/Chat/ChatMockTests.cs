using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
