using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.VectorStores;

[CodeGenType("CreateVectorStoreFileBatchRequest")]
internal partial class InternalCreateVectorStoreFileBatchRequest
{
    public InternalCreateVectorStoreFileBatchRequest(IEnumerable<string> fileIds) : this(fileIds.ToList(), null, null, null, null)
    {
    }

    public InternalCreateVectorStoreFileBatchRequest(IEnumerable<InternalCreateVectorStoreFileRequest> files) : this(null, files.ToList(), null, null, null)
    {
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
