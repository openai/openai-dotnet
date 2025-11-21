using System.Linq;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ReasoningItemResource")]
public partial class ReasoningResponseItem
{
    // CUSTOM: Made nullable since this is a read-only property.
    [CodeGenMember("Status")]
    public ReasoningStatus? Status { get; }

    // CUSTOM: Added as a convenience.
    public ReasoningResponseItem(string summaryText) : this(summary: [new ReasoningSummaryTextPart(summaryText)])
    {
    }

    // CUSTOM: Added as a convenience.
    public string GetSummaryText()
    {
        return string.Concat(
            values: Summary.Select(part => (part as ReasoningSummaryTextPart)?.Text ?? string.Empty));
    }
}
