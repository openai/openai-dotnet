namespace OpenAI.Responses;

[CodeGenType("ItemContentOutputText")]
internal partial class InternalItemContentOutputText
{
    // CUSTOM: Rename for parent recombination of common properties
    [CodeGenMember("Text")]
    public string InternalText { get; set; }
}
