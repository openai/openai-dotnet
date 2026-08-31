// SAMPLE: Generate streaming responses through Responses API
// PAGE: https://platform.openai.com/docs/guides/streaming-responses#enable-streaming
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001
#pragma warning disable SCME0005

#:package System.Linq.Async@6.*
#:package OpenAI@2.*

using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
ResponsesClient client = new(key);

await using var responses = await client.CreateResponseStreamingAsync(
    "gpt-5.2",
    "Say 'double bubble bath' ten times fast."
);

await foreach (var response in responses)
{
    if (response is StreamingResponseOutputTextDeltaUpdate delta)
    {
        Console.Write(delta.Delta);
    }
}
