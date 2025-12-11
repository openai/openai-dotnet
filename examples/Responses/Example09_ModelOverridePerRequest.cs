using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.Collections.Generic;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

public partial class ResponseExamples
{
    [Test]
    public void Example09_ModelOverridePerRequest()
    {
        ResponsesClient client = new(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        List<ResponseItem> inputItems =
        [
            ResponseItem.CreateUserMessageItem("Say 'this is a test."),
        ];

        // Add extra request fields using Patch.
        // Patch lets you set fields like `model` that aren't exposed on CreateResponseOptions.
        // This overrides the model set on the client just for the request where this options instance is used.
        // See the API docs https://platform.openai.com/docs/api-reference/responses/create for supported additional fields.
        CreateResponseOptions options = new(inputItems); // Model can also be set via constructor
        options.Patch.Set("$.model"u8, "gpt-5");

        ResponseResult response = client.CreateResponse(options);

        Console.WriteLine($"[ASSISTANT]: {response.GetOutputText()}, [Model]: {response.Model}");
    }
}

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
#pragma warning restore OPENAI001
