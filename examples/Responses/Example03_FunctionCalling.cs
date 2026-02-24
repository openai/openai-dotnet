using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class ResponseExamples
{
    #region
    private static string GetCurrentLocation()
    {
        // Call the location API here.
        return "San Francisco";
    }

    private static string GetCurrentWeather(string location, string unit = "celsius")
    {
        // Call the weather API here.
        return $"31 {unit}";
    }
    #endregion

    #region
    private static readonly FunctionTool getCurrentLocationTool = ResponseTool.CreateFunctionTool(
        functionName: nameof(GetCurrentLocation),
        functionDescription: "Get the user's current location",
        functionParameters: null,
        strictModeEnabled: false
    );

    private static readonly FunctionTool getCurrentWeatherTool = ResponseTool.CreateFunctionTool(
        functionName: nameof(GetCurrentWeather),
        functionDescription: "Get the current weather in a given location",
        functionParameters: BinaryData.FromBytes("""
            {
                "type": "object",
                "properties": {
                    "location": {
                        "type": "string",
                        "description": "The city and state, e.g. Boston, MA"
                    },
                    "unit": {
                        "type": "string",
                        "enum": [ "celsius", "fahrenheit" ],
                        "description": "The temperature unit to use. Infer this from the specified location."
                    }
                },
                "required": [ "location" ]
            }
            """u8.ToArray()),
        strictModeEnabled: false
    );
    #endregion

    [Test]
    public void Example03_FunctionCalling()
    {
        ResponsesClient client = new(apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        List<ResponseItem> inputItems =
        [
            ResponseItem.CreateUserMessageItem("What's the weather like today for my current location?"),
        ];

        PrintMessageItems(inputItems.OfType<MessageResponseItem>());

        bool requiresAction;

        do
        {
            requiresAction = false;

            CreateResponseOptions options = new("gpt-5-mini", inputItems)
            {
                Tools = { getCurrentLocationTool, getCurrentWeatherTool },
            };

            ResponseResult response = client.CreateResponse(options);

            inputItems.AddRange(response.OutputItems);

            foreach (ResponseItem outputItem in response.OutputItems)
            {
                if (outputItem is FunctionCallResponseItem functionCall)
                {
                    switch (functionCall.FunctionName)
                    {
                        case nameof(GetCurrentLocation):
                            {
                                string functionOutput = GetCurrentLocation();
                                inputItems.Add(new FunctionCallOutputResponseItem(functionCall.CallId, functionOutput));
                                break;
                            }

                        case nameof(GetCurrentWeather):
                            {
                                // The arguments that the model wants to use to call the function are specified as a
                                // stringified JSON object based on the schema defined in the tool definition. Note that
                                // the model may hallucinate arguments too. Consequently, it is important to do the
                                // appropriate parsing and validation before calling the function.
                                using JsonDocument argumentsJson = JsonDocument.Parse(functionCall.FunctionArguments);
                                bool hasLocation = argumentsJson.RootElement.TryGetProperty("location", out JsonElement location);
                                bool hasUnit = argumentsJson.RootElement.TryGetProperty("unit", out JsonElement unit);

                                if (!hasLocation)
                                {
                                    throw new ArgumentNullException(nameof(location), "The location argument is required.");
                                }

                                string functionOutput = hasUnit
                                    ? GetCurrentWeather(location.GetString(), unit.GetString())
                                    : GetCurrentWeather(location.GetString());
                                inputItems.Add(new FunctionCallOutputResponseItem(functionCall.CallId, functionOutput));
                                break;
                            }

                        default:
                            {
                                // Handle other unexpected calls.
                                throw new NotImplementedException();
                            }
                    }

                    requiresAction = true;
                    break;
                }
            }

            PrintMessageItems(response.OutputItems.OfType<MessageResponseItem>());

        } while (requiresAction);
    }

    private void PrintMessageItems(IEnumerable<ResponseItem> messageItems)
    {
        foreach (MessageResponseItem messageItem in messageItems)
        {
            switch (messageItem.Role)
            {
                case MessageRole.User:
                    Console.WriteLine($"[USER]:");
                    Console.WriteLine($"{messageItem.Content[0].Text}");
                    Console.WriteLine();
                    break;

                case MessageRole.Assistant:
                    Console.WriteLine($"[ASSISTANT]:");
                    Console.WriteLine($"{messageItem.Content[0].Text}");
                    Console.WriteLine();
                    break;

                default:
                    break;
            }
        }
    }
}

#pragma warning restore OPENAI001