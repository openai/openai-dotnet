// SAMPLE: Generate response from remote MCP through Responses API
// PAGE: https://platform.openai.com/docs/quickstart?tool-type=remote-mcp#extend-the-model-with-tools
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*

using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
ResponsesClient client = new(model: "gpt-5.2", apiKey: key);

CreateResponseOptions options = new();
options.Tools.Add(ResponseTool.CreateMcpTool(
    serverLabel: "dmcp",
    serverUri: new Uri("https://dmcp-server.deno.dev/sse"),
    toolCallApprovalPolicy: new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.NeverRequireApproval)
));
options.InputItems.Add(ResponseItem.CreateUserMessageItem("Roll 2d4+1"));

ResponseResult response = client.CreateResponse(options);

Console.WriteLine(response.GetOutputText());