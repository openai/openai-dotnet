using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeTool")]
public abstract partial class ConversationTool
{
    [CodeGenMember("Type")]
    public ConversationToolKind Kind { get; }

    public static ConversationTool CreateFunctionTool(string name, string description = null, BinaryData parameters = null)
        => new ConversationFunctionTool(name)
        {
            Description = description,
            Parameters = parameters,
        };
}
