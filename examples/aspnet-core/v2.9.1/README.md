# OpenAI ASP.NET Core example (OpenAI 2.9.1 and earlier)

> **Legacy sample.** This project targets **.NET 10.0** and uses the **OpenAI 2.9.1** package. It is kept to illustrate the manual DI singleton registration pattern that users on `OpenAI` 2.9.1 or earlier are familiar with. If you're using `OpenAI` 2.10.0 or later, see the [v2.10.0 sample](../v2.10.0/README.md) for the recommended configuration-driven approach.
>
> ↩️ See the [top-level README](../README.md) to choose between samples.

This example demonstrates how to use the OpenAI .NET client library's **Responses API** with ASP.NET Core's dependency injection container, registering a `ResponsesClient` as a singleton for optimal performance and resource usage.

## Features

- **Singleton Registration**: `ResponsesClient` registered as singleton in DI container
- **Thread-Safe**: Demonstrates concurrent usage for response-creation endpoints
- **Configurable Model**: Model selection via configuration (appsettings.json)
- **Modern ASP.NET Core**: Uses minimal APIs with async/await patterns

## Prerequisites

- .NET 10 SDK or later
- An OpenAI API key

## Setup and testing

1. Store your OpenAI API key using one of the following methods:

    1. **Environment variable:**

       ```
       $env:OPENAI_API_KEY = "<your-API-key-here>"
       ```

    1. **.NET User Secrets Manager:**

        Run the following command in your project's root directory:

        ```
        dotnet user-secrets set "OPENAI_API_KEY" "<your-API-key-here>"
        ```

1. Install dependencies, build, & run the app:

   ```
   dotnet run
   ```

1. Send a request using one of the following approaches:
    1. **curl:**

        ```
        curl -X POST https://localhost:63869/responses/create \
             -H "Content-Type: application/json" \
             -d '{\"message\": \"What is the capital of France?\"}'
        ```

    1. **PowerShell:**

        ```powershell
        Invoke-RestMethod -Uri "https://localhost:63869/responses/create" `
          -Method POST `
          -ContentType "application/json" `
          -Body '{"message": "What is the capital of France?"}'
        ```

## Key implementation details

### Singleton registration

```csharp
builder.Services.AddSingleton<ResponsesClient>(serviceProvider => new ResponsesClient(
    new ApiKeyCredential(builder.Configuration["OPENAI_API_KEY"]
        ?? throw new InvalidOperationException("OpenAI API key not found")))
);
```

A singleton is used for the following reasons:

- **Thread-Safe**: `ResponsesClient` is thread-safe and can handle concurrent requests
- **Resource Efficient**: Reuses HTTP connections and avoids creating multiple instances
- **Performance**: Reduces object allocation overhead
- **Stateless**: Clients don't maintain per-request state

The `builder.Configuration["OPENAI_API_KEY"]` code retrieves the API key from configuration, which can be set via environment variables or user secrets. The `ResponsesClient` is registered as a singleton, ensuring that the same instance is reused across all requests, which is efficient and thread-safe.

### Dependency injection usage

```csharp
app.MapPost("/responses/create",
    async (ResponsesRequest request, ResponsesClient client, IConfiguration configuration) =>
{
    string model = configuration["OpenAI:Model"]
        ?? throw new InvalidOperationException("Model not configured at OpenAI:Model.");
    ResponseResult response = await client.CreateResponseAsync(model, request.Message);
    return new ResponsesResponse(response.GetOutputText());
});
```

The model is read from *appsettings.json* on each call.

## Additional resources

- [Tutorial: Create a minimal API with ASP.NET Core](https://learn.microsoft.com/aspnet/core/tutorials/min-web-api)
- [.NET dependency injection](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection)
- [Configuration in ASP.NET Core](https://learn.microsoft.com/aspnet/core/fundamentals/configuration/)
