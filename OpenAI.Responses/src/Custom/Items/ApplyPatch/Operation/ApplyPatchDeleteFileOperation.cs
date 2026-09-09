using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ApplyPatchDeleteFileOperation")]
public partial class ApplyPatchDeleteFileOperation
{
    // CUSTOM: Renamed.
    [CodeGenMember("Path")]
    public string FilePath { get; set; }
}
