// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;

namespace OpenAI.Chat
{
    public partial class ChatTokenUsage
    {
        internal IDictionary<string, BinaryData> SerializedAdditionalRawData { get; set; }
        internal ChatTokenUsage(int outputTokenCount, int inputTokenCount, int totalTokenCount)
        {
            OutputTokenCount = outputTokenCount;
            InputTokenCount = inputTokenCount;
            TotalTokenCount = totalTokenCount;
        }

        internal ChatTokenUsage(int outputTokenCount, int inputTokenCount, int totalTokenCount, ChatOutputTokenUsageDetails outputTokenDetails, IDictionary<string, BinaryData> serializedAdditionalRawData)
        {
            OutputTokenCount = outputTokenCount;
            InputTokenCount = inputTokenCount;
            TotalTokenCount = totalTokenCount;
            OutputTokenDetails = outputTokenDetails;
            SerializedAdditionalRawData = serializedAdditionalRawData;
        }

        internal ChatTokenUsage()
        {
        }
    }
}
