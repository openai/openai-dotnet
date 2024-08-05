﻿using NUnit.Framework;
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
        List<ChatMessage> messages = [
            new UserChatMessage("What's the weather like today?"),
        ];

        ChatCompletionOptions options = new()
        {
            Tools = ChatTool.CreateFunctionTools(typeof(MyFunctions))
        };
        #endregion

        #region
        bool requiresAction;

        do
        {
            requiresAction = false;
            ChatCompletion chatCompletion = await client.CompleteChatAsync(messages, options);

            switch (chatCompletion.FinishReason)
            {
                case ChatFinishReason.Stop:
                    {
                        // Add the assistant message to the conversation history.
                        messages.Add(new AssistantChatMessage(chatCompletion));
                        break;
                    }

                case ChatFinishReason.ToolCalls:
                    {
                        // First, add the assistant message with tool calls to the conversation history.
                        messages.Add(new AssistantChatMessage(chatCompletion));

                        // Then, add a new tool message for each tool call that is resolved.
                        foreach (ChatToolCall toolCall in chatCompletion.ToolCalls)
                        {
                            switch (toolCall.FunctionName)
                            {
                                case nameof(MyFunctions.GetCurrentLocation):
                                    {
                                        string toolResult = MyFunctions.GetCurrentLocation();
                                        messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                                        break;
                                    }

                                case nameof(MyFunctions.GetCurrentWeather):
                                    {
                                        var location = toolCall.GetFunctionArgument<string>("location");
                                        var unit = toolCall.GetFunctionArgument("unit", MyFunctions.TemperatureUnit.Celsius);
                                        string toolResult = MyFunctions.GetCurrentWeather(location, unit);
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
                    throw new NotImplementedException(chatCompletion.FinishReason.ToString());
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
