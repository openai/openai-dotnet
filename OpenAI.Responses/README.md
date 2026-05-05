# OpenAI.Responses .NET API library

[![NuGet stable version](https://img.shields.io/nuget/v/openai.responses.svg)](https://www.nuget.org/packages/OpenAI.Responses)

The `OpenAI.Responses` .NET library provides convenient access to the [OpenAI Responses API](https://platform.openai.com/docs/api-reference/responses) from .NET applications.

It is generated from the [OpenAPI specification](https://github.com/openai/openai-openapi) in collaboration with Microsoft.

## Table of Contents

- [Getting started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Install the NuGet package](#install-the-nuget-package)
- [Using the client library](#using-the-client-library)
  - [Using the async API](#using-the-async-api)
  - [Using a custom base URL and API key](#using-a-custom-base-url-and-api-key)
- [How to use dependency injection](#how-to-use-dependency-injection)
- [How to use responses with streaming and reasoning](#how-to-use-responses-with-streaming-and-reasoning)
- [How to use responses with file search](#how-to-use-responses-with-file-search)
- [How to use responses with web search](#how-to-use-responses-with-web-search)
- [How to work with Azure OpenAI](#how-to-work-with-azure-openai)
- [Advanced scenarios](#advanced-scenarios)
  - [Using protocol methods](#using-protocol-methods)
  - [Mock a client for testing](#mock-a-client-for-testing)
  - [Automatically retrying errors](#automatically-retrying-errors)
  - [Observability](#observability)

## Getting started

### Prerequisites

To call the OpenAI REST API, you will need an API key. To obtain one, first [create a new OpenAI account](https://platform.openai.com/signup) or [log in](https://platform.openai.com/login). Next, navigate to the [API key page](https://platform.openai.com/account/api-keys) and select "Create new secret key", optionally naming the key. Make sure to save your API key somewhere safe and do not share it with anyone.

### Install the NuGet package

Add the client library to your .NET project by installing the [NuGet](https://www.nuget.org/) package via your IDE or by running the following command in the .NET CLI:

```cli
dotnet add package OpenAI.Responses
```

Note that the code examples included below were written using [.NET 10](https://dotnet.microsoft.com/download/dotnet/10.0). The `OpenAI.Responses` library is compatible with all .NET Standard 2.0 applications, but the syntax used in some of the code examples in this document may depend on newer language features.

## Using the client library

The full API of this library can be found in the [OpenAI.Responses.netstandard2.0.cs](https://github.com/openai/openai-dotnet/blob/main/api/OpenAI.Responses.netstandard2.0.cs) file, and there are many [code examples](https://github.com/openai/openai-dotnet/tree/main/examples/Responses) to help. The basic entry point is the `ResponsesClient` class:

```C# Snippet:ReadMe_ResponsesClient_Basic
ResponsesClient client = new(apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

CreateResponseOptions options = new() { Model = "gpt-5.1" };
options.InputItems.Add(ResponseItem.CreateUserMessageItem("Say 'this is a test.'"));

ResponseResult response = client.CreateResponse(options);
Console.WriteLine($"[ASSISTANT]: {response.GetOutputText()}");
```

While you can pass your API key directly as a string, it is highly recommended that you keep it in a secure location and instead access it via an environment variable or configuration file as shown above to avoid storing it in source control.

### Using the async API

Every method on `ResponsesClient` that performs a synchronous API call has an asynchronous variant in the same class. For instance, the asynchronous variant of `CreateResponse` is `CreateResponseAsync`. To rewrite a call using the asynchronous counterpart, simply `await` the call:

```C# Snippet:ReadMe_ResponsesClient_Async
ResponseResult response = await client.CreateResponseAsync(options);
```

### Using a custom base URL and API key

If you need to connect to an alternative API endpoint (for example, a proxy or self-hosted OpenAI-compatible LLM), you can specify a custom base URL and API key using the `ApiKeyCredential` and `ResponsesClientOptions`:

```C# Snippet:ReadMe_ResponsesClient_CustomUrl
ResponsesClient client = new(
    credential: new ApiKeyCredential(Environment.GetEnvironmentVariable("OPENAI_API_KEY")),
    options: new ResponsesClientOptions()
    {
        Endpoint = new Uri("https://YOUR_BASE_URL")
    });
```

Replace `YOUR_BASE_URL` with your endpoint URI. This is useful when working with OpenAI-compatible APIs or custom deployments.

## How to use dependency injection

The `ResponsesClient` is **thread-safe** and can be safely registered as a **singleton** in ASP.NET Core's Dependency Injection container. This maximizes resource efficiency and HTTP connection reuse. In your *Program.cs* file, register the `ResponsesClient` as follows:

```C# Snippet:ReadMe_DependencyInjection_RegisterResponses
builder.AddResponsesClient("Clients:ResponsesClient");
```

Then inject and use the client in your controllers or services:

```C# Snippet:ReadMe_DependencyInjection_ResponsesController
[ApiController]
[Route("api/[controller]")]
public class ResponsesController : ControllerBase
{
    private readonly ResponsesClient _responsesClient;

    public ResponsesController(ResponsesClient responsesClient)
    {
        _responsesClient = responsesClient;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateResponse([FromBody] string message)
    {
        CreateResponseOptions options = new() { Model = "gpt-5.1" };
        options.InputItems.Add(ResponseItem.CreateUserMessageItem(message));
        ResponseResult response = await _responsesClient.CreateResponseAsync(options);
        return Ok(new { response = response.GetOutputText() });
    }
}
```

> For a complete ASP.NET Core sample project, see the [dependency injection sample](https://github.com/openai/openai-dotnet/tree/main/examples/aspnet-core).

## How to use responses with streaming and reasoning

```C# Snippet:ReadMe_ResponsesStreaming
ResponsesClient client = new(
    apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

CreateResponseOptions options = new()
{
    Model = "gpt-5.1",
    ReasoningOptions = new ResponseReasoningOptions()
    {
        ReasoningEffortLevel = ResponseReasoningEffortLevel.High,
    },
};

options.InputItems.Add(ResponseItem.CreateUserMessageItem("What's the optimal strategy to win at poker?"));
ResponseResult response = await client.CreateResponseAsync(options);

CreateResponseOptions streamingOptions = new()
{
    Model = "gpt-5.1",
    ReasoningOptions = new ResponseReasoningOptions()
    {
        ReasoningEffortLevel = ResponseReasoningEffortLevel.High,
    },
    StreamingEnabled = true,
};

streamingOptions.InputItems.Add(ResponseItem.CreateUserMessageItem("What's the optimal strategy to win at poker?"));

await foreach (StreamingResponseUpdate update
    in client.CreateResponseStreamingAsync(streamingOptions))
{
    if (update is StreamingResponseOutputItemAddedUpdate itemUpdate
        && itemUpdate.Item is ReasoningResponseItem reasoningItem)
    {
        Console.WriteLine($"[Reasoning] ({reasoningItem.Status})");
    }
    else if (update is StreamingResponseOutputItemAddedUpdate itemDone
        && itemDone.Item is ReasoningResponseItem reasoningDone)
    {
        Console.WriteLine($"[Reasoning DONE] ({reasoningDone.Status})");
    }
    else if (update is StreamingResponseOutputTextDeltaUpdate delta)
    {
        Console.Write(delta.Delta);
    }
}
```

## How to use responses with file search

```C# Snippet:ReadMe_ResponsesFileSearch
ResponsesClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
string vectorStoreId = "vs-123";

ResponseTool fileSearchTool
    = ResponseTool.CreateFileSearchTool(vectorStoreIds: [vectorStoreId]);

CreateResponseOptions options = new()
{
    Model = "gpt-5.1",
    Tools = { fileSearchTool }
};

options.InputItems.Add(ResponseItem.CreateUserMessageItem("According to available files, what's the secret number?"));
ResponseResult response = await client.CreateResponseAsync(options);

foreach (ResponseItem outputItem in response.OutputItems)
{
    if (outputItem is FileSearchCallResponseItem fileSearchCall)
    {
        Console.WriteLine($"[file_search] ({fileSearchCall.Status}): {fileSearchCall.Id}");
        foreach (string query in fileSearchCall.Queries)
        {
            Console.WriteLine($"  - {query}");
        }
    }
    else if (outputItem is MessageResponseItem message)
    {
        Console.WriteLine($"[{message.Role}] {message.Content.FirstOrDefault()?.Text}");
    }
}
```

## How to use responses with web search

```C# Snippet:ReadMe_ResponsesWebSearch
ResponsesClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

CreateResponseOptions options = new()
{
    Model = "gpt-5.1",
    Tools = { ResponseTool.CreateWebSearchTool() },
};

options.InputItems.Add(ResponseItem.CreateUserMessageItem("What's a happy news headline from today?"));
ResponseResult response = await client.CreateResponseAsync(options);

foreach (ResponseItem item in response.OutputItems)
{
    if (item is WebSearchCallResponseItem webSearchCall)
    {
        Console.WriteLine($"[Web search invoked]({webSearchCall.Status}) {webSearchCall.Id}");
    }
    else if (item is MessageResponseItem message)
    {
        Console.WriteLine($"[{message.Role}] {message.Content?.FirstOrDefault()?.Text}");
    }
}
```

## How to work with Azure OpenAI

Switching from OpenAI to Azure OpenAI is simple, and in most cases requires little to no code changes. To get started quickly, check out the starter kit at https://aka.ms/openai/start. If you want to understand how endpoint switching works, you can also read: https://aka.ms/openai/switch.

### Secure Access with Microsoft Entra ID (No API Keys)

The starter kit includes examples showing how to call Azure OpenAI securely using Microsoft Entra ID instead of API keys. This is the recommended approach for production scenarios. Here's a direct link to the .NET sample using Entra ID in the starter kit: https://github.com/Azure-Samples/azure-openai-starter/blob/main/src/dotnet/responses_example_entra.cs

Below is the core pattern using the OpenAI SDK for .NET with Azure OpenAI + Entra ID:

```C# Snippet:ReadMe_AzureOpenAI
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
    ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is required.");

var client = new ResponsesClient(
    new BearerTokenPolicy(new DefaultAzureCredential(), "https://ai.azure.com/.default"),
    new ResponsesClientOptions { Endpoint = new Uri($"{endpoint}/openai/v1/") }
);

var response = await client.CreateResponseAsync("gpt-5-mini", "Hello world!");
Console.WriteLine(response.Value.GetOutputText());
```

### Why this works

- One OpenAI SDK: You use the official OpenAI SDK for .NET. Azure OpenAI is just a different endpoint you point the client library to.
- Unified /openai/v1/ endpoint: Azure OpenAI uses the same path shape as OpenAI, so most client code can stay unchanged.
- Enterprise-ready auth: Azure Identity SDK with Microsoft Entra ID lets you access Azure OpenAI without storing secrets.
- Drop-in model switching: Swap "gpt-5-mini" or any other model as long as the Azure model deployment has the same name as the model.

## Advanced scenarios

### Using protocol methods

In addition to the client methods that use strongly-typed request and response objects, the .NET library also provides _protocol methods_ that enable more direct access to the REST API. Protocol methods are "binary in, binary out" accepting `BinaryContent` as request bodies and providing `BinaryData` as response bodies.

You can call the protocol method variant of `CreateResponse` by passing a request body as `BinaryContent`, and then retrieve the response body as `BinaryData` via the `PipelineResponse`'s `Content` property on the resulting `ClientResult`.

### Mock a client for testing

The OpenAI .NET library has been designed to support mocking, providing key features such as:

- Client methods made virtual to allow overriding.
- Model factories to assist in instantiating API output models that lack public constructors.

To support mocking output models such as `ResponseResult` (which lacks public constructors), use the `OpenAIResponsesModelFactory` static class to build instances for return values from your mocks.

### Automatically retrying errors

By default, the `ResponsesClient` will automatically retry the following errors up to three additional times using exponential backoff:

- 408 Request Timeout
- 429 Too Many Requests
- 500 Internal Server Error
- 502 Bad Gateway
- 503 Service Unavailable
- 504 Gateway Timeout

### Observability

The OpenAI .NET library supports experimental distributed tracing and metrics with OpenTelemetry. Check out [Observability with OpenTelemetry](https://github.com/openai/openai-dotnet/blob/main/docs/Observability.md) for more details.
