// SAMPLE: Generate response from remote MCP with approval through Responses API
// PAGE: https://platform.openai.com/docs/guides/tools-connectors-mcp#approvals
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*
#:property PublishAot=false

using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
OpenAIResponseClient client = new(model: "gpt-5", apiKey: key);

ResponseCreationOptions options = new();
options.Tools.Add(ResponseTool.CreateMcpTool(
    serverLabel: "dmcp",
    serverUri: new Uri("https://dmcp-server.deno.dev/sse"),
    toolCallApprovalPolicy: new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.AlwaysRequireApproval)
));

// STEP 1: Create response that requests tool call approval
OpenAIResponse response1 = (OpenAIResponse)client.CreateResponse([
    ResponseItem.CreateUserMessageItem([
        ResponseContentPart.CreateInputTextPart("Roll 2d4+1")
    ])
], options);

McpToolCallApprovalRequestItem? approvalRequestItem = response1.OutputItems.Last() as McpToolCallApprovalRequestItem;

// STEP 2: Approve the tool call request and get final response
options.PreviousResponseId = response1.Id;
OpenAIResponse response2 = (OpenAIResponse)client.CreateResponse([
    ResponseItem.CreateMcpApprovalResponseItem(approvalRequestItem!.Id, approved: true),
], options);

Console.WriteLine(response2.GetOutputText());