// SAMPLE: Generate text from a simple prompt through Responses API
// PAGE: https://platform.openai.com/docs/overview
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*

using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
ResponsesClient client = new(model: "gpt-5.2", apiKey: key);
ResponseResult response = client.CreateResponse("Write a short bedtime story about a unicorn.");

Console.WriteLine(response.GetOutputText());
