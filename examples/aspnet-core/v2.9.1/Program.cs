using System.ClientModel;
using OpenAI.Responses;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ResponsesClient>(serviceProvider => new ResponsesClient(
    new ApiKeyCredential(builder.Configuration["OPENAI_API_KEY"]
        ?? throw new InvalidOperationException("OpenAI API key not found")))
);

var app = builder.Build();
app.MapPost("/responses/create",
    async (ResponsesRequest request, ResponsesClient client, IConfiguration configuration) =>
{
    string model = configuration["OpenAI:Model"]
        ?? throw new InvalidOperationException("Model not configured at OpenAI:Model.");
    ResponseResult response = await client.CreateResponseAsync(model, request.Message);
    return new ResponsesResponse(response.GetOutputText());
});
app.Run();

public record ResponsesRequest(string Message);

public record ResponsesResponse(string Response);