using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("Reasoning")]
[CodeGenVisibility(nameof(ResponseReasoningOptions), CodeGenVisibility.Public)]
public partial class ResponseReasoningOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Effort")]
    public ResponseReasoningEffortLevel? ReasoningEffortLevel { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Summary")]
    public ResponseReasoningSummaryVerbosity? ReasoningSummaryVerbosity { get; set; }
}