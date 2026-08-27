using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("DotNetRealtimeErrorGA")]
public partial class RealtimeError
{
    // CUSTOM: Renamed.
    [CodeGenMember("Param")]
    public string ParameterName { get; }
}