using System;

namespace OpenAI.Responses;

// CUSTOM:
// - Renamed.
// - Suppressed constructor in favor of custom constructor with required `id` parameter.
[CodeGenType("ResponsesFunctionCallItem")]
[CodeGenSuppress(nameof(FunctionCallResponseItem), typeof(string), typeof(string), typeof(BinaryData))]
public partial class FunctionCallResponseItem
{
    public FunctionCallResponseItem(string id, string callId, string functionName, BinaryData functionArguments) : base(InternalResponsesItemType.FunctionCall, id)
    {
        Argument.AssertNotNull(id, nameof(id));
        Argument.AssertNotNull(callId, nameof(callId));
        Argument.AssertNotNull(functionName, nameof(functionName));
        Argument.AssertNotNull(functionArguments, nameof(functionArguments));

        CallId = callId;
        FunctionName = functionName;
        FunctionArguments = functionArguments;
    }

    // CUSTOM: Renamed.
    [CodeGenMember("Name")]
    public string FunctionName { get; set; }

    // CUSTOM:
    // - Renamed.
    // - Changed type from string to BinaryData.
    [CodeGenMember("Arguments")]
    public BinaryData FunctionArguments { get; set; }
}