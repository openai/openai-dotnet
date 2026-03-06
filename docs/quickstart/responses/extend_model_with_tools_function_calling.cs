// SAMPLE: Generate response from function calling through Responses API
// PAGE: https://platform.openai.com/docs/quickstart?tool-type=function-calling#extend-the-model-with-tools
// GUIDANCE: Instructions to run this code: https://aka.ms/oai/net/start
#pragma warning disable OPENAI001

#:package OpenAI@2.*
#:property PublishAot=false

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using OpenAI.Responses;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
ResponsesClient client = new(key);

CreateResponseOptions options = new() { Model = "gpt-5.2" };
options.Tools.Add(
    ResponseTool.CreateFunctionTool(
        functionName: "get_weather",
        functionDescription: "Get current temperature for a given location.",
                functionParameters: BinaryData.FromString(
                        """
                        {
                            "type": "object",
                            "properties": {
                                "location": {
                                    "type": "string",
                                    "description": "City and country e.g. Bogotá, Colombia"
                                }
                            },
                            "required": ["location"],
                            "additionalProperties": false
                        }
                        """),
        strictModeEnabled: true
    )
);
options.InputItems.Add(
    ResponseItem.CreateUserMessageItem("What is the weather like in Paris today?")
);

ResponseResult response = client.CreateResponse(options);
JsonSerializerOptions serializerOptions = new()
{
    TypeInfoResolver = new DefaultJsonTypeInfoResolver()
};

Console.WriteLine(JsonSerializer.Serialize(response.OutputItems[0], serializerOptions));
