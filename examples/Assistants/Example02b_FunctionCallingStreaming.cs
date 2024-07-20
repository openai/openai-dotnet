using NUnit.Framework;
using OpenAI.Assistants;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class AssistantExamples
{
    [Test]
    public async Task Example02b_FunctionCallingStreaming()
    {
        // This example parallels the content at the following location:
        // https://platform.openai.com/docs/assistants/tools/function-calling/function-calling-beta
        #region Step 1 - Define Functions
        
        // First, define the functions that the assistant will use in its defined tools.

        FunctionToolDefinition getTemperatureTool = new()
        {
            FunctionName = "get_current_temperature",
            Description = "Gets the current temperature at a specific location.",
            Parameters = BinaryData.FromString("""
            {
              "type": "object",
              "properties": {
                "location": {
                  "type": "string",
                  "description": "The city and state, e.g., San Francisco, CA"
                },
                "unit": {
                  "type": "string",
                  "enum": ["Celsius", "Fahrenheit"],
                  "description": "The temperature unit to use. Infer this from the user's location."
                }
              }
            }
            """),
        };

        FunctionToolDefinition getRainProbabilityTool = new()
        {
            FunctionName = "get_current_rain_probability",
            Description = "Gets the current forecasted probability of rain at a specific location,"
                + " represented as a percent chance in the range of 0 to 100.",
            Parameters = BinaryData.FromString("""
            {
              "type": "object",
              "properties": {
                "location": {
                  "type": "string",
                  "description": "The city and state, e.g., San Francisco, CA"
                }
              },
              "required": ["location"]
            }
            """),
        };

        #endregion

        // Assistants is a beta API and subject to change; acknowledge its experimental status by suppressing the matching warning.
#pragma warning disable OPENAI001
        AssistantClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        #region Create a new assistant with function tools
        // Create an assistant that can call the function tools.
        AssistantCreationOptions assistantOptions = new()
        {
            Name = "Example: Function Calling",
            Instructions =
                "Don't make assumptions about what values to plug into functions."
                + " Ask for clarification if a user request is ambiguous.",
            Tools = { getTemperatureTool, getRainProbabilityTool },
        };

        Assistant assistant = await client.CreateAssistantAsync("gpt-4-turbo", assistantOptions);
        #endregion

        #region Step 2 - Create a thread and add messages
        AssistantThread thread = await client.CreateThreadAsync();
        ThreadMessage message = await client.CreateMessageAsync(
            thread,
            MessageRole.User,
            [
                "What's the weather in San Francisco today and the likelihood it'll rain?"
            ]);
        #endregion

        #region Step 3 - Initiate a streaming run
        StreamingRunOperation runOperation = client.CreateRunStreaming(thread, assistant);
        IAsyncEnumerable<StreamingUpdate> updates = runOperation.GetUpdatesStreamingAsync();

        await foreach (StreamingUpdate update in updates)
        {
            if (update is RequiredActionUpdate requiredActionUpdate)
            {
                List<ToolOutput> outputsToSubmit = [];

                foreach (RequiredAction action in requiredActionUpdate.RequiredActions)
                {
                    if (action.FunctionName == getTemperatureTool.FunctionName)
                    {
                        outputsToSubmit.Add(new ToolOutput(action.ToolCallId, "57"));
                    }
                    else if (action.FunctionName == getRainProbabilityTool.FunctionName)
                    {
                        outputsToSubmit.Add(new ToolOutput(action.ToolCallId, "25%"));
                    }
                }

                await runOperation.SubmitToolOutputsToRunStreamingAsync(outputsToSubmit);
            }
            else if (update is MessageContentUpdate contentUpdate)
            {
                Console.Write(contentUpdate.Text);
            }
        }
        #endregion

        // Optionally, delete the resources for tidiness if no longer needed.
        RequestOptions noThrowOptions = new() { ErrorOptions = ClientErrorBehaviors.NoThrow };
        _ = await client.DeleteThreadAsync(thread.Id, noThrowOptions);
        _ = await client.DeleteAssistantAsync(assistant.Id, noThrowOptions);
    }
}
