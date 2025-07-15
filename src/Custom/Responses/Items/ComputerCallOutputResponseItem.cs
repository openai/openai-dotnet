using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ComputerToolCallOutputItemResource")]
public partial class ComputerCallOutputResponseItem
{
    // CUSTOM: Retain optionality of OpenAPI read-only property value
    [CodeGenMember("Status")]
    public ComputerCallOutputStatus? Status { get; }

    // CUSTOM: For reuse as an input model
    internal ComputerCallOutputResponseItem(string callId, ComputerOutput output)
        : this(id: null, callId, output, status: null)
    { }
}
