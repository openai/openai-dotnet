// SAMPLE: Generate response from remote MCP with no approval through Responses API
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
    serverLabel: "deepwiki",
    serverUri: new Uri("https://mcp.deepwiki.com/mcp"),
    allowedTools: new McpToolFilter() { ToolNames = { "ask_question", "read_wiki_structure" } },
    toolCallApprovalPolicy: new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.NeverRequireApproval)
));

OpenAIResponse response = client.CreateResponse([
    ResponseItem.CreateUserMessageItem([
        ResponseContentPart.CreateInputTextPart("What transport protocols does the 2025-03-26 version of the MCP spec (modelcontextprotocol/modelcontextprotocol) support?")
    ])
], options);

Console.WriteLine(response.GetOutputText());