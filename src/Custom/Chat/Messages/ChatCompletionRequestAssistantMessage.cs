using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat
{
    [Experimental("OPENAI001")]
    public partial class ChatCompletionRequestAssistantMessage : ChatMessage
    {
        public ChatCompletionRequestAssistantMessage() : this(ChatMessageRole.Assistant, null, default, null, null, null, null, null)
        {
        }

        public ChatCompletionRequestAssistantMessage(ChatCompletionResponseMessage message) : this(ChatMessageRole.Assistant, null, default, null, null, null, null, null)
        {
            Refusal = message.Refusal;
            Audio = new(message.Audio?.Id);
            ToolCalls = message.ToolCalls != null && message.ToolCalls.Count > 0 ? new(message.ToolCalls) :  new ChangeTrackingList<ChatToolCall>();
            FunctionCall = message.FunctionCall != null ? new(message.FunctionCall.Name, BinaryData.FromString(message.FunctionCall.Arguments)) : null;
        }

        internal ChatCompletionRequestAssistantMessage(ChatMessageRole role, ChatMessageContent content, in JsonPatch patch, string refusal, string name, ChatOutputAudioReference audio, IList<ChatToolCall> toolCalls, ChatFunctionCall functionCall) : base(role, content, patch)
        {
            // Plugin customization: ensure initialization of collections
            Refusal = refusal;
            Name = name;
            Audio = audio;
            ToolCalls = toolCalls ?? new ChangeTrackingList<ChatToolCall>();
            FunctionCall = functionCall;
        }

        public string Refusal { get; set; }

        public string Name { get; set; }

        public ChatOutputAudioReference Audio { get; set; }

        public IList<ChatToolCall> ToolCalls { get; }

        public ChatFunctionCall FunctionCall { get; set; }
    }
}