using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class ChatExamples
{
    // See Example03_FunctionCalling.cs for the tool and function definitions.

    [Test]
    public async Task Example03_FunctionCallingAsync()
    {
        ChatClient client = new("gpt-4-turbo", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        #region
        List<ChatMessage> messages =
        [
            new UserChatMessage("What's the weather like today?"),
        ];

        ChatCompletionOptions options = new()
        {
            Tools = { getCurrentLocationTool, getCurrentWeatherTool },
        };
        #endregion

        #region
        bool requiresAction;

        do
        {
            requiresAction = false;
            ChatCompletion completion = await client.CompleteChatAsync(messages, options);

            switch (completion.FinishReason)
            {
                case ChatFinishReason.Stop:
                    {
                        // Add the assistant message to the conversation history.
                        messages.Add(new AssistantChatMessage(completion));
                        break;
                    }

                case ChatFinishReason.ToolCalls:
                    {
                        // First, add the assistant message with tool calls to the conversation history.
                        messages.Add(new AssistantChatMessage(completion));

                        // Then, add a new tool message for each tool call that is resolved.
                        foreach (ChatToolCall toolCall in completion.ToolCalls)
                        {
                            switch (toolCall.FunctionName)
                            {
                                case nameof(GetCurrentLocation):
                                    {
                                        string toolResult = GetCurrentLocation();
                                        messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                                        break;
                                    }

                                case nameof(GetCurrentWeather):
                                    {
                                        // The arguments that the model wants to use to call the function are specified as a
                                        // stringified JSON object based on the schema defined in the tool definition. Note that
                                        // the model may hallucinate arguments too. Consequently, it is important to do the
                                        // appropriate parsing and validation before calling the function.
                                        using JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
                                        bool hasLocation = argumentsJson.RootElement.TryGetProperty("location", out JsonElement location);
                                        bool hasUnit = argumentsJson.RootElement.TryGetProperty("unit", out JsonElement unit);

                                        if (!hasLocation)
                                        {
                                            throw new ArgumentNullException(nameof(location), "The location argument is required.");
                                        }

                                        string toolResult = hasUnit
                                            ? GetCurrentWeather(location.GetString(), unit.GetString())
                                            : GetCurrentWeather(location.GetString());
                                        messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                                        break;
                                    }

                                default:
                                    {
                                        // Handle other unexpected calls.
                                        throw new NotImplementedException();
                                    }
                            }
                        }

                        requiresAction = true;
                        break;
                    }

                case ChatFinishReason.Length:
                    throw new NotImplementedException("Incomplete model output due to MaxTokens parameter or token limit exceeded.");

                case ChatFinishReason.ContentFilter:
                    throw new NotImplementedException("Omitted content due to a content filter flag.");

                case ChatFinishReason.FunctionCall:
                    throw new NotImplementedException("Deprecated in favor of tool calls.");

                default:
                    throw new NotImplementedException(completion.FinishReason.ToString());
            }
        } while (requiresAction);
        #endregion

        #region
        foreach (ChatMessage message in messages)
        {
            switch (message)
            {
                case UserChatMessage userMessage:
                    Console.WriteLine($"[USER]:");
                    Console.WriteLine($"{userMessage.Content[0].Text}");
                    Console.WriteLine();
                    break;

                case AssistantChatMessage assistantMessage when assistantMessage.Content.Count > 0:
                    Console.WriteLine($"[ASSISTANT]:");
                    Console.WriteLine($"{assistantMessage.Content[0].Text}");
                    Console.WriteLine();
                    break;

                case ToolChatMessage:
                    // Do not print any tool messages; let the assistant summarize the tool results instead.
                    break;

                default:
                    break;
            }
        }
        #endregion
    }
}
