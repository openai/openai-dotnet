using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Responses;

// CUSTOM:
// - Renamed.
[CodeGenType("ResponsesReasoningItem")]
public partial class ReasoningResponseItem
{
    // CUSTOM: Convert simple text input into typed wire input
    public ReasoningResponseItem(IEnumerable<string> summaryTextParts) : base(InternalResponsesItemType.Reasoning)
    {
        Argument.AssertNotNull(summaryTextParts, nameof(summaryTextParts));

        Summary ??= [];
        foreach (string summaryTextPart in summaryTextParts)
        {
            Summary.Add(new InternalResponsesReasoningItemSummaryElementSummaryText(summaryTextPart));
        }
    }

    // CUSTOM: Made internal for simplified public reprojection
    [CodeGenMember("Summary")]
    internal IList<InternalResponsesReasoningItemSummaryElement> Summary { get; }

    public IReadOnlyList<string> SummaryTextParts
        => Summary?
            .Select(summaryElement => summaryElement as InternalResponsesReasoningItemSummaryElementSummaryText)?
            .Select(summaryTextElement => summaryTextElement.Text)?
            .ToList()
        ?? [];
}
