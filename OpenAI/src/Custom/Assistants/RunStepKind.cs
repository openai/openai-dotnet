using Microsoft.TypeSpec.Generator.Customizations;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[CodeGenType("RunStepKind")]
public readonly partial struct RunStepKind
{
    [CodeGenMember("MessageCreation")]
    public static RunStepKind CreatedMessage { get; } = new RunStepKind(MessageCreationValue);

    [CodeGenMember("ToolCalls")]
    public static RunStepKind ToolCall { get; } = new RunStepKind(ToolCallsValue);
}
