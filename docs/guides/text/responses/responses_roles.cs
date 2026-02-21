// SAMPLE: Generate text with messages using different roles
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*

using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
ResponsesClient client = new(key);

ResponseResult response = client.CreateResponse("gpt-5.2", [
    ResponseItem.CreateDeveloperMessageItem("Talk like a pirate."),
    ResponseItem.CreateUserMessageItem("Are semicolons optional in JavaScript?")
]);

Console.WriteLine(response.GetOutputText());