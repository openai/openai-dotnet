// SAMPLE: Get information from file search through Responses API
// PAGE: https://platform.openai.com/docs/guides/tools?tool-type=file-search
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*

using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
ResponsesClient client = new(key);

CreateResponseOptions options = new() { Model = "gpt-5.2" };
options.Tools.Add(
    ResponseTool.CreateFileSearchTool([ "<vector_store_id>" ])
);
options.InputItems.Add(
    ResponseItem.CreateUserMessageItem("What is deep research by OpenAI?")
);

ResponseResult response = client.CreateResponse(options);

Console.WriteLine(response.GetOutputText());