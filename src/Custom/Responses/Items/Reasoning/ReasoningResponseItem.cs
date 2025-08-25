using System.Linq;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ReasoningItemResource")]
public partial class ReasoningResponseItem
{
    // CUSTOM: Made nullable since this is a read-only property.
    [CodeGenMember("Status")]
    public ReasoningStatus? Status { get; }

    // CUSTOM: Added for convenience.
    public ReasoningResponseItem(string summaryText) : this(summaryParts: [new ReasoningSummaryTextPart(summaryText)])
    {
        Argument.AssertNotNull(summaryText, nameof(summaryText));
    }

    // CUSTOM: Added for convenience.
    public string GetSummaryText()
    {
        return string.Join(
            separator: string.Empty,
            values: SummaryParts.Select(part => (part as ReasoningSummaryTextPart)?.Text ?? string.Empty));
    }
}
