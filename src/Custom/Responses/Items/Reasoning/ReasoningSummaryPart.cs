namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ReasoningItemSummaryPart")]
public partial class ReasoningSummaryPart
{
    // CUSTOM: Added as a constructor alternative for discoverability
    /// <summary> Creates a new instance of <see cref="ReasoningSummaryTextPart"/>. </summary>
    /// <param name="text"> The summary text. </param>
    public static ReasoningSummaryTextPart CreateTextPart(string text)
    {
        return new ReasoningSummaryTextPart(text);
    }
}
