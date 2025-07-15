using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat;

// CUSTOM: Added Experimental attribute.
/// <summary>
/// Represents the configuration details for output audio requested in a chat completion request.
/// </summary>
/// <remarks>
/// When provided to a <see cref="ChatCompletionOptions"/> instance's <see cref="ChatCompletionOptions.AudioOptions"/> property,
/// the request's specified content modalities will be automatically updated to reflect desired audio output.
/// </remarks>
[CodeGenType("CreateChatCompletionRequestAudio1")]
[CodeGenVisibility(nameof(ChatAudioOptions), CodeGenVisibility.Internal)]
public partial class ChatAudioOptions
{
    // CUSTOM: Renamed.
    /// <summary>
    /// Gets or sets the voice model that the response should use to synthesize audio.
    /// </summary>
    [CodeGenMember("Voice")]
    public ChatOutputAudioVoice OutputAudioVoice { get; }

    // CUSTOM: Renamed.
    /// <summary>
    /// Specifies the output format desired for synthesized audio.
    /// </summary>
    [CodeGenMember("Format")]
    public ChatOutputAudioFormat OutputAudioFormat { get; }
}
