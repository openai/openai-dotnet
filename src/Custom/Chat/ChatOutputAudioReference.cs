using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat;

// CUSTOM: Added Experimental attribute.
/// <summary>
/// Represents an ID-based reference to a past audio output as received from a prior chat completion response, as
/// provided when creating an <see cref="AssistantChatMessage"/> instance for use in a conversation history.
/// </summary>
/// <remarks>
/// This value is obtained from the <see cref="ChatCompletion.OutputAudio.Id"/> or
/// <see cref="StreamingChatCompletionUpdate.OutputAudioUpdate.Id"/> properties for streaming and non-streaming
/// responses, respectively. The <see cref="AssistantChatMessage(ChatCompletion)"/> constructor overload can also be
/// used to automatically populate the appropriate properties from a <see cref="ChatCompletion"/> instance.
/// </remarks>
[CodeGenType("ChatCompletionRequestAssistantMessageAudio1")]
public partial class ChatOutputAudioReference
{
}