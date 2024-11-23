using System;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeServerEvent")]
public partial class ConversationUpdate
{
    [CodeGenMember("Kind")]
    public ConversationUpdateKind Kind { get; internal protected set; }

    public BinaryData GetRawContent() => ModelReaderWriter.Write(this);
}