using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeContentPartType")]
public readonly partial struct ConversationContentPartKind
{
    // GA format: output_audio and output_text are the type discriminator values
    // These expose the generated constants as public properties
    public static ConversationContentPartKind OutputAudio { get; } = new(OutputAudioValue);
    public static ConversationContentPartKind OutputText { get; } = new(OutputTextValue);
}