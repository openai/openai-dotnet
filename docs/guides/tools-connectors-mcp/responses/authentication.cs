// SAMPLE: Generate response from remote MCP through Responses API
// PAGE: https://platform.openai.com/docs/guides/tools-connectors-mcp#authentication
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*

using OpenAI.Responses;

string authToken = Environment.GetEnvironmentVariable("STRIPE_OAUTH_ACCESS_TOKEN")!;
string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
ResponsesClient client = new(key);

CreateResponseOptions options = new() { Model = "gpt-5.2" };
options.Tools.Add(
    ResponseTool.CreateMcpTool(
        serverLabel: "stripe",
        serverUri: new Uri("https://mcp.stripe.com"),
        authorizationToken: authToken
    )
);
options.InputItems.Add(
    ResponseItem.CreateUserMessageItem("Create a payment link for $20")
);

ResponseResult response = client.CreateResponse(options);

Console.WriteLine(response.GetOutputText());