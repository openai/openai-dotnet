// SAMPLE: Get information from web search by specifying user location through Responses API
// PAGE: https://platform.openai.com/docs/guides/tools-web-search?api-mode=responses#user-location
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*

using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
ResponsesClient client = new(key);

CreateResponseOptions options = new() { Model = "gpt-5.2" };
options.Tools.Add(
    ResponseTool.CreateWebSearchTool(
        userLocation: WebSearchToolLocation.CreateApproximateLocation(
            country: "GB",
            city: "London",
            region: "Granary Square"
        )
    )
);
options.InputItems.Add(
    ResponseItem.CreateUserMessageItem("What are the best restaurants near me?")
);

ResponseResult response = client.CreateResponse(options);

Console.WriteLine(response.GetOutputText());