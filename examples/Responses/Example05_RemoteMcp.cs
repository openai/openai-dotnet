using NUnit.Framework;
using OpenAI.Responses;
using System;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class ResponseExamples
{
    [Test]
    public void Example05_RemoteMcp()
    {
        CreateResponseOptions options = new(
            "gpt-5",
            [
                ResponseItem.CreateUserMessageItem("Roll 2d4+1")
            ])
            {
                Tools = {
                    new McpTool(serverLabel: "dmcp", serverUri: new Uri("https://dmcp-server.deno.dev/sse"))
                    {
                        ServerDescription = "A Dungeons and Dragons MCP server to assist with dice rolling.",
                        ToolCallApprovalPolicy = new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.NeverRequireApproval)
                    }
                }
            };

        ResponsesClient client = new(apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        ResponseResult response = client.CreateResponse(options);

        Console.WriteLine($"[ASSISTANT]: {response.GetOutputText()}");
    }
}

#pragma warning restore OPENAI001