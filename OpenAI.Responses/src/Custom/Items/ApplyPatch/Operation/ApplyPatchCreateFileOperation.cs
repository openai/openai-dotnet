using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ApplyPatchCreateFileOperation")]
[CodeGenSuppress("ApplyPatchCreateFileOperation")]
public partial class ApplyPatchCreateFileOperation
{
    public ApplyPatchCreateFileOperation() : this(InternalApplyPatchOperationType.CreateFile, default, null, null)
    {
    }

    // CUSTOM: Renamed.
    [CodeGenMember("Path")]
    public string FilePath { get; set; }
}
