using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeResponseUsageOutputTokenDetails")] 
public partial class ConversationOutputTokenUsageDetails
{
    // CUSTOM: Remove output model optionality, make 'Count' names consistent with Chat

    [CodeGenMember("TextTokens")]
    public int TextTokenCount { get; }

    [CodeGenMember("AudioTokens")]
    public int AudioTokenCount { get; }
}
