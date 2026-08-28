using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ApplyPatchUpdateFileOperation")]
[CodeGenSuppress("ApplyPatchUpdateFileOperation")]
public partial class ApplyPatchUpdateFileOperation
{
    public ApplyPatchUpdateFileOperation() : this(InternalApplyPatchOperationType.UpdateFile, default, null, null)
    {
    }

    // CUSTOM: Renamed.
    [CodeGenMember("Path")]
    public string FilePath { get; set; }
}
