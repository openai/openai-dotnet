using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeResponseCreateParams")]
public partial class ConversationResponseOptions
{
    [CodeGenMember("Conversation")]
    public ResponseConversationSelection? ConversationSelection { get; set; }

    // CUSTOM: Use a scenario-specific copy of the voice ID collection.
    [CodeGenMember("Voice")]
    public ConversationVoice? Voice;

    [CodeGenMember("Modalities")]
    private IList<InternalRealtimeRequestSessionModality> _internalModalities;

    public RealtimeContentModalities ContentModalities
    {
        get => RealtimeContentModalitiesExtensions.FromInternalModalities(_internalModalities);
        set => _internalModalities = value.ToInternalModalities();
    }

    [CodeGenMember("ToolChoice")]
    private BinaryData _internalToolChoice;

    public ConversationToolChoice ToolChoice
    {
        get => ConversationToolChoice.FromBinaryData(_internalToolChoice);
        set
        {
            _internalToolChoice = value is not null
                ? ModelReaderWriter.Write(value)
                : null;
        }
    }

    [CodeGenMember("MaxResponseOutputTokens")]
    private BinaryData _maxResponseOutputTokens;

    public ConversationMaxTokensChoice MaxOutputTokens
    {
        get => ConversationMaxTokensChoice.FromBinaryData(_maxResponseOutputTokens);
        set
        {
            _maxResponseOutputTokens = value == null ? null : ModelReaderWriter.Write(value);
        }
    }

    [CodeGenMember("Input")]
    public IList<RealtimeItem> OverrideItems { get; }
}
