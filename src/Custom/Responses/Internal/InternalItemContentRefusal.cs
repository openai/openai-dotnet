namespace OpenAI.Responses;

[CodeGenType("ItemContentRefusal")]
internal partial class InternalItemContentRefusal
{
    // CUSTOM: Rename for parent recombination of common properties
    [CodeGenMember("Refusal")]
    public string InternalRefusal { get; set; }
}
