// SAMPLE: Analyzes file from a file upload through Responses API
// PAGE: https://platform.openai.com/docs/quickstart?input-type=file-upload#analyze-images-and-files
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*

using OpenAI.Files;
using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
ResponsesClient client = new(key);

OpenAIFileClient files = new(key);
OpenAIFile file = files.UploadFile("draconomicon.pdf", FileUploadPurpose.UserData);

ResponseResult response = client.CreateResponse("gpt-5.2", [
    ResponseItem.CreateUserMessageItem([
        ResponseContentPart.CreateInputFilePart(file.Id),
        ResponseContentPart.CreateInputTextPart("What is the first dragon in the book?")
    ])
]);

Console.WriteLine(response.GetOutputText());