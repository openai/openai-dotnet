using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Responses;

// CUSTOM:
// - Renamed.
// - Suppressed constructor in favor of custom constructor with required `id` parameter.
[CodeGenType("ResponsesComputerCallItem")]
[CodeGenSuppress(nameof(ComputerCallResponseItem), typeof(string), typeof(ComputerCallAction), typeof(IEnumerable<ComputerCallSafetyCheck>))]
public partial class ComputerCallResponseItem
{
    public ComputerCallResponseItem(string id, string callId, ComputerCallAction action, IEnumerable<ComputerCallSafetyCheck> pendingSafetyChecks) : base(InternalResponsesItemType.ComputerCall, id)
    {
        Argument.AssertNotNull(id, nameof(id));
        Argument.AssertNotNull(callId, nameof(callId));
        Argument.AssertNotNull(action, nameof(action));
        Argument.AssertNotNull(pendingSafetyChecks, nameof(pendingSafetyChecks));

        CallId = callId;
        Action = action;
        PendingSafetyChecks = pendingSafetyChecks.ToList();
    }
}
