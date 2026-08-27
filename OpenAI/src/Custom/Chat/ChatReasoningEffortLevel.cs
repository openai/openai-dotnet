using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Chat;

// CUSTOM: Added Experimental attribute.
[CodeGenType("ChatReasoningEffort")]
public readonly partial struct ChatReasoningEffortLevel
{
}