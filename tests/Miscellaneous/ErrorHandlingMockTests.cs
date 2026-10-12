using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Evals;
using OpenAI.Responses;
using System.ClientModel;
using System.Collections.Generic;

namespace OpenAI.Tests.Miscellaneous;

/// <summary>
/// Validates that error responses are deserialized and formatted consistently across
/// feature areas. Covers the following expectations:
/// <list type="bullet">
///   <item>The error <c>type</c> (rendered as "kind") and <c>code</c> appear in the formatted message.</item>
///   <item>The error <c>param</c> (when present) appears in the formatted message.</item>
///   <item>An error payload that has only a <c>type</c> (and no <c>message</c>) is still
///     surfaced rather than swallowed.</item>
/// </list>
/// </summary>
[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public class ErrorHandlingMockTests : ClientTestBase
{
    private static readonly ApiKeyCredential s_fakeCredential = new ApiKeyCredential("key");

    public ErrorHandlingMockTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    public void ResponsesErrorMessageIncludesTypeCodeParamAndMessage()
    {
        const string content = """
            {
              "error": {
                "type": "invalid_request_error",
                "code": "invalid_value",
                "param": "input",
                "message": "Invalid value for parameter 'input'."
              }
            }
            """;
        MockPipelineResponse response = new MockPipelineResponse(400).WithContent(content);
        ResponsesClientOptions options = new ResponsesClientOptions
        {
            Transport = new MockPipelineTransport(_ => response) { ExpectSyncPipeline = !IsAsync },
        };
        ResponsesClient client = CreateProxyFromClient(new ResponsesClient(s_fakeCredential, options));

        Assert.That(
            async () => await client.CreateResponseAsync("model", "hello"),
            Throws.TypeOf<ClientResultException>()
                .With.Property(nameof(ClientResultException.Status)).EqualTo(400)
                .And.Message.Contains("HTTP 400")
                .And.Message.Contains("invalid_request_error")
                .And.Message.Contains("invalid_value")
                .And.Message.Contains("Parameter: input")
                .And.Message.Contains("Invalid value for parameter 'input'."));
    }

    [Test]
    public void EvaluationsErrorMessageIncludesTypeCodeParamAndMessage()
    {
        const string content = """
            {
              "error": {
                "type": "invalid_request_error",
                "code": "missing_required_field",
                "param": "data_source",
                "message": "Missing required field 'data_source'."
              }
            }
            """;
        MockPipelineResponse response = new MockPipelineResponse(400).WithContent(content);
        OpenAIClientOptions options = new OpenAIClientOptions
        {
            Transport = new MockPipelineTransport(_ => response) { ExpectSyncPipeline = !IsAsync },
        };
        EvaluationClient client = CreateProxyFromClient(new EvaluationClient(s_fakeCredential, options));

        Assert.That(
            async () => await client.GetEvaluationsAsync(limit: null, orderBy: null, order: null, after: null, options: null),
            Throws.TypeOf<ClientResultException>()
                .With.Property(nameof(ClientResultException.Status)).EqualTo(400)
                .And.Message.Contains("HTTP 400")
                .And.Message.Contains("invalid_request_error")
                .And.Message.Contains("missing_required_field")
                .And.Message.Contains("Parameter: data_source")
                .And.Message.Contains("Missing required field 'data_source'."));
    }

    [Test]
    public void ErrorMessageWithOnlyTypeIsNotSwallowed()
    {
        // Regression test: a previous version of the error formatter required a non-null
        // "message" before producing any output. An error payload with only "type" should
        // still be surfaced so the user can see *something* about the failure.
        const string content = """
            {
              "error": {
                "type": "server_error"
              }
            }
            """;
        MockPipelineResponse response = new MockPipelineResponse(500).WithContent(content);
        OpenAIClientOptions options = new OpenAIClientOptions
        {
            Transport = new MockPipelineTransport(_ => response) { ExpectSyncPipeline = !IsAsync },
        };
        ChatClient client = CreateProxyFromClient(new ChatClient("model", s_fakeCredential, options));

        Assert.That(
            async () => await client.CompleteChatAsync(new List<ChatMessage> { new UserChatMessage("hi") }),
            Throws.TypeOf<ClientResultException>()
                .With.Property(nameof(ClientResultException.Status)).EqualTo(500)
                .And.Message.Contains("HTTP 500")
                .And.Message.Contains("server_error"));
    }
}
