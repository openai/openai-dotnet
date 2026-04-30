# OpenAI ASP.NET Core example

This sample demonstrates the configuration-driven approach available in the OpenAI .NET client library starting with version **2.10.0**, where a `ResponsesClient` is constructed from a strongly-typed `ResponsesClientSettings` instance bound from `IConfiguration`. It calls the OpenAI **Responses API**.

## Features

- **Configuration-driven registration**: `ResponsesClient` is bound from `IConfiguration` via `ResponsesClientSettings` using the `builder.AddResponsesClient(...)` extension method
- **Strongly-typed settings**: `ResponsesClientSettings` enables validation for client configuration
- **Thread-Safe**: `ResponsesClient` is thread-safe and registered for the application's lifetime
- **Configurable Model**: Model selection via configuration (appsettings.json)
- **Modern ASP.NET Core**: Uses minimal APIs with async/await patterns

## Prerequisites

- .NET 10 SDK or later
- An OpenAI API key

## Setup and testing

1. Store your OpenAI API key using one of the following methods:

    1. **Environment variable:**

        ```
        $env:Clients__ResponsesClient__Credential__Key = "<your-API-key-here>"
        ```

    1. **.NET User Secrets Manager:**

        Run the following command in your project's root directory:

        ```
        dotnet user-secrets set "Clients:ResponsesClient:Credential:Key" "<your-API-key-here>"
        ```

1. Install dependencies, build, & run the app:

   ```
   dotnet run
   ```

1. Send a request using one of the following approaches:
    1. **curl:**

        ```
        curl -X POST "https://localhost:7098/responses/create" \
             -H "Content-Type: application/json" \
             -d "{\"message\": \"What is the capital of France?\"}"
        ```

    1. **PowerShell:**

        ```powershell
        Invoke-RestMethod -Uri "https://localhost:7098/responses/create" `
          -Method POST `
          -ContentType "application/json" `
          -Body '{"message": "What is the capital of France?"}'
        ```

## Key implementation details

### Configuration-driven registration

```csharp
builder.AddResponsesClient("Clients:ResponsesClient");
```

`AddResponsesClient` is an `IHostApplicationBuilder` extension provided by the OpenAI library. It reads the named configuration section (here `Clients:ResponsesClient`), binds it to `ResponsesClientSettings`, and registers the resulting `ResponsesClient` with the DI container. The credential is resolved from the `Credential` subsection (e.g., `Clients:ResponsesClient:Credential:Key`), so no manual `ApiKeyCredential` plumbing is required.

`ResponsesClientSettings` is annotated `[Experimental("SCME0002")]`. This warning is suppressed via `<NoWarn>SCME0002</NoWarn>` in the project file.

### Dependency injection usage

```csharp
app.MapPost("/responses/create",
    async (ResponsesRequest request, ResponsesClient client, IConfiguration configuration) =>
{
    string model = configuration["Clients:ResponsesClient:Model"]
        ?? throw new InvalidOperationException("Model not configured at Clients:ResponsesClient:Model.");
    ResponseResult response = await client.CreateResponseAsync(model, request.Message);
    return new ResponsesResponse(response.GetOutputText());
});
```

The model is read from *appsettings.json* on each call.

## Additional resources

- [Tutorial: Create a minimal API with ASP.NET Core](https://learn.microsoft.com/aspnet/core/tutorials/min-web-api)
- [.NET dependency injection](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection)
- [Configuration in ASP.NET Core](https://learn.microsoft.com/aspnet/core/fundamentals/configuration/)
