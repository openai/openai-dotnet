using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("FunctionToolCallOutputItemResource")]
public partial class FunctionCallOutputResponseItem
{
    // CUSTOM: Renamed.
    [CodeGenMember("Output")]
    public string FunctionOutput { get; set; }

    // CUSTOM: Retain optionality of OpenAPI read-only property value
    [CodeGenMember("Status")]
    public FunctionCallOutputStatus? Status { get; }

    // CUSTOM: For reuse as an input model
    internal FunctionCallOutputResponseItem(string callId, string functionOutput)
        : this(id: null, callId, functionOutput, status: null)
    { }
}
