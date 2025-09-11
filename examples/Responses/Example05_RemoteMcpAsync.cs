using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.Threading.Tasks;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class ResponseExamples
{
    [Test]
    public async Task Example05_RemoteMcpAsync()
    {
        // This is a dice rolling MCP server.
        string serverLabel = "dmcp";
        Uri serverUri = new Uri("https://dmcp-server.deno.dev/sse");

        McpToolCallApprovalPolicy approvalPolicy = new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.NeverRequireApproval);

        ResponseCreationOptions options = new()
        {
            Tools = {
                new McpTool(serverLabel, serverUri)
                {
                    ToolCallApprovalPolicy = approvalPolicy
                }
            }
        };

        OpenAIResponseClient client = new(model: "gpt-5", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        OpenAIResponse response = await client.CreateResponseAsync("Roll 2d4+1", options);

        Console.WriteLine($"[ASSISTANT]: {response.GetOutputText()}");
    }
}

#pragma warning restore OPENAI001