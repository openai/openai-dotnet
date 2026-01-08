using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel;

namespace OpenAI.Files;

[CodeGenType("DeleteFileResponseObject")]
internal readonly partial struct InternalDeleteFileResponseObject { }
[CodeGenType("AddUploadPartRequest")] internal partial class InternalAddUploadPartRequest { }

[CodeGenType("CompleteUploadRequest")] internal partial class InternalCompleteUploadRequest
{
    public static implicit operator BinaryContent(InternalCompleteUploadRequest internalCompleteUploadRequest)
    {
        if (internalCompleteUploadRequest == null)
        {
            return null;
        }
        return BinaryContent.Create(internalCompleteUploadRequest, ModelSerializationExtensions.WireOptions);
    }
}

[CodeGenType("CreateUploadRequest")] internal partial class InternalCreateUploadRequest
{
    public static implicit operator BinaryContent(InternalCreateUploadRequest internalCreateUploadRequest)
    {
        if (internalCreateUploadRequest == null)
        {
            return null;
        }
        return BinaryContent.Create(internalCreateUploadRequest, ModelSerializationExtensions.WireOptions);
    }
}

[CodeGenType("CreateUploadRequestPurpose")] internal readonly partial struct InternalCreateUploadRequestPurpose { }
[CodeGenType("ListFilesResponseObject")] internal readonly partial struct InternalListFilesResponseObject { }
[CodeGenType("Upload")]
internal partial class InternalUpload
{
}
[CodeGenType("UploadObject")] internal readonly partial struct InternalUploadObject { }
[CodeGenType("UploadPart")] internal partial class InternalUploadPart { }
[CodeGenType("UploadPartObject")] internal readonly partial struct InternalUploadPartObject { }
[CodeGenType("UploadStatus")] internal readonly partial struct InternalUploadStatus { }
