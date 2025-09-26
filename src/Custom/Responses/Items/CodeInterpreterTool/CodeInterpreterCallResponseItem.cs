namespace OpenAI.Responses;

// CUSTOM: Renamed and made public.
[CodeGenType("CodeInterpreterToolCallItemResource")]
public partial class CodeInterpreterCallResponseItem
{
    // CUSTOM: Made nullable since this is a read-only property.  
    [CodeGenMember("Status")]
    public CodeInterpreterCallStatus? Status { get; }
}