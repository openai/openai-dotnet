// SAMPLE: Generate images with Responses API
// PAGE: https://platform.openai.com/docs/guides/images-vision?api-mode=responses#generate-or-edit-images
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*
#:property PublishAot=false

using System.ClientModel;
using System.Text.Json;
using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
OpenAIResponseClient client = new(model: "gpt-image-1", apiKey: key);

OpenAIResponse response = (OpenAIResponse)client.CreateResponse(
        BinaryContent.Create(BinaryData.FromObjectAsJson(new
        {
            model = "gpt-5",
            input = "Generate an image of gray tabby cat hugging an otter with an orange scarf",
            tools = new[]
            {
                new
                {
                    type = "image_generation"
                }
            }
        }
    ))
);

var serialised = JsonSerializer.Serialize(response.OutputItems);

Console.WriteLine(serialised);