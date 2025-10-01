using OpenAI.Containers;
using System.ClientModel;

namespace OpenAI.VectorStores;

// CUSTOM: Made internal.
[CodeGenType("CreateVectorStoreFileBatchRequest")] public partial class InternalCreateVectorStoreFileBatchRequest
{
    public static implicit operator BinaryContent(InternalCreateVectorStoreFileBatchRequest internalCreateVectorStoreFileBatchRequest)
    {
        if (internalCreateVectorStoreFileBatchRequest == null)
        {
            return null;
        }
        return BinaryContent.Create(internalCreateVectorStoreFileBatchRequest, ModelSerializationExtensions.WireOptions);
    }
}
[CodeGenType("CreateVectorStoreFileRequest")] public partial class InternalCreateVectorStoreFileRequest { }
[CodeGenType("DeleteVectorStoreFileResponseObject")] public readonly partial struct InternalDeleteVectorStoreFileResponseObject { }
[CodeGenType("DeleteVectorStoreResponseObject")] public readonly partial struct InternalDeleteVectorStoreResponseObject { }
[CodeGenType("ListVectorStoreFilesResponse")] public partial class InternalListVectorStoreFilesResponse : IInternalListResponse<VectorStoreFile> { }
[CodeGenType("ListVectorStoreFilesResponseObject")] public readonly partial struct InternalListVectorStoreFilesResponseObject { }
[CodeGenType("ListVectorStoresResponse")] public partial class InternalListVectorStoresResponse : IInternalListResponse<VectorStore> { }
[CodeGenType("ListVectorStoresResponseObject")] public readonly partial struct InternalListVectorStoresResponseObject { }
[CodeGenType("VectorStoreFileBatchObjectFileCounts")] public partial class InternalVectorStoreFileBatchObjectFileCounts { }
[CodeGenType("VectorStoreFileBatchObjectObject")] public readonly partial struct InternalVectorStoreFileBatchObjectObject { }
[CodeGenType("VectorStoreFileObjectObject")] public readonly partial struct InternalVectorStoreFileObjectObject { }
[CodeGenType("VectorStoreObjectObject")] public readonly partial struct InternalVectorStoreObjectObject { }
[CodeGenType("StaticChunkingStrategy")] public partial class InternalStaticChunkingStrategy { }
[CodeGenType("ChunkingStrategyRequestParam")] public partial class InternalChunkingStrategyRequestParam { }
[CodeGenType("ChunkingStrategyRequestParamType")] public readonly partial struct InternalChunkingStrategyRequestParamType {}
[CodeGenType("AutoChunkingStrategyRequestParam")] public partial class InternalAutoChunkingStrategyRequestParam { }
[CodeGenType("StaticChunkingStrategyRequestParam")] public partial class InternalStaticChunkingStrategyRequestParam { }
[CodeGenType("UnknownChunkingStrategyRequestParam")] public partial class InternalUnknownChunkingStrategyRequestParamProxy { }
[CodeGenType("ChunkingStrategyResponseParam")] public partial class InternalChunkingStrategyResponseParam { }
[CodeGenType("ChunkingStrategyResponseParamType")] public readonly partial struct InternalChunkingStrategyResponseParamType { }
[CodeGenType("StaticChunkingStrategyResponseParam")] public partial class InternalStaticChunkingStrategyResponseParam { }
[CodeGenType("OtherChunkingStrategyResponseParam")] public partial class InternalOtherChunkingStrategyResponseParam { }
[CodeGenType("DotNetCombinedAutoChunkingStrategyParam")] public partial class InternalDotNetCombinedAutoChunkingStrategyParam { }
[CodeGenType("DotNetCombinedChunkingStrategyParamType")] public readonly partial struct InternalDotNetCombinedChunkingStrategyParamType { }
[CodeGenType("DotNetCombinedChunkingStrategyParamType3")] public readonly partial struct InternalDotNetCombinedChunkingStrategyParamType3 { }
[CodeGenType("UnknownDotNetCombinedChunkingStrategyParam")] public partial class InternalUnknownDotNetCombinedChunkingStrategyParam { }
[CodeGenType("DotNetCombinedOtherChunkingStrategyParam")] public partial class InternalDotNetCombinedOtherChunkingStrategyParam { }
[CodeGenType("UnknownChunkingStrategyResponseParam")] public partial class InternalUnknownChunkingStrategyResponseParam { }
[CodeGenType("VectorStoreFileAttributes")] public partial class InternalVectorStoreFileAttributes { }
[CodeGenType("VectorStoreFileContentResponseObject")] public readonly partial struct InternalVectorStoreFileContentResponseObject {}
[CodeGenType("VectorStoreSearchRequestRankingOptionsRanker")] public readonly partial struct InternalVectorStoreSearchRequestRankingOptionsRanker {}
[CodeGenType("VectorStoreSearchResultsPageObject")] public readonly partial struct InternalVectorStoreSearchResultsPageObject {}
[CodeGenType("VectorStoreSearchResultContentObjectType")] public readonly partial struct InternalVectorStoreSearchResultContentObjectType {}
[CodeGenType("UpdateVectorStoreFileAttributesRequest")] public partial class InternalUpdateVectorStoreFileAttributesRequest
{
    public static implicit operator BinaryContent(InternalUpdateVectorStoreFileAttributesRequest internalUpdateVectorStoreFileAttributesRequest)
    {
        if (internalUpdateVectorStoreFileAttributesRequest == null)
        {
            return null;
        }
        return BinaryContent.Create(internalUpdateVectorStoreFileAttributesRequest, ModelSerializationExtensions.WireOptions);
    }
}
[CodeGenType("VectorStoreFileContentResponse")] public partial class InternalVectorStoreFileContentResponse {}
[CodeGenType("VectorStoreFileContentResponseDatum")] public partial class InternalVectorStoreFileContentResponseDatum {}
[CodeGenType("VectorStoreSearchRequestRankingOptions")] public partial class InternalVectorStoreSearchRequestRankingOptions {}
[CodeGenType("VectorStoreSearchRequest")] public partial class InternalVectorStoreSearchRequest {}
[CodeGenType("VectorStoreSearchResultsPage")] public partial class InternalVectorStoreSearchResultsPage {}
[CodeGenType("VectorStoreSearchResultItem")] public partial class InternalVectorStoreSearchResultItem {}
[CodeGenType("VectorStoreSearchResultContentObject")] public partial class InternalVectorStoreSearchResultContentObject {}