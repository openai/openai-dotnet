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
    [Ignore("Compilation-only example.")]
    [Test]
    public async Task Example06_RemoteMcpAuthenticationAsync()
    {
        List<ResponseItem> inputItems =
        [
            ResponseItem.CreateUserMessageItem("Roll 2d4+1"),
        ];

        CreateResponseOptions options = new("gpt-5", inputItems)
        {
            Tools = {
                new McpTool(serverLabel: "stripe", serverUri: new Uri("https://mcp.stripe.com"))
                {
                    AuthorizationToken = Environment.GetEnvironmentVariable("STRIPE_OAUTH_ACCESS_TOKEN"),
                }
            }
        };

        ResponsesClient client = new(apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        ResponseResult response = await client.CreateResponseAsync(options);

        Console.WriteLine($"[ASSISTANT]: {response.GetOutputText()}");
    }
}

#pragma warning restore OPENAI001