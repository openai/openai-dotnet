namespace OpenAI.Audio;

/// <summary> The options to configure text-to-speech audio generation. </summary>
[CodeGenModel("CreateSpeechRequest")]
[CodeGenSuppress("SpeechGenerationOptions", typeof(InternalCreateSpeechRequestModel), typeof(string), typeof(GeneratedSpeechVoice))]
public partial class SpeechGenerationOptions
{
    // CUSTOM:
    // - Made internal. The model is specified by the client.
    // - Added setter.
    [CodeGenMember("Model")] 
    internal InternalCreateSpeechRequestModel Model { get; set; }

    // CUSTOM:
    // - Made internal. This value comes from a parameter on the client method.
    // - Added setter.
    /// <summary> The text to generate audio for. The maximum length is 4096 characters. </summary>
    [CodeGenMember("Input")] 
    internal string Input { get; set; }

    // CUSTOM:
    // - Made internal. This value comes from a parameter on the client method.
    // - Added setter.
    [CodeGenMember("Voice")]
    internal GeneratedSpeechVoice Voice { get; set; }

    // CUSTOM: Made public now that there are no required properties.
    /// <summary> Initializes a new instance of <see cref="SpeechGenerationOptions"/>. </summary>
    public SpeechGenerationOptions()
    {
    }

    // CUSTOM: Renamed.
    /// <summary>
    ///     The speed of the generated audio expressed as a ratio between 0.5 and 2.0. The default is 1.0.
    /// </summary>
    [CodeGenMember("Speed")]

    public float? SpeedRatio { get; set; }
}