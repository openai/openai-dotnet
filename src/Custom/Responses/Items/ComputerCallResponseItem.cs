using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ComputerToolCallItemResource")]
public partial class ComputerCallResponseItem
{
    // CUSTOM: Retain optionality of OpenAPI read-only property value
    [CodeGenMember("Status")]
    public ComputerCallStatus? Status { get; }

    // CUSTOM: For reuse as an input model
    public ComputerCallResponseItem(string callId, ComputerCallAction action, IEnumerable<ComputerCallSafetyCheck> pendingSafetyChecks)
        : this(id: null, callId, action, pendingSafetyChecks, status: null)
    { }
}
