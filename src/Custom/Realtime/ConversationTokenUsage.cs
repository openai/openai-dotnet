using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

public partial class ConversationTokenUsage
{
    // CUSTOM: Renamed, nullability removed
    [CodeGenMember("InputTokens")]
    public int InputTokenCount { get; }

    // CUSTOM: Renamed, nullability removed
    [CodeGenMember("OutputTokens")]
    public int OutputTokenCount { get; }

    // CUSTOM: Renamed, nullability removed
    [CodeGenMember("TotalTokens")]
    public int TotalTokenCount { get; }
}