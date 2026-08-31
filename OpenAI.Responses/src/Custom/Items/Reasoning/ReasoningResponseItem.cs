using Microsoft.TypeSpec.Generator.Customizations;
using System.Linq;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ReasoningItemResource")]
[CodeGenSuppress("ReasoningResponseItem")]
public partial class ReasoningResponseItem
{
    // CUSTOM: Delegate to internal hydration constructor.
    public ReasoningResponseItem() : this(ResponseItemKind.Reasoning, null, default, default, null, null)
    {
    }

    // CUSTOM:
    // - Made nullable because this is an optional property.
    // - Added setter because this is an optional property in an input/output type.
    [CodeGenMember("Status")]
    public ReasoningStatus? Status { get; set; }

    // CUSTOM: Added for convenience.
    public ReasoningResponseItem(string summaryText) : this(summaryParts: [new ReasoningSummaryTextPart(summaryText)])
    {
        Argument.AssertNotNull(summaryText, nameof(summaryText));
    }

    // CUSTOM: Added for convenience.
    public string GetSummaryText()
    {
        return string.Concat(values: SummaryParts.Select(part => (part as ReasoningSummaryTextPart)?.Text ?? string.Empty));
    }
}
