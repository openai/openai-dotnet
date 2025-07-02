using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
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
                task: default,
                language: null,
                duration: null,
                text: response.Content?.ToString(),
                words: new ChangeTrackingList<TranscribedWord>(),
                segments: new ChangeTrackingList<TranscribedSegment>(),
                transcriptionTokenLogProbabilities: new ChangeTrackingList<AudioTokenLogProbabilityDetails>(),
                additionalBinaryDataProperties: new Dictionary<string, BinaryData>());
        }

        using var document = JsonDocument.Parse(response.Content);
        return DeserializeAudioTranscription(document.RootElement, null);
    }
}
