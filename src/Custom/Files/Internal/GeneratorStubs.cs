using OpenAI.Graders;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;

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
    public static explicit operator InternalUpload(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeInternalUpload(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
[CodeGenType("UploadObject")] internal readonly partial struct InternalUploadObject { }
[CodeGenType("UploadPart")] internal partial class InternalUploadPart { }
[CodeGenType("UploadPartObject")] internal readonly partial struct InternalUploadPartObject { }
[CodeGenType("UploadStatus")] internal readonly partial struct InternalUploadStatus { }
