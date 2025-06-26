// SAMPLE: Generate text from a simple prompt
#:package OpenAI@2.2.*-*
#:property PublishAot=false

using OpenAI.Chat; 

string key = Environment.GetEnvironmentVariable("OPENAI_KEY")!;
ChatClient client = new("gpt-4.1", key);
ChatCompletion acompletion = client.CompleteChat("Write a one-sentence bedtime story about a unicorn.");
Console.WriteLine(acompletion.Content[0].Text);