using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat;

// CUSTOM: Added Experimental attribute.
/// <summary>
/// Specifies the audio format the model should use when generating output audio as part of a chat completion
/// response.
/// </summary>
[CodeGenType("CreateChatCompletionRequestAudioFormat")]
public readonly partial struct ChatOutputAudioFormat
{

}
