// SAMPLE: Analyzes image by passing a base64-encoded image through Responses API
// PAGE: https://platform.openai.com/docs/guides/images-vision?api-mode=responses&format=base64-encoded#analyze-images
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*
#:property PublishAot=false

using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
OpenAIResponseClient client = new(model: "gpt-5", apiKey: key);

using var http = new HttpClient();

// Download an image as stream
using var stream = await http.GetStreamAsync("https://openai-documentation.vercel.app/images/cat_and_otter.png");

OpenAIResponse response1 = (OpenAIResponse)client.CreateResponse([
    ResponseItem.CreateUserMessageItem([
        ResponseContentPart.CreateInputTextPart("What is in this image?"),
        ResponseContentPart.CreateInputImagePart(BinaryData.FromStream(stream), "image/png")
    ])
]);

Console.WriteLine($"From image stream: {response1.GetOutputText()}");

// Download an image as byte array
byte[] bytes = await http.GetByteArrayAsync("https://openai-documentation.vercel.app/images/cat_and_otter.png");

OpenAIResponse response2 = (OpenAIResponse)client.CreateResponse([
    ResponseItem.CreateUserMessageItem([
        ResponseContentPart.CreateInputTextPart("What is in this image?"),
        ResponseContentPart.CreateInputImagePart(BinaryData.FromBytes(bytes), "image/png")
    ])
]);

Console.WriteLine($"From byte array: {response2.GetOutputText()}");

// Convert the byte array to a base64 string
string base64 = Convert.ToBase64String(bytes);

OpenAIResponse response3 = (OpenAIResponse)client.CreateResponse([
    ResponseItem.CreateUserMessageItem([
        ResponseContentPart.CreateInputTextPart("What is in this image?"),
        ResponseContentPart.CreateInputImagePart(BinaryData.FromString(base64), "image/png")
    ])
]);

Console.WriteLine($"From base64 string: {response3.GetOutputText()}");