using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ApplyPatchDeleteFileOperation")]
[CodeGenSuppress("ApplyPatchDeleteFileOperation")]
public partial class ApplyPatchDeleteFileOperation
{
    public ApplyPatchDeleteFileOperation() : this(InternalApplyPatchOperationType.DeleteFile, default, null)
    {
    }

    // CUSTOM: Renamed.
    [CodeGenMember("Path")]
    public string FilePath { get; set; }
}
