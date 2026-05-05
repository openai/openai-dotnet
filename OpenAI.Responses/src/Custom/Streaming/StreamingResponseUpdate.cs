using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponseStreamEvent")]
[CodeGenSuppress("StreamingResponseUpdate", typeof(System.ClientModel.ClientResult))]
public partial class StreamingResponseUpdate
{
    // CUSTOM:
    // - Renamed.
    // - Made public.
    // - Removed setter.
    [CodeGenMember("Type")]
    public StreamingResponseUpdateKind Kind { get; }
}