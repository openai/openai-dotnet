using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

public partial class ResponseExamples
{
    [Test]
    public async Task Example08_OutputAdditionalPropertiesAsync()
    {
        ResponsesClient client = new(apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        List<ResponseItem> inputItems =
        [
            ResponseItem.CreateUserMessageItem("Generate an image of gray tabby cat hugging an otter with an orange scarf."),
        ];

        CreateResponseOptions options = new("gpt-5", inputItems)
        {
            Tools =
            {
                ResponseTool.CreateImageGenerationTool(
                    model: "gpt-image-1",
                    outputFileFormat: ImageGenerationToolOutputFileFormat.Png,
                    inputFidelity: ImageGenerationToolInputFidelity.High)
            }
        };

        ResponseResult response = await client.CreateResponseAsync(options);
        ImageGenerationCallResponseItem imageGenResponse = (ImageGenerationCallResponseItem)response.OutputItems[1];
        BinaryData bytes = imageGenResponse.ImageResultBytes;

        using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.png");
        bytes.ToStream().CopyTo(stream);

        // Read extra fields from the response via Patch.
        // The service returns fields like `output_format` and `quality` that aren’t modeled on ImageGenerationCallResponseItem.
        // You can access their values by using the path with Patch.
        // See the API docs https://platform.openai.com/docs/api-reference/responses/object for supported additional fields.
        var outputFormat = imageGenResponse.Patch.GetString("$.output_format"u8);
        var quality = imageGenResponse.Patch.GetString("$.quality"u8);
        var size = imageGenResponse.Patch.GetString("$.size"u8);
        Console.WriteLine($"output_format={outputFormat}, quality={quality}, size={size}");
    }
}

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
#pragma warning restore OPENAI001
