using OpenAI.Graders;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Files;

[CodeGenType("DeleteFileResponseObject")]
public readonly partial struct InternalDeleteFileResponseObject { }
[CodeGenType("AddUploadPartRequest")] public partial class InternalAddUploadPartRequest { }

[CodeGenType("CompleteUploadRequest")] public partial class InternalCompleteUploadRequest
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

[CodeGenType("CreateUploadRequest")] public partial class InternalCreateUploadRequest
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

[CodeGenType("CreateUploadRequestPurpose")] public readonly partial struct InternalCreateUploadRequestPurpose { }
[CodeGenType("ListFilesResponseObject")] public readonly partial struct InternalListFilesResponseObject { }
[CodeGenType("Upload")]
public partial class InternalUpload
{
}
[CodeGenType("UploadObject")] public readonly partial struct InternalUploadObject { }
[CodeGenType("UploadPart")] public partial class InternalUploadPart { }
[CodeGenType("UploadPartObject")] public readonly partial struct InternalUploadPartObject { }
[CodeGenType("UploadStatus")] public readonly partial struct InternalUploadStatus { }
