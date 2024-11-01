using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeClientEventResponseCreateResponse")]
internal partial class InternalRealtimeClientEventResponseCreateResponse
{
    [CodeGenMember("ToolChoice")]
    public BinaryData ToolChoice { get; set; }

    public static InternalRealtimeClientEventResponseCreateResponse FromSessionOptions(
        ConversationSessionOptions sessionOptions)
    {
        Argument.AssertNotNull(sessionOptions, nameof(sessionOptions));
        if (Optional.IsDefined(sessionOptions.InputAudioFormat))
        {
            throw new InvalidOperationException($"{nameof(sessionOptions.InputAudioFormat)} cannot be overriden"
                + " per response.");
        }
        BinaryData maxTokensChoice = Optional.IsDefined(sessionOptions.MaxOutputTokens)
            ? ModelReaderWriter.Write(sessionOptions.MaxOutputTokens)
            : null;
        IList<InternalRealtimeRequestSessionModality> internalModalities
            = sessionOptions.ContentModalities.ToInternalModalities();
        IList<string> rawModalities = internalModalities.Count > 0
            ? internalModalities.Select(modality => modality.ToString()).ToList()
            : new ChangeTrackingList<string>();
        BinaryData toolChoice = Optional.IsDefined(sessionOptions.ToolChoice)
            ? ModelReaderWriter.Write(sessionOptions.ToolChoice)
            : null;
        InternalRealtimeClientEventResponseCreateResponse internalOptions = new(
            modalities: rawModalities,
            instructions: sessionOptions.Instructions,
            voice: sessionOptions.Voice?.ToString(),
            outputAudioFormat: sessionOptions.OutputAudioFormat?.ToString(),
            tools: sessionOptions.Tools,
            toolChoice: toolChoice,
            temperature: sessionOptions.Temperature,
            maxOutputTokens: maxTokensChoice,
            serializedAdditionalRawData: null);
        return internalOptions;
    }
}
