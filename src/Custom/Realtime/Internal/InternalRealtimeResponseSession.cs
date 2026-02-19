using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
[CodeGenType("RealtimeResponseSession")]
internal partial class InternalRealtimeResponseSession
{
    // CUSTOM: Use a scenario-specific copy of the voice ID collection.
    [CodeGenMember("Voice")]
    internal ConversationVoice Voice { get; set; }

    // Customization: API changed from max_response_output_tokens to max_output_tokens
    [CodeGenMember("MaxOutputTokens")]
    private BinaryData _maxOutputTokens;

    public ConversationMaxTokensChoice MaxOutputTokens
    {
        get => ConversationMaxTokensChoice.FromBinaryData(_maxOutputTokens);
        set
        {
            _maxOutputTokens = value == null ? null : ModelReaderWriter.Write(value, ModelReaderWriterOptions.Json, OpenAIContext.Default);
        }
    }
}
