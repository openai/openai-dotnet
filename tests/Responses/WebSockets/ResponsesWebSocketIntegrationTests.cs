using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Responses;
using OpenAI.Tests.Utility;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Responses.WebSockets;

#pragma warning disable OPENAI001

// Use direct clients: the recording proxy does not support WebSocket upgrades.
[Category("Responses")]
[LiveOnly(Reason = "Test framework doesn't support recording with websockets")]
[TestFixture(true)]
public class ResponsesWebSocketIntegrationTests : OpenAIRecordedTestBase
{
    public ResponsesWebSocketIntegrationTests(bool isAsync) : base(isAsync)
    {
        TestTimeoutInSeconds = 120;
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task StructuredOutputMatchesHttpResponse(bool streamingHttp)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(90));
        var client = TestEnvironment.GetTestResponsesClient();
        var options = new CreateResponseOptions(TestModel.Responses,
            [ResponseItem.CreateUserMessageItem("Return the capital of France in the city field.")])
        {
            StoredOutputEnabled = false,
            TextOptions = new ResponseTextOptions
            {
                TextFormat = ResponseTextFormat.CreateJsonSchemaFormat(
                    "capital",
                    BinaryData.FromString("""
                        {
                          "type": "object",
                          "properties": {
                            "city": { "type": "string", "enum": ["Paris"] }
                          },
                          "required": ["city"],
                          "additionalProperties": false
                        }
                        """),
                    jsonSchemaIsStrict: true)
            }
        };

        var webSocketResponse = await CreateWebSocketResponseAsync(client, options, timeout.Token);
        var httpResponse = await CreateHttpResponseAsync(client, options, streamingHttp, timeout.Token);

        AssertEquivalentResponseMetadata(httpResponse, webSocketResponse);
        Assert.That(httpResponse.OutputItems.Single(), Is.InstanceOf<MessageResponseItem>());
        Assert.That(webSocketResponse.OutputItems.Single(), Is.InstanceOf<MessageResponseItem>());
        Assert.That(httpResponse.TextOptions.TextFormat.Kind, Is.EqualTo(ResponseTextFormatKind.JsonSchema));
        Assert.That(webSocketResponse.TextOptions.TextFormat.Kind, Is.EqualTo(httpResponse.TextOptions.TextFormat.Kind));

