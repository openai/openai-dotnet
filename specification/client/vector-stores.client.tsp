import "../base/typespec/vector-stores/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@visibility(ListVectorStoresResponse.data, Lifecycle.Read);
@@visibility(ListVectorStoreFilesResponse.data, Lifecycle.Read);

@@visibility(VectorStoreObject.metadata, Lifecycle.Read);

@@alternateType(CreateVectorStoreRequest.metadata, Record<string>);
@@alternateType(UpdateVectorStoreRequest.metadata, Record<string>);
@@alternateType(VectorStoreObject.metadata, Record<string> | null);

@@alternateType(VectorStoreExpirationAfter.anchor, VectorStoreExpirationAnchor);

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

// Add pagination decorators
@@list(listVectorStores);
@@list(listFilesInVectorStoreBatch);
@@list(listVectorStoreFiles);

@@pageItems(ListVectorStoresResponse.data);
@@pageItems(ListVectorStoreFilesResponse.data);

@@continuationToken(ListVectorStoresResponse.last_id);
@@continuationToken(ListVectorStoreFilesResponse.last_id);
