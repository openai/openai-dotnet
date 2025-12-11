using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.Collections.Generic;
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
        List<ResponseItem> inputItems =
        [
            ResponseItem.CreateUserMessageItem("Roll 2d4+1"),
        ];

        CreateResponseOptions options = new(inputItems)
        {
            Tools = {
                new McpTool(serverLabel: "dmcp", serverUri: new Uri("https://dmcp-server.deno.dev/sse"))
                {
                    ServerDescription = "A Dungeons and Dragons MCP server to assist with dice rolling.",
                    ToolCallApprovalPolicy = new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.NeverRequireApproval)
                }
            }
        };

        ResponsesClient client = new(model: "gpt-5", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        ResponseResult response = await client.CreateResponseAsync(options);

        Console.WriteLine($"[ASSISTANT]: {response.GetOutputText()}");
    }
}

#pragma warning restore OPENAI001