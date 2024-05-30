using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Audio;

public partial class AudioTranscription
{
    internal static AudioTranscription FromResponse(PipelineResponse response)
    {
        // Customization: handle plain text responses (SRT/VTT formats)
        if (response?.Headers?.TryGetValue("Content-Type", out string contentType) == true && contentType.StartsWith("text/plain"))
        {
            return new AudioTranscription(
                InternalCreateTranscriptionResponseVerboseJsonTask.Transcribe,
                language: null,
                duration: null,
                text: response.Content?.ToString(),
                words: [],
                segments: [],
                serializedAdditionalRawData: new ChangeTrackingDictionary<string, BinaryData>());
        }

        using var document = JsonDocument.Parse(response.Content);
        return DeserializeAudioTranscription(document.RootElement);
    }
}
