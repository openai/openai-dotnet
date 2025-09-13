// SAMPLE: Generate text from a simple prompt through Responses API
// PAGE: https://platform.openai.com/docs/quickstart
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*
#:property PublishAot=false

using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
OpenAIResponseClient client = new(model: "gpt-5", apiKey: key);
OpenAIResponse response = client.CreateResponse("Write a one-sentence bedtime story about a unicorn.");

Console.WriteLine(response.GetOutputText());