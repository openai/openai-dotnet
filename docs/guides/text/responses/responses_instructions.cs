// SAMPLE: Generate text with instructions
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start 
#:package OpenAI@2.2.*-*
#:property PublishAot false

using OpenAI.Responses; 

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
OpenAIResponseClient client = new("gpt-4.1", key);
OpenAIResponse response = client.CreateResponse(
    "Are semicolons optional in JavaScript?",
    new() { Instructions = "Talk like a pirate." }
);


Console.WriteLine(response.GetOutputText());