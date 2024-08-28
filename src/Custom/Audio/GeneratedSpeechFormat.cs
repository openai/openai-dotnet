namespace OpenAI.Audio;

/// <summary>
/// Represents an audio data format available as either input or output into an audio operation.
/// </summary>
[CodeGenModel("CreateSpeechRequestResponseFormat")]
public enum GeneratedSpeechFormat
{
    /// <summary> MP3. /// </summary>
    [CodeGenMember("Mp3")]
    Mp3,

    /// <summary> Opus. /// </summary>
    [CodeGenMember("Opus")]
    Opus,

    /// <summary> AAC (advanced audio coding). /// </summary>
    [CodeGenMember("Aac")]
    Aac,

    /// <summary> FLAC (free lossless audio codec). /// </summary>
    [CodeGenMember("Flac")]
    Flac,

    /// <summary> WAV. /// </summary>
    [CodeGenMember("Wav")]
    Wav,

    /// <summary> PCM (pulse-code modulation). /// </summary>
    [CodeGenMember("Pcm")]
    Pcm,
}