// Generate text from a simple prompt using Chat Completions API (https://platform.openai.com/docs/guides/text?api-mode=chat)
// This file can be run as an app using .NET 10 Preview 4 or later using the following steps 
//  1. Set OPENAI_KEY environment variable to your OpenAI API key, never hardcode it.  
//  2. Run this command: type chat_simpleprompt.cs
//  3. Run this commnd: dotnet run chat_simpleprompt.cs 
#:package OpenAI@2.*
#:property PublishAot=false
using OpenAI.Chat;
string key = Environment.GetEnvironmentVariable("OPENAI_KEY")!;
ChatClient client = new (model: "gpt-4.1", apiKey: apiKey); 
ChatCompletion acompletion = client.CompleteChat("list all noble gases"); 
Console.WriteLine(acompletion.Content[0].Text);
