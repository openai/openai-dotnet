using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.Buffers;
using System.ClientModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace OpenAI.Examples;

public partial class ChatExamples
{
    // See Example03_FunctionCalling.cs for the tool and function definitions.

    #region
    public class StreamingChatToolCallsBuilder
    {
        private readonly Dictionary<int, string> _indexToToolCallId = [];
        private readonly Dictionary<int, string> _indexToFunctionName = [];
        private readonly Dictionary<int, SequenceBuilder<byte>> _indexToFunctionArguments = [];

        public void Append(StreamingChatToolCallUpdate toolCallUpdate)
        {
            // Keep track of which tool call ID belongs to this update index.
            if (toolCallUpdate.ToolCallId != null)
            {
                _indexToToolCallId[toolCallUpdate.Index] = toolCallUpdate.ToolCallId;
            }

            // Keep track of which function name belongs to this update index.
            if (toolCallUpdate.FunctionName != null)
            {
                _indexToFunctionName[toolCallUpdate.Index] = toolCallUpdate.FunctionName;
            }

            // Keep track of which function arguments belong to this update index,
            // and accumulate the arguments as new updates arrive.
            if (toolCallUpdate.FunctionArgumentsUpdate != null && !toolCallUpdate.FunctionArgumentsUpdate.ToMemory().IsEmpty)
            {
                if (!_indexToFunctionArguments.TryGetValue(toolCallUpdate.Index, out SequenceBuilder<byte> argumentsBuilder))
                {
                    argumentsBuilder = new SequenceBuilder<byte>();
                    _indexToFunctionArguments[toolCallUpdate.Index] = argumentsBuilder;
                }

                argumentsBuilder.Append(toolCallUpdate.FunctionArgumentsUpdate);
            }
        }

        public IReadOnlyList<ChatToolCall> Build()
        {
            List<ChatToolCall> toolCalls = [];

            foreach ((int index, string toolCallId) in _indexToToolCallId)
            {
                ReadOnlySequence<byte> sequence = _indexToFunctionArguments[index].Build();

                ChatToolCall toolCall = ChatToolCall.CreateFunctionToolCall(
                    id: toolCallId,
                    functionName: _indexToFunctionName[index],
                    functionArguments: BinaryData.FromBytes(sequence.ToArray()));

                toolCalls.Add(toolCall);
            }

            return toolCalls;
        }
    }
    #endregion

    #region
    public class SequenceBuilder<T>
    {
        Segment _first;
        Segment _last;

        public void Append(ReadOnlyMemory<T> data)
        {
            if (_first == null)
            {
                Debug.Assert(_last == null);
                _first = new Segment(data);
                _last = _first;
            }
            else
            {
                _last = _last!.Append(data);
            }
        }

        public ReadOnlySequence<T> Build()
        {
            if (_first == null)
            {
                Debug.Assert(_last == null);
                return ReadOnlySequence<T>.Empty;
            }

            if (_first == _last)
            {
                Debug.Assert(_first.Next == null);
                return new ReadOnlySequence<T>(_first.Memory);
            }

            return new ReadOnlySequence<T>(_first, 0, _last!, _last!.Memory.Length);
        }

        private sealed class Segment : ReadOnlySequenceSegment<T>
        {
            public Segment(ReadOnlyMemory<T> items) : this(items, 0)
            {
            }

            private Segment(ReadOnlyMemory<T> items, long runningIndex)
            {
                Debug.Assert(runningIndex >= 0);
                Memory = items;
                RunningIndex = runningIndex;
            }

            public Segment Append(ReadOnlyMemory<T> items)
            {
                long runningIndex;
                checked { runningIndex = RunningIndex + Memory.Length; }
                Segment segment = new(items, runningIndex);
                Next = segment;
                return segment;
            }
        }
    }
    #endregion

    [Test]
    public void Example04_FunctionCallingStreaming()
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
            StringBuilder contentBuilder = new();
            StreamingChatToolCallsBuilder toolCallsBuilder = new();

            CollectionResult<StreamingChatCompletionUpdate> completionUpdates = client.CompleteChatStreaming(messages, options);

            foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
            {
                // Accumulate the text content as new updates arrive.
                foreach (ChatMessageContentPart contentPart in completionUpdate.ContentUpdate)
                {
                    contentBuilder.Append(contentPart.Text);
                }

                // Build the tool calls as new updates arrive.
                foreach (StreamingChatToolCallUpdate toolCallUpdate in completionUpdate.ToolCallUpdates)
                {
                    toolCallsBuilder.Append(toolCallUpdate);
                }

                switch (completionUpdate.FinishReason)
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
                            IReadOnlyList<ChatToolCall> toolCalls = toolCallsBuilder.Build();

                            // Next, add the assistant message with tool calls to the conversation history.
                            AssistantChatMessage assistantMessage = new(toolCalls);

                            if (contentBuilder.Length > 0)
                            {
                                assistantMessage.Content.Add(ChatMessageContentPart.CreateTextPart(contentBuilder.ToString()));
                            }

                            messages.Add(assistantMessage);

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
