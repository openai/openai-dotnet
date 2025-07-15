using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ReasoningItemResource")]
public partial class ReasoningResponseItem
{
    // CUSTOM: Retain optionality of OpenAPI read-only property value
    [CodeGenMember("Status")]
    public ReasoningStatus? Status { get; }

    // CUSTOM: Rename for collection clarity
    [CodeGenMember("Summary")]
    public IReadOnlyList<ReasoningSummaryPart> SummaryParts { get; }

    // CUSTOM: Enable reuse as an input model
    public ReasoningResponseItem(IEnumerable<ReasoningSummaryPart> summaryParts)
        : this(id: null, summaryParts)
    { }

    // CUSTOM: Facilitate typical single-item summary text input model use
    public ReasoningResponseItem(string summaryText)
        : this(summaryParts: [new ReasoningSummaryTextPart(summaryText)])
    { }

    // CUSTOM: Provide convenience for typical single-item or text-concatenation scenario
    public string GetSummaryText()
        => string.Join(
            separator: string.Empty,
            SummaryParts.Select(part => (part as ReasoningSummaryTextPart)?.Text ?? string.Empty));
}
