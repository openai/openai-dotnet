using OpenAI.Containers;
using System.ClientModel;

namespace OpenAI.VectorStores;

// CUSTOM: Made internal.
[CodeGenType("CreateVectorStoreFileBatchRequest")] internal partial class InternalCreateVectorStoreFileBatchRequest { }
[CodeGenType("CreateVectorStoreFileRequest")] internal partial class InternalCreateVectorStoreFileRequest { }
[CodeGenType("DeleteVectorStoreFileResponseObject")] internal readonly partial struct InternalDeleteVectorStoreFileResponseObject { }
[CodeGenType("DeleteVectorStoreResponseObject")] internal readonly partial struct InternalDeleteVectorStoreResponseObject { }
[CodeGenType("ListVectorStoreFilesResponse")] internal partial class InternalListVectorStoreFilesResponse : IInternalListResponse<VectorStoreFileAssociation> { }
[CodeGenType("ListVectorStoreFilesResponseObject")] internal readonly partial struct InternalListVectorStoreFilesResponseObject { }
[CodeGenType("ListVectorStoresResponse")] internal partial class InternalListVectorStoresResponse : IInternalListResponse<VectorStore> { }
[CodeGenType("ListVectorStoresResponseObject")] internal readonly partial struct InternalListVectorStoresResponseObject { }
[CodeGenType("VectorStoreFileBatchObjectFileCounts")] internal partial class InternalVectorStoreFileBatchObjectFileCounts { }
[CodeGenType("VectorStoreFileBatchObjectObject")] internal readonly partial struct InternalVectorStoreFileBatchObjectObject { }
[CodeGenType("VectorStoreFileObjectObject")] internal readonly partial struct InternalVectorStoreFileObjectObject { }
[CodeGenType("VectorStoreObjectObject")] internal readonly partial struct InternalVectorStoreObjectObject { }
[CodeGenType("StaticChunkingStrategy")] internal partial class InternalStaticChunkingStrategy { }
[CodeGenType("ChunkingStrategyRequestParam")] internal partial class InternalChunkingStrategyRequestParam { }
[CodeGenType("ChunkingStrategyRequestParamType")] internal readonly partial struct InternalChunkingStrategyRequestParamType {}
[CodeGenType("AutoChunkingStrategyRequestParam")] internal partial class InternalAutoChunkingStrategyRequestParam { }
[CodeGenType("StaticChunkingStrategyRequestParam")] internal partial class InternalStaticChunkingStrategyRequestParam { }
[CodeGenType("UnknownChunkingStrategyRequestParam")] internal partial class InternalUnknownChunkingStrategyRequestParamProxy { }
[CodeGenType("ChunkingStrategyResponseParam")] internal partial class InternalChunkingStrategyResponseParam { }
[CodeGenType("ChunkingStrategyResponseParamType")] internal readonly partial struct InternalChunkingStrategyResponseParamType { }
[CodeGenType("StaticChunkingStrategyResponseParam")] internal partial class InternalStaticChunkingStrategyResponseParam { }
[CodeGenType("OtherChunkingStrategyResponseParam")] internal partial class InternalOtherChunkingStrategyResponseParam { }
[CodeGenType("DotNetCombinedAutoChunkingStrategyParam")] internal partial class InternalDotNetCombinedAutoChunkingStrategyParam { }
[CodeGenType("DotNetCombinedChunkingStrategyParamType")] internal readonly partial struct InternalDotNetCombinedChunkingStrategyParamType { }
[CodeGenType("DotNetCombinedChunkingStrategyParamType3")] internal readonly partial struct InternalDotNetCombinedChunkingStrategyParamType3 { }
[CodeGenType("UnknownDotNetCombinedChunkingStrategyParam")] internal partial class InternalUnknownDotNetCombinedChunkingStrategyParam { }
[CodeGenType("DotNetCombinedOtherChunkingStrategyParam")] internal partial class InternalDotNetCombinedOtherChunkingStrategyParam { }
[CodeGenType("UnknownChunkingStrategyResponseParam")] internal partial class InternalUnknownChunkingStrategyResponseParam { }
[CodeGenType("VectorStoreFileAttributes")] internal partial class InternalVectorStoreFileAttributes { }
[CodeGenType("VectorStoreFileContentResponseObject")] internal readonly partial struct InternalVectorStoreFileContentResponseObject {}
[CodeGenType("VectorStoreSearchRequestRankingOptionsRanker")] internal readonly partial struct InternalVectorStoreSearchRequestRankingOptionsRanker {}
[CodeGenType("VectorStoreSearchResultsPageObject")] internal readonly partial struct InternalVectorStoreSearchResultsPageObject {}
[CodeGenType("VectorStoreSearchResultContentObjectType")] internal readonly partial struct InternalVectorStoreSearchResultContentObjectType {}
[CodeGenType("UpdateVectorStoreFileAttributesRequest")] internal partial class InternalUpdateVectorStoreFileAttributesRequest
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
[CodeGenType("VectorStoreFileContentResponse")] internal partial class InternalVectorStoreFileContentResponse {}
[CodeGenType("VectorStoreFileContentResponseDatum")] internal partial class InternalVectorStoreFileContentResponseDatum {}
[CodeGenType("VectorStoreSearchRequestRankingOptions")] internal partial class InternalVectorStoreSearchRequestRankingOptions {}
[CodeGenType("VectorStoreSearchRequest")] internal partial class InternalVectorStoreSearchRequest {}
[CodeGenType("VectorStoreSearchResultsPage")] internal partial class InternalVectorStoreSearchResultsPage {}
[CodeGenType("VectorStoreSearchResultItem")] internal partial class InternalVectorStoreSearchResultItem {}
[CodeGenType("VectorStoreSearchResultContentObject")] internal partial class InternalVectorStoreSearchResultContentObject {}