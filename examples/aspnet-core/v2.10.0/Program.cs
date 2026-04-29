using System.ClientModel;
using OpenAI.Responses;

var builder = WebApplication.CreateBuilder(args);
builder.AddClient<ResponsesClient, ResponsesClientSettings>("Clients:ResponsesClient");

var app = builder.Build();
app.MapPost("/responses/create",
    async (ResponsesRequest request, ResponsesClient client, IConfiguration configuration) =>
{
    string model = configuration["Clients:ResponsesClient:Model"]
        ?? throw new InvalidOperationException("Model not configured at Clients:ResponsesClient:Model.");
    ResponseResult response = await client.CreateResponseAsync(model, request.Message);
    return new ResponsesResponse(response.GetOutputText());
});
app.Run();

public class ResponsesRequest(string message)
{
    public string Message { get; set; } = message;
}

public class ResponsesResponse(string response)
{
    public string Response { get; set; } = response;
}
