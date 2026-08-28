using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ApplyPatchCreateFileOperation")]
[CodeGenSuppress("ApplyPatchCreateFileOperation")]
public partial class ApplyPatchCreateFileOperation
{
    // CUSTOM: Renamed.
    [CodeGenMember("Path")]
    public string FilePath { get; set; }
}
