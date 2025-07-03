// SAMPLE: Generate text with a prompt template
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start 
#:package OpenAI@2.2.*-*
#:property PublishAot false

using OpenAI.Responses;
using System.ClientModel;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
OpenAIResponseClient client = new("gpt-4.1", key);
OpenAIResponse response = (OpenAIResponse)client.CreateResponse(
    BinaryContent.Create(BinaryData.FromObjectAsJson(
    new {
        model = "gpt-4.1",
        prompt = new {
            id = "pmpt_abc123",
            version = "2",
            variables = new {
                customer_name = "Jane Doe",
                product = "40oz juice box"
            }
        }
    }
)));
Console.WriteLine(response.GetOutputText());