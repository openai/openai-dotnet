using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.VectorStores;

[CodeGenType("CreateVectorStoreFileBatchRequest")]
internal partial class InternalCreateVectorStoreFileBatchRequest
{
    public InternalCreateVectorStoreFileBatchRequest(IEnumerable<string> fileIds)
    {
        FileIds = fileIds.ToList();
        Attributes = new ChangeTrackingDictionary<string, BinaryData>();
    }

    public InternalCreateVectorStoreFileBatchRequest(IEnumerable<InternalCreateVectorStoreFileRequest> files)
    {
        Files = files.ToList();
        Attributes = new ChangeTrackingDictionary<string, BinaryData>();
    }

    public static implicit operator BinaryContent(InternalCreateVectorStoreFileBatchRequest internalCreateVectorStoreFileBatchRequest)
    {
        if (internalCreateVectorStoreFileBatchRequest == null)
        {
            return null;
        }
        return BinaryContent.Create(internalCreateVectorStoreFileBatchRequest, ModelSerializationExtensions.WireOptions);
    }
}
