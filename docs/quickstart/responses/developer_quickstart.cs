// SAMPLE: Generate text from a simple prompt through Responses API
// PAGE: https://platform.openai.com/docs/quickstart
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*

using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
ResponsesClient client = new(key);

ResponseResult response = client.CreateResponse(
    "gpt-5.2",
    "Write a one-sentence bedtime story about a unicorn."
);

Console.WriteLine(response.GetOutputText());