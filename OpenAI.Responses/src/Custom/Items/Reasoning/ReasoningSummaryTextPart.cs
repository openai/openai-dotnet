using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ReasoningItemSummaryTextPart")]
[CodeGenSuppress("ReasoningSummaryTextPart")]
public partial class ReasoningSummaryTextPart
{
    public ReasoningSummaryTextPart() : this(InternalReasoningItemSummaryPartType.SummaryText, default, null)
    {
    }
}
