namespace OpenAI.Responses;

[CodeGenType("ResponseTextOptions")]
public partial class ResponseTextOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Format")]
    public ResponseTextFormat ResponseFormat { get; set; }
}