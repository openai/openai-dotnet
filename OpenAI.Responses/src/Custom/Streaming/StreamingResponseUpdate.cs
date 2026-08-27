using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponseStreamEvent")]
[CodeGenSuppress("StreamingResponseUpdate", typeof(System.ClientModel.ClientResult))]
[CodeGenVisibility(nameof(StreamingResponseUpdate), CodeGenVisibility.ProtectedInternal, typeof(StreamingResponseUpdateKind), typeof(int))]
public partial class StreamingResponseUpdate
{
    // CUSTOM:
    // - Renamed.
    // - Made public.
    // - Removed setter.
    [CodeGenMember("Type")]
    public StreamingResponseUpdateKind Kind { get; }
}
