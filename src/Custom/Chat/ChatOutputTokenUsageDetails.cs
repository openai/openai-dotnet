using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat;

/// <summary> A breakdown of the number of tokens used to generate the output as reported in <see cref="ChatTokenUsage.OutputTokenCount"/>. </summary>
[CodeGenType("CompletionUsageCompletionTokensDetails")]
public partial class ChatOutputTokenUsageDetails
{
    // CUSTOM: Renamed.
    /// <summary> The number of tokens consumed internally by the model for the purpose of reasoning. </summary>
    /// <remarks> Only applicable to models with reasoning capabilities, such as the <see href="https://openai.com/o1/">OpenAI o1 series</see>. </remarks>
    [CodeGenMember("ReasoningTokens")]
    public int ReasoningTokenCount { get; }

    // CUSTOM: Renamed.
    /// <summary> The number of audio tokens in the output. </summary>
    [CodeGenMember("AudioTokens")]
    public int AudioTokenCount { get; }

    // CUSTOM:
    // - Added Experimental attribute.
    // - Renamed.
    [Experimental("OPENAI001")]
    [CodeGenMember("AcceptedPredictionTokens")]
    public int AcceptedPredictionTokenCount { get; }

    // CUSTOM:
    // - Added Experimental attribute.
    // - Renamed.
    [Experimental("OPENAI001")]
    [CodeGenMember("RejectedPredictionTokens")]
    public int RejectedPredictionTokenCount { get; }
}