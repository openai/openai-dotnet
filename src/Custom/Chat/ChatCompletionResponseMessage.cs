using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OpenAI.Chat;

namespace OpenAI.Chat
{
    [Experimental("OPENAI001")]
    public partial class ChatCompletionResponseMessage
    {
        private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

        internal ChatCompletionResponseMessage(string content, string refusal)
        {
            Content = content;
            Refusal = refusal;
            ToolCalls = new ChangeTrackingList<ChatToolCall>();
            Annotations = new ChangeTrackingList<ChatMessageAnnotation>();
        }

        internal ChatCompletionResponseMessage(string content, string refusal, IReadOnlyList<ChatToolCall> toolCalls, IReadOnlyList<ChatMessageAnnotation> annotations, ChatMessageRole role, ChatCompletionResponseMessageFunctionCall functionCall, ChatOutputAudio audio, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        {
            // Plugin customization: ensure initialization of collections
            Content = content;
            Refusal = refusal;
            ToolCalls = toolCalls ?? new ChangeTrackingList<ChatToolCall>();
            Annotations = annotations ?? new ChangeTrackingList<ChatMessageAnnotation>();
            Role = role;
            FunctionCall = functionCall;
            Audio = audio;
            _additionalBinaryDataProperties = additionalBinaryDataProperties;
        }

        public string Content { get; }

        public string Refusal { get; }

        public IReadOnlyList<ChatToolCall> ToolCalls { get; }

        public IReadOnlyList<ChatMessageAnnotation> Annotations { get; }

        public ChatMessageRole Role { get; } = ChatMessageRole.Assistant;

        public ChatCompletionResponseMessageFunctionCall FunctionCall { get; }

        public ChatOutputAudio Audio { get; }

        internal IDictionary<string, BinaryData> SerializedAdditionalRawData
        {
            get => _additionalBinaryDataProperties;
            set => _additionalBinaryDataProperties = value;
        }
    }
}