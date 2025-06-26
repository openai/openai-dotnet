// SAMPLE: Generate text from a simple prompt
#:package OpenAI@2.2.*-*
#:property PublishAot=false

using OpenAI.Responses; 

string key = Environment.GetEnvironmentVariable("OPENAI_KEY")!;
OpenAIResponseClient client = new("gpt-4.1", key);
OpenAIResponse response = client.CreateResponse("Write a one-sentence bedtime story about a unicorn.");
Console.WriteLine(response.GetOutputText());