// SAMPLE: Analyzes image by passing a base64-encoded image through Responses API
// PAGE: https://platform.openai.com/docs/guides/images-vision?api-mode=responses&format=base64-encoded#analyze-images
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*

using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
ResponsesClient client = new(key);

Uri imageUrl = new("https://openai-documentation.vercel.app/images/cat_and_otter.png");
using HttpClient http = new();

// Download an image as stream
using var stream = await http.GetStreamAsync(imageUrl);

ResponseResult response1 = client.CreateResponse("gpt-5.2", [
    ResponseItem.CreateUserMessageItem([
        ResponseContentPart.CreateInputTextPart("What is in this image?"),
        ResponseContentPart.CreateInputImagePart(BinaryData.FromStream(stream), "image/png")
    ])
]);

Console.WriteLine($"From image stream: {response1.GetOutputText()}");

// Download an image as byte array
byte[] bytes = await http.GetByteArrayAsync(imageUrl);

ResponseResult response2 = client.CreateResponse("gpt-5.2", [
    ResponseItem.CreateUserMessageItem([
        ResponseContentPart.CreateInputTextPart("What is in this image?"),
        ResponseContentPart.CreateInputImagePart(BinaryData.FromBytes(bytes), "image/png")
    ])
]);

Console.WriteLine($"From byte array: {response2.GetOutputText()}");
