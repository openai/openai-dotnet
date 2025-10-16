// SAMPLE: Get information from file search through Responses API
// PAGE: https://platform.openai.com/docs/guides/tools?tool-type=file-search
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*
#:property PublishAot=false

using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
OpenAIResponseClient client = new(model: "gpt-5", apiKey: key);

ResponseCreationOptions options = new();
options.Tools.Add(ResponseTool.CreateFileSearchTool([ "<vector_store_id>" ]));

OpenAIResponse response = client.CreateResponse([
    ResponseItem.CreateUserMessageItem([
        ResponseContentPart.CreateInputTextPart("What is deep research by OpenAI?")
    ])
], options);

Console.WriteLine(response.GetOutputText());