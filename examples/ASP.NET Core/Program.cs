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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Chat completion endpoint using injected ChatClient client
app.MapPost("/chat/complete", async (ChatRequest request, ChatClient client) =>
{
    var completion = await client.CompleteChatAsync(request.Message);

    return new ChatResponse(completion.Value.Content[0].Text);
});

app.Run();

record ChatRequest(string Message);
record ChatResponse(string Response);
record EmbeddingRequest(string Text);
record EmbeddingResponse(float[] Vector);
