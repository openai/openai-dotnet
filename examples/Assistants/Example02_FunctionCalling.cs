using NUnit.Framework;
using OpenAI.Assistants;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class AssistantExamples
{
    [Test]
    public void Example02_FunctionCalling()
    {
        #region
        string GetCurrentLocation()
        {
            // Call a location API here.
            return "San Francisco";
        }

        const string GetCurrentLocationFunctionName = "get_current_location";

        FunctionToolDefinition getLocationTool = new(GetCurrentLocationFunctionName)
        {
            Description = "Get the user's current location"
        };

        string GetCurrentWeather(string location, string unit = "celsius")
        {
            // Call a weather API here.
            return $"31 {unit}";
        }

        const string GetCurrentWeatherFunctionName = "get_current_weather";

        FunctionToolDefinition getWeatherTool = new(GetCurrentWeatherFunctionName)
        {
            Description = "Get the current weather in a given location",
            Parameters = BinaryData.FromString("""
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
            """),
        };
        #endregion

        // Assistants is a beta API and subject to change; acknowledge its experimental status by suppressing the matching warning.
        AssistantClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        #region
        // Create an assistant that can call the function tools.
        AssistantCreationOptions assistantOptions = new()
        {
            Name = "Example: Function Calling",
            Instructions =
                "Don't make assumptions about what values to plug into functions."
                + " Ask for clarification if a user request is ambiguous.",
            Tools = { getLocationTool, getWeatherTool },
        };

        Assistant assistant = client.CreateAssistant("gpt-4-turbo", assistantOptions);
        #endregion

        #region
        // Create a thread with an initial user message and run it.
        ThreadCreationOptions threadOptions = new()
        {
            InitialMessages = { "What's the weather like today?" }
        };

        ThreadRun run = client.CreateThreadAndRun(assistant.Id, threadOptions);
        #endregion

        #region
        // Poll the run until it is no longer queued or in progress.
        while (!run.Status.IsTerminal)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            run = client.GetRun(run.ThreadId, run.Id);

            // If the run requires action, resolve them.
            if (run.Status == RunStatus.RequiresAction)
            {
                List<ToolOutput> toolOutputs = [];

                foreach (RequiredAction action in run.RequiredActions)
                {
                    switch (action.FunctionName)
                    {
                        case GetCurrentLocationFunctionName:
                            {
                                string toolResult = GetCurrentLocation();
                                toolOutputs.Add(new ToolOutput(action.ToolCallId, toolResult));
                                break;
                            }

                        case GetCurrentWeatherFunctionName:
                            {
                                // The arguments that the model wants to use to call the function are specified as a
                                // stringified JSON object based on the schema defined in the tool definition. Note that
                                // the model may hallucinate arguments too. Consequently, it is important to do the
                                // appropriate parsing and validation before calling the function.
                                using JsonDocument argumentsJson = JsonDocument.Parse(action.FunctionArguments);
                                bool hasLocation = argumentsJson.RootElement.TryGetProperty("location", out JsonElement location);
                                bool hasUnit = argumentsJson.RootElement.TryGetProperty("unit", out JsonElement unit);

                                if (!hasLocation)
                                {
                                    throw new ArgumentNullException(nameof(location), "The location argument is required.");
                                }

                                string toolResult = hasUnit
                                    ? GetCurrentWeather(location.GetString(), unit.GetString())
                                    : GetCurrentWeather(location.GetString());
                                toolOutputs.Add(new ToolOutput(action.ToolCallId, toolResult));
                                break;
                            }

                        default:
                            {
                                // Handle other or unexpected calls.
                                throw new NotImplementedException();
                            }
                    }
                }

                // Submit the tool outputs to the assistant, which returns the run to the queued state.
                run = client.SubmitToolOutputsToRun(run.ThreadId, run.Id, toolOutputs);
            }
        }
        #endregion

        #region
        // With the run complete, list the messages and display their content
        if (run.Status == RunStatus.Completed)
        {
            CollectionResult<ThreadMessage> messages
                = client.GetMessages(run.ThreadId, new MessageCollectionOptions() { Order = MessageCollectionOrder.Ascending });

            foreach (ThreadMessage message in messages)
            {
                Console.WriteLine($"[{message.Role.ToString().ToUpper()}]: ");
                foreach (MessageContent contentItem in message.Content)
                {
                    Console.WriteLine($"{contentItem.Text}");

                    if (contentItem.ImageFileId is not null)
                    {
                        Console.WriteLine($" <Image File ID> {contentItem.ImageFileId}");
                    }

                    // Include annotations, if any.
                    if (contentItem.TextAnnotations.Count > 0)
                    {
                        Console.WriteLine();
                        foreach (TextAnnotation annotation in contentItem.TextAnnotations)
                        {
                            Console.WriteLine($"* File ID used by file_search: {annotation.InputFileId}");
                            Console.WriteLine($"* File ID created by code_interpreter: {annotation.OutputFileId}");
                            Console.WriteLine($"* Text to replace: {annotation.TextToReplace}");
                            Console.WriteLine($"* Message content index range: {annotation.StartIndex}-{annotation.EndIndex}");
                        }
                    }

                }
                Console.WriteLine();
            }
        }
        else
        {
            throw new NotImplementedException(run.Status.ToString());
        }
        #endregion
    }
}

#pragma warning restore OPENAI001