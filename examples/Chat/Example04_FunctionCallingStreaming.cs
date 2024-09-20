using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OpenAI.Examples;

public partial class ChatExamples
{
    // See Example03_FunctionCalling.cs for the tool and function definitions.

    [Test]
    public void Example04_FunctionCallingStreaming()
    {
        ChatClient client = new("gpt-4-turbo", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        #region
        List<ChatMessage> messages = [
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
            Dictionary<int, string> indexToToolCallId = [];
            Dictionary<int, string> indexToFunctionName = [];
            Dictionary<int, StringBuilder> indexToFunctionArguments = [];
            StringBuilder contentBuilder = new();
            CollectionResult<StreamingChatCompletionUpdate> chatUpdates
                = client.CompleteChatStreaming(messages, options);

            foreach (StreamingChatCompletionUpdate chatUpdate in chatUpdates)
            {
                // Accumulate the text content as new updates arrive.
                foreach (ChatMessageContentPart contentPart in chatUpdate.ContentUpdate)
                {
                    contentBuilder.Append(contentPart.Text);
                }

                // Build the tool calls as new updates arrive.
                foreach (StreamingChatToolCallUpdate toolCallUpdate in chatUpdate.ToolCallUpdates)
                {
                    // Keep track of which tool call ID belongs to this update index.
                    if (toolCallUpdate.Id is not null)
                    {
                        indexToToolCallId[toolCallUpdate.Index] = toolCallUpdate.Id;
                    }

                    // Keep track of which function name belongs to this update index.
                    if (toolCallUpdate.FunctionName is not null)
                    {
                        indexToFunctionName[toolCallUpdate.Index] = toolCallUpdate.FunctionName;
                    }

                    // Keep track of which function arguments belong to this update index,
                    // and accumulate the arguments string as new updates arrive.
                    if (toolCallUpdate.FunctionArgumentsUpdate is not null)
                    {
                        StringBuilder argumentsBuilder
                            = indexToFunctionArguments.TryGetValue(toolCallUpdate.Index, out StringBuilder existingBuilder)
                                ? existingBuilder
                                : new StringBuilder();
                        argumentsBuilder.Append(toolCallUpdate.FunctionArgumentsUpdate);
                        indexToFunctionArguments[toolCallUpdate.Index] = argumentsBuilder;
                    }
                }

                switch (chatUpdate.FinishReason)
                {
                    case ChatFinishReason.Stop:
                        {
                            // Add the assistant message to the conversation history.
                            messages.Add(new AssistantChatMessage(contentBuilder.ToString()));
                            break;
                        }

                    case ChatFinishReason.ToolCalls:
                        {
                            // First, collect the accumulated function arguments into complete tool calls to be processed
                            List<ChatToolCall> toolCalls = [];
                            foreach ((int index, string toolCallId) in indexToToolCallId)
                            {
                                ChatToolCall toolCall = ChatToolCall.CreateFunctionToolCall(
                                    toolCallId,
                                    indexToFunctionName[index],
                                    indexToFunctionArguments[index].ToString());

                                toolCalls.Add(toolCall);
                            }

                            // Next, add the assistant message with tool calls to the conversation history.
                            var assistantChatMessage = new AssistantChatMessage(toolCalls);
                            string content = contentBuilder.Length > 0 ? contentBuilder.ToString() : null;
                            if (content != null)
                            {
                                assistantChatMessage.Content.Add(ChatMessageContentPart.CreateTextPart(content));
                            }
                            messages.Add(assistantChatMessage);

                            // Then, add a new tool message for each tool call to be resolved.
                            foreach (ChatToolCall toolCall in toolCalls)
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

                    case null:
                        break;
                }
            }
        } while (requiresAction);
        #endregion

        #region
        foreach (ChatMessage requestMessage in messages)
        {
            switch (requestMessage)
            {
                case SystemChatMessage systemMessage:
                    Console.WriteLine($"[SYSTEM]:");
                    Console.WriteLine($"{systemMessage.Content[0].Text}");
                    Console.WriteLine();
                    break;

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
