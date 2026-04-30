using OpenAI;
using ResponsesApiClient = OpenAI.Responses.ResponsesClient;
using ResponsesApiResult = OpenAI.Responses.ResponseResult;

var builder = WebApplication.CreateBuilder(args);
builder.AddResponsesClient("Clients:ResponsesClient");

var app = builder.Build();
app.MapPost("/responses/create",
    async (ResponsesRequest request, ResponsesApiClient client, IConfiguration configuration) =>
{
    string model = configuration["Clients:ResponsesClient:Model"]
        ?? throw new InvalidOperationException("Model not configured at Clients:ResponsesClient:Model.");
    ResponsesApiResult response = await client.CreateResponseAsync(model, request.Message);
    return new ResponsesResponse(response.GetOutputText());
});
app.Run();

public record ResponsesRequest(string Message);

public record ResponsesResponse(string Response);
