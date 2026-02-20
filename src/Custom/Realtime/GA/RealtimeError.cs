using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("DotNetRealtimeErrorGA")]
public partial class GARealtimeError
{
    // CUSTOM: Renamed.
    [CodeGenMember("Param")]
    public string ParameterName { get; }
}