using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
[CodeGenType("RealtimeResponseMessageItem")]
internal partial class InternalRealtimeResponseMessageItem
{
    // CUSTOM: Use the available strong type for roles.

    [CodeGenMember("Role")]
    public ConversationMessageRole Role { get; }

    // CUSTOM: Explicitly apply response model read-only.

    [CodeGenMember("Content")]
    public IReadOnlyList<ConversationContentPart> Content { get; }
}
