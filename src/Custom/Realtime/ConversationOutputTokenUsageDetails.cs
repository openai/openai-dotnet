using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeResponseUsageOutputTokenDetails")]
public partial class ConversationOutputTokenUsageDetails
{
    // CUSTOM: Remove output model optionality, make 'Count' names consistent with Chat

    [CodeGenMember("TextTokens")]
    public int TextTokenCount { get; }

    [CodeGenMember("AudioTokens")]
    public int AudioTokenCount { get; }
}
