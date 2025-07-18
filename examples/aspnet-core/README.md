# OpenAI ASP.NET Core Example

This example demonstrates how to use the OpenAI .NET client library with ASP.NET Core's dependency injection container, registering a `ChatClient` as a singleton for optimal performance and resource usage.

## Features

- **Singleton Registration**: ChatClient registered as singleton in DI container
- **Thread-Safe**: Demonstrates concurrent usage for chat completion endpoints
- **Configurable Model**: Model selection via configuration (appsettings.json)
- **Modern ASP.NET Core**: Uses minimal APIs with async/await patterns

## Prerequisites

- .NET 8.0 or later
- OpenAI API key

## Setup

1. **Set your OpenAI API key** using one of these methods:

   **Environment Variable (Recommended):**

   ```powershell
   $env:OPENAI_API_KEY = "your-api-key-here"
   ```

   **Configuration (appsettings.json):**

   ```json
   {
     "OpenAI": {
       "Model": "gpt-4o-mini",
       "ApiKey": "your-api-key-here"
     }
   }
   ```

2. **Install dependencies:**

   ```powershell
   dotnet restore
   ```

3. **Run the application:**

   ```powershell
   dotnet run
   ```

## API Endpoints

### Chat Completion

- **POST** `/chat/complete`
- **Request Body:**

  ```json
  {
    "message": "Hello, how are you?"
  }
  ```

- **Response:**

  ```json
  {
    "response": "I'm doing well, thank you for asking! How can I help you today?"
  }
  ```

## Testing with PowerShell

**Chat Completion:**

```powershell
Invoke-RestMethod -Uri "https://localhost:5000/chat/complete" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"message": "What is the capital of France?"}'
```

## Key Implementation Details

### Singleton Registration

```csharp
builder.Services.AddSingleton<ChatClient>(serviceProvider => new ChatClient(
    builder.Configuration["OpenAI:Model"],
    new ApiKeyCredential(builder.Configuration["OpenAI:ApiKey"] 
        ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY")
        ?? throw new InvalidOperationException("OpenAI API key not found")))
);
```

### Dependency Injection Usage

```csharp
app.MapPost("/chat/complete", async (ChatRequest request, ChatClient client) =>
{
    var completion = await client.CompleteChatAsync(request.Message);
    
    return new ChatResponse(completion.Value.Content[0].Text);
});
```

## Why Singleton?

- **Thread-Safe**: `ChatClient` is thread-safe and can handle concurrent requests
- **Resource Efficient**: Reuses HTTP connections and avoids creating multiple instances
- **Performance**: Reduces object allocation overhead
- **Stateless**: Clients don't maintain per-request state

## Swagger UI

When running in development mode, you can access the Swagger UI at:

- `https://localhost:7071/swagger`

This provides an interactive interface to test the API endpoints.

## Additional Resources

- [Tutorial: Create a minimal API with ASP.NET Core](https://learn.microsoft.com/aspnet/core/tutorials/min-web-api)
- [.NET dependency injection](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection)
- [Logging in C# and .NET](https://learn.microsoft.com/dotnet/core/extensions/logging)