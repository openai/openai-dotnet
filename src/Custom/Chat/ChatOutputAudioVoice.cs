using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat;

// CUSTOM: Added Experimental attribute.
/// <summary>
/// Specifies the available voices that the model can use when generating output audio as part of a chat completion.
/// </summary>
[CodeGenType("DotNetChatVoiceIds")]
public readonly partial struct ChatOutputAudioVoice
{

}
