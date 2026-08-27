using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using OpenAI.Responses;

#pragma warning disable OPENAI001
#pragma warning disable SCME0002

namespace OpenAI.Tests.Snippets;

[Explicit("These tests are used for compile-time verification of code snippets used in READMEs accompanying sample apps in the examples directory. There is little value in executing them for test runs.")]
[Category("Snippets")]
[TestFixture]
public class ExampleSnippets
{
    [Test]
    public void AspNetCore_DependencyInjection_Register()
    {
        var builderMock = new Mock<IHostApplicationBuilder>();
        var builder = builderMock.Object;

        #region Snippet:Add_Responses_Client
        builder.AddResponsesClient("Clients:ResponsesClient");
        #endregion
    }

    [Test]
    public void AspNetCore_DependencyInjection_Usage()
    {
        WebApplication app = WebApplication.CreateBuilder(Array.Empty<string>()).Build();

        #region Snippet:Responses_Create_Endpoint
        app.MapPost("/responses/create",
            async (ResponsesRequest request, ResponsesClient client, IConfiguration configuration) =>
        {
            string model = configuration["Clients:ResponsesClient:Model"]
                ?? throw new InvalidOperationException("Model not configured at Clients:ResponsesClient:Model.");
            ResponseResult response = await client.CreateResponseAsync(model, request.Message);
            return new ResponsesResponse(response.GetOutputText());
        });
        #endregion
    }
}

public record ResponsesRequest(string Message);

public record ResponsesResponse(string Response);
