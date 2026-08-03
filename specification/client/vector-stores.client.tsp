import "../base/typespec/vector-stores/main.tsp";
import "./models/vector-stores-enums.models.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using OpenAI;
using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

@@visibility(ListVectorStoresResponse.data, Lifecycle.Read);
@@visibility(ListVectorStoreFilesResponse.data, Lifecycle.Read);

@@visibility(VectorStoreObject.metadata, Lifecycle.Read);

@@alternateType(CreateVectorStoreRequest.metadata, Record<string>);
@@alternateType(UpdateVectorStoreRequest.metadata, Record<string>);
@@alternateType(VectorStoreObject.metadata, Record<string> | null);
@@alternateType(VectorStoreObject.status, VectorStoreStatus);

@@alternateType(VectorStoreExpirationAfter.anchor, VectorStoreExpirationAnchor);

@@alternateType(VectorStoreFileObject.status, VectorStoreFileStatus);

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

@@dynamicModel(VectorStoreStatus);
@@dynamicModel(VectorStoreFileStatus);
