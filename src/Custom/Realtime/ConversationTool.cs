using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeTool")]
public partial class ConversationTool
{
    // CUSTOM: Remove setter.
    [CodeGenMember("Type")]
    public ConversationToolKind Kind { get; }

    public static ConversationTool CreateFunctionTool(string name, string description = null, BinaryData parameters = null)
        => new ConversationFunctionTool(name)
        {
            Description = description,
            Parameters = parameters,
        };
}
