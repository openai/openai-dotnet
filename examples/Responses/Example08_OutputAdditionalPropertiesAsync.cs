using NUnit.Framework;
using System;
using OpenAI.Responses;
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
        OpenAIResponseClient client = new(model: "gpt-5", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        ResponseCreationOptions options = new()
        {
            Tools =
            {
                ResponseTool.CreateImageGenerationTool(
                    model: "gpt-image-1",
                    outputFileFormat: ImageGenerationToolOutputFileFormat.Png,
                    inputFidelityLevel: ImageGenerationToolInputFidelityLevel.High)
            }
        };

        OpenAIResponse response = await client.CreateResponseAsync("Generate an image of gray tabby cat hugging an otter with an orange scarf", options);
        ImageGenerationCallResponseItem imageGenResponse = (ImageGenerationCallResponseItem)response.OutputItems[1];
        BinaryData bytes = imageGenResponse.GeneratedImageBytes;

        using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.png");
        bytes.ToStream().CopyTo(stream);

        // You can use the Patch property to read additional properties from the response
        var outputFormat = imageGenResponse.Patch.GetString("$.output_format"u8);
        var quality = imageGenResponse.Patch.GetString("$.quality"u8);
        var size = imageGenResponse.Patch.GetString("$.size"u8);
        Console.WriteLine($"output_format={outputFormat}, quality={quality}, size={size}");
    }
}

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
#pragma warning restore OPENAI001
