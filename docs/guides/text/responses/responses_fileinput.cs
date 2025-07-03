// SAMPLE: Prompt template with file input variable
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start 
#:package OpenAI@2.2.*-*
#:property PublishAot false

using OpenAI.Responses;
using OpenAI.Files;
using System.ClientModel;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
OpenAIResponseClient client = new("gpt-4.1", key);

// Upload a PDF we will reference in the variables
OpenAIFileClient files = new(key);
OpenAIFile file = files.UploadFile("draconomicon.pdf", FileUploadPurpose.UserData);

OpenAIResponse response = (OpenAIResponse)client.CreateResponse(
    BinaryContent.Create(BinaryData.FromObjectAsJson(
    new {
        model = "gpt-4.1",
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