        using var httpOutput = JsonDocument.Parse(httpResponse.GetOutputText());
        using var webSocketOutput = JsonDocument.Parse(webSocketResponse.GetOutputText());
        Assert.That(httpOutput.RootElement.EnumerateObject().Count(), Is.EqualTo(1));
        Assert.That(webSocketOutput.RootElement.EnumerateObject().Count(), Is.EqualTo(1));
        Assert.That(httpOutput.RootElement.GetProperty("city").GetString(), Is.EqualTo("Paris"));
        Assert.That(webSocketOutput.RootElement.GetProperty("city").GetString(),
            Is.EqualTo(httpOutput.RootElement.GetProperty("city").GetString()));
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task ForcedFunctionCallMatchesHttpResponse(bool streamingHttp)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(90));
        var client = TestEnvironment.GetTestResponsesClient();
        var options = new CreateResponseOptions(TestModel.Responses,
            [ResponseItem.CreateUserMessageItem("Look up the weather in Paris using get_weather.")])
        {
            StoredOutputEnabled = false,
            ParallelToolCallsEnabled = false,
            ToolChoice = ResponseToolChoice.CreateFunctionChoice("get_weather"),
            Tools =
            {
                ResponseTool.CreateFunctionTool(
                    functionName: "get_weather",
                    functionDescription: "Looks up the weather in Paris.",
                    functionParameters: BinaryData.FromString("""
                        {
                          "type": "object",
                          "properties": {
                            "city": { "type": "string", "enum": ["Paris"] }
                          },
                          "required": ["city"],
                          "additionalProperties": false
                        }
                        """),
                    strictModeEnabled: true)
            }
        };

        var webSocketResponse = await CreateWebSocketResponseAsync(client, options, timeout.Token);
        var httpResponse = await CreateHttpResponseAsync(client, options, streamingHttp, timeout.Token);

        AssertEquivalentResponseMetadata(httpResponse, webSocketResponse);
        Assert.That(httpResponse.OutputItems.Single(), Is.TypeOf<FunctionCallResponseItem>());
        Assert.That(webSocketResponse.OutputItems.Single(), Is.TypeOf<FunctionCallResponseItem>());
        var httpCall = (FunctionCallResponseItem)httpResponse.OutputItems.Single();
        var webSocketCall = (FunctionCallResponseItem)webSocketResponse.OutputItems.Single();
        Assert.That(httpCall.FunctionName, Is.EqualTo("get_weather"));
        Assert.That(webSocketCall.FunctionName, Is.EqualTo(httpCall.FunctionName));
        Assert.That(httpCall.CallId, Is.Not.Null.And.Not.Empty);
        Assert.That(webSocketCall.CallId, Is.Not.Null.And.Not.Empty);

        using var httpArguments = JsonDocument.Parse(httpCall.FunctionArguments);
        using var webSocketArguments = JsonDocument.Parse(webSocketCall.FunctionArguments);
        Assert.That(httpArguments.RootElement.EnumerateObject().Count(), Is.EqualTo(1));
        Assert.That(webSocketArguments.RootElement.EnumerateObject().Count(), Is.EqualTo(1));
        Assert.That(httpArguments.RootElement.GetProperty("city").GetString(), Is.EqualTo("Paris"));
        Assert.That(webSocketArguments.RootElement.GetProperty("city").GetString(),
            Is.EqualTo(httpArguments.RootElement.GetProperty("city").GetString()));
    }

    private static void AssertEquivalentResponseMetadata(ResponseResult httpResponse, ResponseResult webSocketResponse)
    {
        // IDs, timestamps, and usage vary across independent requests.
        Assert.That(httpResponse.Status, Is.EqualTo(ResponseStatus.Completed));
        Assert.That(webSocketResponse.Status, Is.EqualTo(httpResponse.Status));
        Assert.That(webSocketResponse.Model, Is.EqualTo(httpResponse.Model));
        Assert.That(httpResponse.Error, Is.Null);
        Assert.That(webSocketResponse.Error, Is.Null);
        Assert.That(httpResponse.OutputItems, Has.Count.EqualTo(1));
        Assert.That(webSocketResponse.OutputItems, Has.Count.EqualTo(httpResponse.OutputItems.Count));
    }

    private static async Task<ResponseResult> CreateHttpResponseAsync(
        ResponsesClient client, CreateResponseOptions options, bool streaming, CancellationToken cancellationToken)
    {
        if (!streaming)
        {
            return await client.CreateResponseAsync(options, cancellationToken);
        }

        options.StreamingEnabled = true;
        ResponseResult response = null;
        await foreach (var update in client.CreateResponseStreamingAsync(options, cancellationToken))
        {
            switch (update)
            {
                case StreamingResponseCompletedUpdate completed:
                    response = completed.Response;
                    break;
                case StreamingResponseFailedUpdate:
                case StreamingResponseIncompleteUpdate:
                case StreamingResponseErrorUpdate:
                    Assert.Fail($"HTTP streaming failed: {update}");
                    break;
            }
        }

        Assert.That(response, Is.Not.Null, "HTTP streaming ended without response.completed.");
        return response;
    }

    private static async Task<ResponseResult> CreateWebSocketResponseAsync(
        ResponsesClient client, CreateResponseOptions options, CancellationToken cancellationToken)
    {
        await using var webSocket = client.CreateWebSocketClient(new ResponsesWebSocketClientOptions
        {
            ShouldReconnect = false
        });
        var response = new TaskCompletionSource<ResponseResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        webSocket.MessageReceivedAsync += args =>
        {
            var message = args.Message;
            if (message.Update is StreamingResponseCompletedUpdate completed)
            {
                response.TrySetResult(completed.Response);
            }
            else if (message.Kind == ResponsesWebSocketMessageKind.Error
                || message.Update is StreamingResponseFailedUpdate or StreamingResponseIncompleteUpdate)
            {
                response.TrySetException(new InvalidOperationException($"WebSocket response failed: {message.RawData}"));
            }
            return Task.CompletedTask;
        };
        webSocket.ErrorOccurredAsync += args =>
        {
            response.TrySetException(args.Exception);
            return Task.CompletedTask;
        };

        await webSocket.ConnectAsync(cancellationToken);
        await webSocket.CreateAsync(options, cancellationToken: cancellationToken);
        var finished = await Task.WhenAny(response.Task, webSocket.Completion).WaitAsync(cancellationToken);
        if (finished != response.Task)
        {
            await webSocket.Completion;
            throw new InvalidOperationException("The WebSocket closed without response.completed.");
        }

        var result = await response.Task;
        await webSocket.CloseAsync(cancellationToken);
        return result;
    }
}
