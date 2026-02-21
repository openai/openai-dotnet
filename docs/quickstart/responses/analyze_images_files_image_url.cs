// SAMPLE: Analyzes image from an image URL through Responses API
// PAGE: https://platform.openai.com/docs/quickstart?input-type=image-url#analyze-images-and-files
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*

using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
ResponsesClient client = new(key);

ResponseResult response = client.CreateResponse("gpt-5.2", [
    ResponseItem.CreateUserMessageItem([
        ResponseContentPart.CreateInputTextPart("What is in this image?"),
        ResponseContentPart.CreateInputImagePart(new Uri("https://openai-documentation.vercel.app/images/cat_and_otter.png"))
    ])
]);

Console.WriteLine(response.GetOutputText());