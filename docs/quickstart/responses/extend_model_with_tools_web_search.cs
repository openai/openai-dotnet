// SAMPLE: Get information from web search through Responses API
// PAGE: https://platform.openai.com/docs/quickstart?tool-type=web-search#extend-the-model-with-tools
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*

using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
ResponsesClient client = new(key);

CreateResponseOptions options = new() { Model = "gpt-5.2" };
options.Tools.Add(
    ResponseTool.CreateWebSearchTool()
);
options.InputItems.Add(
    ResponseItem.CreateUserMessageItem("What was a positive news story from today?")
);

ResponseResult response = client.CreateResponse(options);

Console.WriteLine(response.GetOutputText());