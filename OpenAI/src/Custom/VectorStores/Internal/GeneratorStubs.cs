using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel;

namespace OpenAI.VectorStores;

// CUSTOM: Made internal.
[CodeGenType("CreateVectorStoreFileRequest")] internal partial class InternalCreateVectorStoreFileRequest { }
[CodeGenType("ListVectorStoreFilesResponse")] internal partial class InternalListVectorStoreFilesResponse : IInternalListResponse<VectorStoreFile> { }
[CodeGenType("ListVectorStoresResponse")] internal partial class InternalListVectorStoresResponse : IInternalListResponse<VectorStore> { }
[CodeGenType("VectorStoreFileBatchObjectFileCounts")] internal partial class InternalVectorStoreFileBatchObjectFileCounts { }
[CodeGenType("StaticChunkingStrategy")] internal partial class InternalStaticChunkingStrategy { }
[CodeGenType("ChunkingStrategyRequestParam")] internal partial class InternalChunkingStrategyRequestParam { }
[CodeGenType("ChunkingStrategyRequestParamType")] internal readonly partial struct InternalChunkingStrategyRequestParamType {}
[CodeGenType("AutoChunkingStrategyRequestParam")] internal partial class InternalAutoChunkingStrategyRequestParam { }
[CodeGenType("StaticChunkingStrategyRequestParam")] internal partial class InternalStaticChunkingStrategyRequestParam { }
[CodeGenType("UnknownChunkingStrategyRequestParam")] internal partial class InternalUnknownChunkingStrategyRequestParamProxy { }
[CodeGenType("ChunkingStrategyResponse")] internal partial class InternalChunkingStrategyResponseParam { }
[CodeGenType("ChunkingStrategyResponseType")] internal readonly partial struct InternalChunkingStrategyResponseParamType { }
[CodeGenType("StaticChunkingStrategyResponseParam")] internal partial class InternalStaticChunkingStrategyResponseParam { }
[CodeGenType("OtherChunkingStrategyResponseParam")] internal partial class InternalOtherChunkingStrategyResponseParam { }
[CodeGenType("DotNetCombinedAutoChunkingStrategyParam")] internal partial class InternalDotNetCombinedAutoChunkingStrategyParam { }
[CodeGenType("DotNetCombinedChunkingStrategyParamType")] internal readonly partial struct InternalDotNetCombinedChunkingStrategyParamType { }
[CodeGenType("DotNetCombinedChunkingStrategyParamType3")] internal readonly partial struct InternalDotNetCombinedChunkingStrategyParamType3 { }
[CodeGenType("UnknownDotNetCombinedChunkingStrategyParam")] internal partial class InternalUnknownDotNetCombinedChunkingStrategyParam { }
[CodeGenType("DotNetCombinedOtherChunkingStrategyParam")] internal partial class InternalDotNetCombinedOtherChunkingStrategyParam { }
[CodeGenType("UnknownChunkingStrategyResponse")] internal partial class InternalUnknownChunkingStrategyResponseParam { }
[CodeGenType("VectorStoreFileAttributes")] internal partial class InternalVectorStoreFileAttributes { }
[CodeGenType("VectorStoreSearchRequestRankingOptionsRanker")] internal readonly partial struct InternalVectorStoreSearchRequestRankingOptionsRanker {}
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
[CodeGenType("VectorStoreFileContentResponseData")] internal partial class InternalVectorStoreFileContentResponseDatum {}
[CodeGenType("VectorStoreSearchRequestRankingOptions")] internal partial class InternalVectorStoreSearchRequestRankingOptions {}
[CodeGenType("VectorStoreSearchRequest")] internal partial class InternalVectorStoreSearchRequest {}
[CodeGenType("VectorStoreSearchResultsPage")] internal partial class InternalVectorStoreSearchResultsPage {}
[CodeGenType("VectorStoreSearchResultItem")] internal partial class InternalVectorStoreSearchResultItem {}
[CodeGenType("VectorStoreSearchResultContentObject")] internal partial class InternalVectorStoreSearchResultContentObject {}
[CodeGenType("Metadata")] internal partial class InternalMetadata { }