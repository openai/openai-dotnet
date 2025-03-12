using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Responses;

// CUSTOM:
// - Renamed.
// - Suppressed constructor in favor of custom constructor with required `id` parameter.
[CodeGenType("ResponsesReasoningItem")]
[CodeGenSuppress(nameof(ReasoningResponseItem), typeof(IEnumerable<ResponseContentPart>))]
public partial class ReasoningResponseItem
{
    public ReasoningResponseItem(string id, IEnumerable<string> summaryTextParts) : base(InternalResponsesItemType.Reasoning, id)
    {
        Argument.AssertNotNull(id, nameof(id));
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
