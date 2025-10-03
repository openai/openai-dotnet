// SAMPLE: Generate response from remote MCP through Responses API
// PAGE: https://platform.openai.com/docs/guides/tools-connectors-mcp#authentication
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*
#:property PublishAot=false

using OpenAI.Responses;

string authToken = Environment.GetEnvironmentVariable("STRIPE_OAUTH_ACCESS_TOKEN")!;
string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
OpenAIResponseClient client = new(model: "gpt-5", apiKey: key);

ResponseCreationOptions options = new();
options.Tools.Add(ResponseTool.CreateMcpTool(
    serverLabel: "stripe",
    serverUri: new Uri("https://mcp.stripe.com"),
    authorizationToken: authToken
));

OpenAIResponse response = client.CreateResponse([
    ResponseItem.CreateUserMessageItem([
        ResponseContentPart.CreateInputTextPart("Create a payment link for $20")
    ])
], options);

Console.WriteLine(response.GetOutputText());