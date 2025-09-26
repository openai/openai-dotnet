// SAMPLE: Analyzes file from a file upload through Responses API
// PAGE: https://platform.openai.com/docs/guides/images-vision?api-mode=responses&format=file#analyze-images
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*
#:property PublishAot=false

using OpenAI.Files;
using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
OpenAIResponseClient client = new(model: "gpt-5", apiKey: key);

using var http = new HttpClient();

// Download an image as stream
using var stream = await http.GetStreamAsync("https://openai-documentation.vercel.app/images/cat_and_otter.png");

OpenAIFileClient files = new(key);
OpenAIFile file = await files.UploadFileAsync(BinaryData.FromStream(stream), "cat_and_otter.png", FileUploadPurpose.Vision);

OpenAIResponse response = (OpenAIResponse)client.CreateResponse([
    ResponseItem.CreateUserMessageItem([
        ResponseContentPart.CreateInputFilePart(file.Id),
        ResponseContentPart.CreateInputTextPart("what's in this image?")
    ])
]);

Console.WriteLine(response.GetOutputText());