// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;

namespace OpenAI.RealtimeConversation
{
    internal partial class InternalRealtimeClientEventResponseCreateResponse
    {
        internal IDictionary<string, BinaryData> SerializedAdditionalRawData { get; set; }
        public InternalRealtimeClientEventResponseCreateResponse()
        {
            Modalities = new ChangeTrackingList<string>();
            Tools = new ChangeTrackingList<ConversationTool>();
        }

        internal InternalRealtimeClientEventResponseCreateResponse(IList<string> modalities, string instructions, string voice, string outputAudioFormat, IList<ConversationTool> tools, BinaryData toolChoice, float? temperature, BinaryData maxOutputTokens, IDictionary<string, BinaryData> serializedAdditionalRawData)
        {
            Modalities = modalities;
            Instructions = instructions;
            Voice = voice;
            OutputAudioFormat = outputAudioFormat;
            Tools = tools;
            ToolChoice = toolChoice;
            Temperature = temperature;
            MaxOutputTokens = maxOutputTokens;
            SerializedAdditionalRawData = serializedAdditionalRawData;
        }

        public IList<string> Modalities { get; }
        public string Instructions { get; set; }
        public string Voice { get; set; }
        public string OutputAudioFormat { get; set; }
        public IList<ConversationTool> Tools { get; }
        public float? Temperature { get; set; }
        public BinaryData MaxOutputTokens { get; set; }
    }
}