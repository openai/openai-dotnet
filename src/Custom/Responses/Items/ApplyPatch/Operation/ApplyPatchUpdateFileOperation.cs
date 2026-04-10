using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ApplyPatchUpdateFileOperation")]
public partial class ApplyPatchUpdateFileOperation
{
    // CUSTOM: Renamed.
    [CodeGenMember("Path")]
    public string FilePath { get; set; }
}
