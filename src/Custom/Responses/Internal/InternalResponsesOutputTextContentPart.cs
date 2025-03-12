namespace OpenAI.Responses;

[CodeGenType("ResponsesOutputContentText")]
internal partial class InternalResponsesOutputTextContentPart : ResponseContentPart
{
    // CUSTOM: Rename for parent recombination of common properties
    [CodeGenMember("Text")]
    public string InternalText { get; set; }
}
