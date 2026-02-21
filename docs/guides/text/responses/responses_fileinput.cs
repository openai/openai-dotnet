// SAMPLE: Prompt template with file input variable
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*
#:property PublishAot=false

using System.ClientModel;
using OpenAI.Responses;
using OpenAI.Files;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
ResponsesClient client = new(key);

// Upload a PDF we will reference in the variables
OpenAIFileClient files = new(key);
OpenAIFile file = files.UploadFile("draconomicon.pdf", FileUploadPurpose.UserData);

ResponseResult response = (ResponseResult)client.CreateResponse(
    BinaryContent.Create(BinaryData.FromObjectAsJson(
    new {
        model = "gpt-5.2",
        prompt = new {
            id = "pmpt_abc123",
            variables = new {
                topic = "Dragons",
                reference_pdf = new {
                    type = "input_file",
                    file_id = file.Id,
                }
            }
        }
    }
)));

Console.WriteLine(response.GetOutputText());