# OpenAI .NET Dependency Injection Extensions

This document demonstrates the new dependency injection features added to the OpenAI .NET library.

## Quick Start

### 1. Basic Registration

```csharp
using OpenAI.Extensions.DependencyInjection;

// Register individual clients
builder.Services.AddOpenAIChat("gpt-4o", "your-api-key");
builder.Services.AddOpenAIEmbeddings("text-embedding-3-small", "your-api-key");

// Register OpenAI client factory
builder.Services.AddOpenAI("your-api-key");
builder.Services.AddOpenAIChat("gpt-4o"); // Uses registered OpenAIClient
```

### 2. Configuration-Based Registration

**appsettings.json:**
```json
{
  "OpenAI": {
    "ApiKey": "your-api-key",
    "DefaultChatModel": "gpt-4o",
    "DefaultEmbeddingModel": "text-embedding-3-small",
    "Endpoint": "https://api.openai.com/v1",
    "OrganizationId": "your-org-id"
  }
}
```

**Program.cs:**
```csharp
using OpenAI.Extensions.DependencyInjection;

// Configure from appsettings.json
builder.Services.AddOpenAIFromConfiguration(builder.Configuration);

// Add clients using default models from configuration
builder.Services.AddChatClientFromConfiguration();
builder.Services.AddEmbeddingClientFromConfiguration();

// Or add all common clients at once
builder.Services.AddAllOpenAIClientsFromConfiguration();
```

### 3. Controller Usage

```csharp
[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatClient _chatClient;
    private readonly EmbeddingClient _embeddingClient;

    public ChatController(ChatClient chatClient, EmbeddingClient embeddingClient)
    {
        _chatClient = chatClient;
        _embeddingClient = embeddingClient;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] string message)
    {
        var completion = await _chatClient.CompleteChatAsync(message);
        return Ok(new { response = completion.Content[0].Text });
    }

    [HttpPost("embeddings")]
    public async Task<IActionResult> GetEmbeddings([FromBody] string text)
    {
        var embedding = await _embeddingClient.GenerateEmbeddingAsync(text);
        var vector = embedding.ToFloats();
        return Ok(new { dimensions = vector.Length, vector = vector.ToArray() });
    }
}
```

## Available Extension Methods

### Core Extensions (ServiceCollectionExtensions)

- **AddOpenAI()** - Register OpenAIClient factory
  - Overloads: API key, ApiKeyCredential, configuration action
- **AddOpenAIChat()** - Register ChatClient
  - Direct or via existing OpenAIClient
- **AddOpenAIEmbeddings()** - Register EmbeddingClient
- **AddOpenAIAudio()** - Register AudioClient
- **AddOpenAIImages()** - Register ImageClient
- **AddOpenAIModeration()** - Register ModerationClient

### Configuration Extensions (ServiceCollectionExtensionsAdvanced)

- **AddOpenAIFromConfiguration()** - Bind from IConfiguration
- **AddChatClientFromConfiguration()** - Add ChatClient from config
- **AddEmbeddingClientFromConfiguration()** - Add EmbeddingClient from config
- **AddAudioClientFromConfiguration()** - Add AudioClient from config
- **AddImageClientFromConfiguration()** - Add ImageClient from config
- **AddModerationClientFromConfiguration()** - Add ModerationClient from config
- **AddAllOpenAIClientsFromConfiguration()** - Add all clients from config

## Configuration Options (OpenAIServiceOptions)

Extends `OpenAIClientOptions` with:

- **ApiKey** - API key (falls back to OPENAI_API_KEY environment variable)
- **DefaultChatModel** - Default: "gpt-4o"
- **DefaultEmbeddingModel** - Default: "text-embedding-3-small"
- **DefaultAudioModel** - Default: "whisper-1"
- **DefaultImageModel** - Default: "dall-e-3"
- **DefaultModerationModel** - Default: "text-moderation-latest"

Plus all base options: Endpoint, OrganizationId, ProjectId, etc.

## Key Features

✅ **Thread-Safe Singleton Registration** - All clients registered as singletons for optimal performance  
✅ **Configuration Binding** - Full support for IConfiguration and appsettings.json  
✅ **Environment Variable Fallback** - Automatic fallback to OPENAI_API_KEY  
✅ **Multiple Registration Patterns** - Direct, factory-based, and configuration-based  
✅ **Comprehensive Error Handling** - Clear error messages for missing configuration  
✅ **.NET Standard 2.0 Compatible** - Works with all .NET implementations  
✅ **Fully Tested** - covering all scenarios  
✅ **Backward Compatible** - No breaking changes to existing code  

## Error Handling

The extension methods provide clear error messages for common configuration issues:

- Missing API keys
- Missing configuration sections
- Invalid model specifications
- Missing required services

All methods validate input parameters and throw appropriate exceptions with helpful messages.