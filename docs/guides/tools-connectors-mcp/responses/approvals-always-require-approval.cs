// SAMPLE: Generate response from remote MCP with approval through Responses API
// PAGE: https://platform.openai.com/docs/guides/tools-connectors-mcp#approvals
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*

using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
ResponsesClient client = new(key);

CreateResponseOptions options = new() { Model = "gpt-5.2" };
options.Tools.Add(
    ResponseTool.CreateMcpTool(
        serverLabel: "dmcp",
        serverUri: new Uri("https://dmcp-server.deno.dev/sse"),
        toolCallApprovalPolicy: new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.AlwaysRequireApproval)
    )
);

// STEP 1: Create response that requests tool call approval
options.InputItems.Add(
    ResponseItem.CreateUserMessageItem("Roll 2d4+1")
);
ResponseResult response1 = client.CreateResponse(options);

McpToolCallApprovalRequestItem? approvalRequestItem = response1.OutputItems.Last() as McpToolCallApprovalRequestItem;

// STEP 2: Approve the tool call request and get final response
options.PreviousResponseId = response1.Id;
options.InputItems.Clear();
options.InputItems.Add(
    ResponseItem.CreateMcpApprovalResponseItem(approvalRequestItem!.Id, approved: true)
);
ResponseResult response2 = client.CreateResponse(options);

Console.WriteLine(response2.GetOutputText());