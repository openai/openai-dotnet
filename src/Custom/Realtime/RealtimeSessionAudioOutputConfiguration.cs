using Microsoft.TypeSpec.Generator.Customizations;
using OpenAI.Internal;

namespace OpenAI.Realtime;

public partial class RealtimeSessionAudioOutputConfiguration
{
    // Note: the generated Format property is of type InternalRealtimeAudioFormats,
    // which is a discriminated union type. This custom property provides a simplified
    // RealtimeAudioFormat interface for setting and getting the audio format.
    private RealtimeAudioFormat? _audioFormat;

    // Note: the generated Voice property is internal with type InternalVoiceIdsShared.
    // This custom property provides a public ConversationVoice interface.
    private ConversationVoice? _voice;

    // Suppress the generated internal Format property
    [CodeGenMember("Format")]
    internal InternalRealtimeAudioFormats InternalFormat
    {
        get => ConvertToInternalFormat(_audioFormat);
        set => _audioFormat = ConvertFromInternalFormat(value);
    }

    // Suppress the generated internal Voice property
    [CodeGenMember("Voice")]
    internal InternalVoiceIdsShared? InternalVoice
    {
        get => _voice.HasValue ? new InternalVoiceIdsShared(_voice.Value.ToString()) : null;
        set => _voice = value.HasValue ? new ConversationVoice(value.Value.ToString()) : null;
    }

    /// <summary>
    /// Gets or sets the audio format for output audio.
    /// </summary>
    /// <remarks>
    /// Supported formats include:
    /// <list type="bullet">
    /// <item><see cref="RealtimeAudioFormat.Pcm16"/> - PCM audio format at 24kHz sample rate</item>
    /// <item><see cref="RealtimeAudioFormat.G711Alaw"/> - G.711 A-law audio format</item>
    /// <item><see cref="RealtimeAudioFormat.G711Ulaw"/> - G.711 μ-law audio format</item>
    /// </list>
    /// </remarks>
    public RealtimeAudioFormat? Format
    {
        get => _audioFormat;
        set
        {
            _audioFormat = value;
        }
    }

    /// <summary>
    /// Gets or sets the voice the model uses to respond.
    /// </summary>
    public ConversationVoice? Voice
    {
        get => _voice;
        set => _voice = value;
    }

    private static RealtimeAudioFormat? ConvertFromInternalFormat(InternalRealtimeAudioFormats format)
    {
        if (format == null)
        {
            return null;
        }

        if (format.Kind == InternalRealtimeAudioFormatsType.AudioPcm)
        {
            return RealtimeAudioFormat.Pcm16;
        }
        else if (format.Kind == InternalRealtimeAudioFormatsType.AudioPcmu)
        {
            return RealtimeAudioFormat.G711Ulaw;
        }
        else if (format.Kind == InternalRealtimeAudioFormatsType.AudioPcma)
        {
            return RealtimeAudioFormat.G711Alaw;
        }

        return null;
    }

    private static InternalRealtimeAudioFormats ConvertToInternalFormat(RealtimeAudioFormat? format)
    {
        if (!format.HasValue)
        {
            return null;
        }

        if (format.Value == RealtimeAudioFormat.Pcm16)
        {
            return new InternalRealtimeAudioFormatsPcm();
        }
        else if (format.Value == RealtimeAudioFormat.G711Ulaw)
        {
            return new InternalRealtimeAudioFormatsPcmu();
        }
        else if (format.Value == RealtimeAudioFormat.G711Alaw)
        {
            return new InternalRealtimeAudioFormatsPcma();
        }

        return null;
    }
}
