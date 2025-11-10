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
    public void Example06_RemoteMcpAuthentication()
    {
        CreateResponseOptions options = new([
            ResponseItem.CreateUserMessageItem("Create a payment link for $20")])
        {
            Tools = {
                new McpTool(serverLabel: "stripe", serverUri: new Uri("https://mcp.stripe.com"))
                {
                    AuthorizationToken = Environment.GetEnvironmentVariable("STRIPE_OAUTH_ACCESS_TOKEN"),
                }
            }
        };

        OpenAIResponseClient client = new(model: "gpt-5", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        ResponseResult response = client.CreateResponse(options);

        Console.WriteLine($"[ASSISTANT]: {response.OutputText}");
    }
}

#pragma warning restore OPENAI001