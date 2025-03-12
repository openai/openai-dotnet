using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Examples;

#pragma warning disable OPENAICUA001

public partial class ResponsesExamples
{
    [Test]
    public async Task Example01_CuaFlow()
    {
        OpenAIResponseClient client = new(
            model: "computer-use-preview",
            credential: new ApiKeyCredential(Environment.GetEnvironmentVariable("OPENAI_API_KEY")));

        OpenAIResponse response = await client.CreateResponseAsync(
            [ResponseItem.CreateUserMessageItem("Click on the OK button")],
            new ResponseCreationOptions()
            {
                Tools = { ResponseTool.CreateComputerTool(1024, 768, ComputerToolEnvironment.Windows) },
            });

        if (response.OutputItems.FirstOrDefault() is ComputerCallResponseItem computerCall)
        {
            if (computerCall.Action.Kind == ComputerCallActionKind.Screenshot)
            {
                Uri screenshotLink = new("https://uxmovement.com/wp-content/uploads/2011/05/left-to-right-mapping.png");

                response = await client.CreateResponseAsync(
                    [ResponseItem.CreateComputerCallOutputItem(computerCall.Id, [], screenshotLink)]);
            }
            else if (computerCall.Action.Kind == ComputerCallActionKind.Click)
            {
                Console.WriteLine($"Instruction from model: click");
            }
        }
    }
}
