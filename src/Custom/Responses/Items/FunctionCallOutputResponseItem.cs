namespace OpenAI.Responses;

[CodeGenType("ResponsesFunctionCallOutput")]
public partial class FunctionCallOutputResponseItem
{
    // CUSTOM: Renamed.
    [CodeGenMember("Output")]
    public string FunctionOutput { get; set; }
}
