import "../base/typespec/vector-stores/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@alternateType(StaticChunkingStrategy.max_chunk_size_tokens, int32);
@@alternateType(StaticChunkingStrategy.chunk_overlap_tokens, int32);
@@alternateType(VectorStoreExpirationAfter.days, int32);
@@alternateType(VectorStoreObject.usage_bytes, int32);
@@alternateType(VectorStoreObjectFileCounts.in_progress, int32);
@@alternateType(VectorStoreObjectFileCounts.completed, int32);
@@alternateType(VectorStoreObjectFileCounts.failed, int32);
@@alternateType(VectorStoreObjectFileCounts.cancelled, int32);
@@alternateType(VectorStoreObjectFileCounts.total, int32);
@@alternateType(VectorStoreFileBatchObjectFileCounts.in_progress, int32);
@@alternateType(VectorStoreFileBatchObjectFileCounts.completed, int32);
@@alternateType(VectorStoreFileBatchObjectFileCounts.failed, int32);
@@alternateType(VectorStoreFileBatchObjectFileCounts.cancelled, int32);
@@alternateType(VectorStoreFileBatchObjectFileCounts.total, int32);
@@alternateType(VectorStoreSearchRequest.max_num_results, int32);
@@alternateType(VectorStoreSearchRequestRankingOptions.score_threshold, float32);
@@alternateType(VectorStoreSearchResultItem.score, float32);
@@alternateType(listVectorStores::parameters.limit, int32);
@@alternateType(listFilesInVectorStoreBatch::parameters.limit, int32);
@@alternateType(listVectorStoreFiles::parameters.limit, int32);

@@visibility(ListVectorStoresResponse.data, Lifecycle.Read);
@@visibility(ListVectorStoreFilesResponse.data, Lifecycle.Read);

@@visibility(VectorStoreObject.metadata, Lifecycle.Read);

@@alternateType(CreateVectorStoreRequest.metadata, Record<string>);
@@alternateType(UpdateVectorStoreRequest.metadata, Record<string>);
@@alternateType(VectorStoreObject.metadata, Record<string> | null);

@@alternateType(VectorStoreExpirationAfter.anchor, VectorStoreExpirationAnchor);

@@clientName(VectorStoreFileObject.usage_bytes, "UsageInBytes");
@@alternateType(VectorStoreFileObject.usage_bytes, int64 | null);

@@clientName(deleteVectorStoreFile, "RemoveFileFromVectorStore");
@@clientName(listVectorStores, "GetVectorStores");
@@clientName(listFilesInVectorStoreBatch, "GetVectorStoreFilesInBatch");
@@clientName(createVectorStoreFile, "AddFileToVectorStore");
@@clientName(createVectorStoreFileBatch, "AddFileBatchToVectorStore");

@@clientLocation(listVectorStores, "VectorStores");
@@clientLocation(createVectorStore, "VectorStores");
@@clientLocation(deleteVectorStore, "VectorStores");
@@clientLocation(getVectorStore, "VectorStores");
@@clientLocation(modifyVectorStore, "VectorStores");

@@clientLocation(createVectorStoreFileBatch, "VectorStores");
@@clientLocation(getVectorStoreFileBatch, "VectorStores");
@@clientLocation(cancelVectorStoreFileBatch, "VectorStores");
@@clientLocation(listFilesInVectorStoreBatch, "VectorStores");

@@clientLocation(listVectorStoreFiles, "VectorStores");
@@clientLocation(createVectorStoreFile, "VectorStores");
@@clientLocation(deleteVectorStoreFile, "VectorStores");
@@clientLocation(getVectorStoreFile, "VectorStores");

@@clientLocation(updateVectorStoreFileAttributes, "VectorStores");
@@clientLocation(retrieveVectorStoreFileContent, "VectorStores");
@@clientLocation(searchVectorStore, "VectorStores");
