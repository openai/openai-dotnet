using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Containers;

// CUSTOM: Renamed.
[CodeGenType("ContainerExpiresAfter")]
public partial class ContainerExpirationPolicy
{
    // CUSTOM:
    // - Renamed.
    // - Changed type to TimeSpan?.
    [CodeGenMember("Minutes")]
    public TimeSpan? Duration { get; set; }
}
