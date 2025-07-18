using System.ClientModel;
using OpenAI.Chat;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ChatClient>(serviceProvider => new ChatClient(builder.Configuration["OpenAI:Model"],
    new ApiKeyCredential(builder.Configuration["OpenAI:ApiKey"]
        ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY")
        ?? throw new InvalidOperationException("OpenAI API key not found")))
);
builder.Services.AddScoped<ChatHttpHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var chatHandler = app.Services.GetRequiredService<ChatHttpHandler>();

app.MapPost("/chat/complete", chatHandler.HandleChatRequest);

app.Run();

public class ChatHttpHandler
{
    private readonly ChatClient _client;
    private readonly ILogger<ChatHttpHandler> _logger;

    // Chat completion endpoint using injected ChatClient client
    public ChatHttpHandler(ChatClient client, ILogger<ChatHttpHandler> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<ChatResponse> HandleChatRequest(ChatRequest request)
    {
        _logger.LogInformation("Handling chat request: {Message}", request.Message);
        var completion = await _client.CompleteChatAsync(request.Message);
        return new ChatResponse(completion.Value.Content[0].Text);
    }
}

public class ChatRequest
{
    public string Message { get; set; }
    
    public ChatRequest(string message)
    {
        Message = message;
    }
}

public class ChatResponse
{
    public string Response { get; set; }
    
    public ChatResponse(string response)
    {
        Response = response;
    }
}