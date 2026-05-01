using OpenAI;
using OpenAI.Responses;

var builder = WebApplication.CreateBuilder(args);

#region Snippet:Add_Responses_Client
builder.AddResponsesClient("Clients:ResponsesClient");
#endregion

var app = builder.Build();

#region Snippet:Responses_Create_Endpoint
app.MapPost("/responses/create",
    async (ResponsesRequest request, ResponsesClient client, IConfiguration configuration) =>
{
    string model = configuration["Clients:ResponsesClient:Model"]
        ?? throw new InvalidOperationException("Model not configured at Clients:ResponsesClient:Model.");
    ResponseResult response = await client.CreateResponseAsync(model, request.Message);
    return new ResponsesResponse(response.GetOutputText());
});
#endregion

app.Run();

public record ResponsesRequest(string Message);

public record ResponsesResponse(string Response);
