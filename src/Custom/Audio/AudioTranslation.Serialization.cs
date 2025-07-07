using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Audio;

public partial class AudioTranslation
{
    internal static AudioTranslation FromResponse(PipelineResponse response)
    {
        // Customization: handle plain text responses (SRT/VTT formats)
        if (response?.Headers?.TryGetValue("Content-Type", out string contentType) == true &&
            contentType.StartsWith("text/plain", StringComparison.Ordinal))
        {
            return new AudioTranslation(null, text: response.Content?.ToString(), null, task: default, null, null);
        }

        using var document = JsonDocument.Parse(response.Content);
        return DeserializeAudioTranslation(document.RootElement, null);
    }
}
