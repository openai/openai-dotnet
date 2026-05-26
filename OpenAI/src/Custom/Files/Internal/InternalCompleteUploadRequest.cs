using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel;

namespace OpenAI.Files;

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
