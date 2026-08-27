using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Audio;

public partial class AudioTranscription
{
    internal static AudioTranscription FromResponse(PipelineResponse response)
    {
        // Customization: handle plain text responses (SRT/VTT formats)
        if (response?.Headers?.TryGetValue("Content-Type", out string contentType) == true &&
            contentType.StartsWith("text/plain", StringComparison.Ordinal))
        {
            return new AudioTranscription(
                language: null,
                duration: null,
                text: response.Content?.ToString(),
                words: new ChangeTrackingList<TranscribedWord>(),
                segments: new ChangeTrackingList<TranscribedSegment>(),
                usage: null,
                transcriptionTokenLogProbabilities: new ChangeTrackingList<AudioTokenLogProbabilityDetails>(),
                additionalBinaryDataProperties: new Dictionary<string, BinaryData>());
        }

        using var document = JsonDocument.Parse(response.Content);
        return DeserializeAudioTranscription(document.RootElement, null);
    }

    // CUSTOM: The explicit operator is not auto-generated because the response type
    // has been customized in the specification (DotNetCombinedJsonTranscriptionResponse).
    // Auto-generation only occurs for root output types that directly match the operation's
    // response schema without customization.
    [Experimental("OPENAI001")]
    public static explicit operator AudioTranscription(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeAudioTranscription(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
