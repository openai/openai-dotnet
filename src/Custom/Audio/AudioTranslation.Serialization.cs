using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Audio;

public partial class AudioTranslation
{
    internal static AudioTranslation FromResponse(PipelineResponse response)
    {
        // Customization: handle plain text responses (SRT/VTT formats)
        if (response?.Headers?.TryGetValue("Content-Type", out string contentType) == true && contentType.StartsWith("text/plain"))
        {
            return new AudioTranslation(
                InternalCreateTranslationResponseVerboseJsonTask.Translate,
                language: null,
                duration: null,
                text: response.Content?.ToString(),
                segments: [],
                serializedAdditionalRawData: new ChangeTrackingDictionary<string, BinaryData>());
        }

        using var document = JsonDocument.Parse(response.Content);
        return DeserializeAudioTranslation(document.RootElement);
    }
}
