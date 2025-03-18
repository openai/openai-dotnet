using System;

namespace OpenAI.Responses;

// CUSTOM:
// - Renamed.
// - Customized serialization of the BinaryData-as-string FunctionArguments
[CodeGenType("ResponsesFunctionCallItem")]
[CodeGenSerialization(nameof(FunctionArguments), SerializationValueHook = nameof(SerializeFunctionArgumentsValue), DeserializationValueHook = nameof(DeserializeFunctionArgumentsValue))]
public partial class FunctionCallResponseItem
{
    // CUSTOM: Renamed.
    [CodeGenMember("Name")]
    public string FunctionName { get; set; }

    // CUSTOM:
    // - Renamed.
    // - Changed type from string to BinaryData.
    [CodeGenMember("Arguments")]
    public BinaryData FunctionArguments { get; set; }
}