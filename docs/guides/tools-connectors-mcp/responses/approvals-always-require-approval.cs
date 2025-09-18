// SAMPLE: Generate response from remote MCP through Responses API
// PAGE: https://platform.openai.com/docs/guides/tools-connectors-mcp#approvals
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*
#:property PublishAot=false

using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
OpenAIResponseClient client = new(model: "gpt-5", apiKey: key);

ResponseCreationOptions options1 = new();
options1.Tools.Add(ResponseTool.CreateMcpTool(
    serverLabel: "dmcp",
    serverUri: new Uri("https://dmcp-server.deno.dev/sse")
));

McpToolCallApprovalRequestItem request1 = ResponseItem.CreateMcpApprovalRequestItem(
    serverLabel: "dmcp",
    name: "roll",
    arguments: BinaryData.FromObjectAsJson(new { diceRollExpression = "2d4+1" })
);

OpenAIResponse response1 = (OpenAIResponse)client.CreateResponse([
    request1
], options1);

ResponseCreationOptions options2 = new();
options2.Tools.Add(ResponseTool.CreateMcpTool(
    serverLabel: "dmcp",
    serverUri: new Uri("https://dmcp-server.deno.dev/sse"),
    toolCallApprovalPolicy: new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.AlwaysRequireApproval)
));

options2.PreviousResponseId = response1.Id;

OpenAIResponse response2 = (OpenAIResponse)client.CreateResponse([
    request1,
    ResponseItem.CreateMcpApprovalResponseItem(request1.Id, approved: true),
], options2);

Console.WriteLine(response2.GetOutputText());