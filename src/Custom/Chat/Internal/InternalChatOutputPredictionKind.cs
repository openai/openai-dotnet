using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat;

[CodeGenType("ChatOutputPredictionType")]
internal readonly partial struct InternalChatOutputPredictionKind
{
    // CUSTOM: Rename for clarity.
    [CodeGenMember("Content")]
    public static InternalChatOutputPredictionKind StaticContent { get; } = new InternalChatOutputPredictionKind(ContentValue);
}