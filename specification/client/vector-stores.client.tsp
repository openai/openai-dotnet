import "../base/typespec/vector-stores/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@visibility(ListVectorStoresResponse.data, Lifecycle.Read);
@@visibility(ListVectorStoreFilesResponse.data, Lifecycle.Read);

@@visibility(VectorStoreObject.metadata, Lifecycle.Read);

@@clientName(VectorStores.deleteVectorStoreFile, "RemoveFileFromStore");
@@clientName(VectorStores.cancelVectorStoreFileBatch, "CancelBatchFileJob");
@@clientName(VectorStores.getVectorStoreFile, "GetFileAssociation");
@@clientName(VectorStores.listVectorStores, "GetVectorStores");
