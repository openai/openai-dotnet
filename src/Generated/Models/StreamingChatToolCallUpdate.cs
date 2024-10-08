// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;

namespace OpenAI.Chat
{
    public partial class StreamingChatToolCallUpdate
    {
        internal IDictionary<string, BinaryData> SerializedAdditionalRawData { get; set; }

        internal StreamingChatToolCallUpdate(int index, string toolCallId, ChatToolCallKind kind, InternalChatCompletionMessageToolCallChunkFunction function, IDictionary<string, BinaryData> serializedAdditionalRawData)
        {
            Index = index;
            ToolCallId = toolCallId;
            Kind = kind;
            Function = function;
            SerializedAdditionalRawData = serializedAdditionalRawData;
        }

        internal StreamingChatToolCallUpdate()
        {
        }

        public int Index { get; }
    }
}
