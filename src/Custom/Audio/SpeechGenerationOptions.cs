namespace OpenAI.Audio;

/// <summary>
/// A representation of additional options available to control the behavior of a text-to-speech audio generation
/// operation.
/// </summary>
[CodeGenModel("CreateSpeechRequest")]
[CodeGenSuppress("SpeechGenerationOptions", typeof(InternalCreateSpeechRequestModel), typeof(string), typeof(GeneratedSpeechVoice))]
public partial class SpeechGenerationOptions
{
    // CUSTOM:
    // - Made internal. The model is specified by the client.
    // - Added setter.
    /// <summary> One of the available [TTS models](/docs/models/tts): `tts-1` or `tts-1-hd`. </summary>
    internal InternalCreateSpeechRequestModel Model { get; set; }

    // CUSTOM:
    // - Made internal. This value comes from a parameter on the client method.
    // - Added setter.
    /// <summary> The text to generate audio for. The maximum length is 4096 characters. </summary>
    internal string Input { get; set; }

    // CUSTOM:
    // - Made internal. This value comes from a parameter on the client method.
    // - Added setter.
    /// <summary>
    /// The voice to use when generating the audio. Supported voices are `alloy`, `echo`, `fable`,
    /// `onyx`, `nova`, and `shimmer`. Previews of the voices are available in the
    /// [Text to speech guide](/docs/guides/text-to-speech/voice-options).
    /// </summary>
    internal GeneratedSpeechVoice Voice { get; set;  }

    // CUSTOM: Made public now that there are no required properties.
    /// <summary> Initializes a new instance of <see cref="SpeechGenerationOptions"/>. </summary>
    public SpeechGenerationOptions()
    {
    }
}