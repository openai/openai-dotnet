using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenType("RealtimeResponseUsageInputTokenDetails")]
public partial class ConversationInputTokenUsageDetails
{
    // CUSTOM: Remove output model optionality, make 'Count' names consistent with Chat

    [CodeGenMember("AudioTokens")]
    public int AudioTokenCount { get; }

    [CodeGenMember("CachedTokens")]
    public int CachedTokenCount { get; }

    [CodeGenMember("TextTokens")]
    public int TextTokenCount { get; }
}
