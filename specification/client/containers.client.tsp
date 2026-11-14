import "../base/typespec/containers/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@alternateType(ContainerExpiresAfter.minutes, int32);
@@alternateType(ContainerCollectionOptions.limit, int32);
@@alternateType(ContainerFileCollectionOptions.limit, int32);

@@clientName(Containers.retrieveContainer, "GetContainer");
@@clientName(Containers.createContainerFile, "UploadContainerFile");
@@clientName(Containers.retrieveContainerFile, "GetContainerFile");
@@clientName(Containers.retrieveContainerFileContent, "DownloadContainerFile");

@@clientName(ContainerResource.expires_after, "ExpirationPolicy");

@@clientName(ContainerCreationOptions.expires_after, "ExpirationPolicy");

@@clientName(ContainerFileResource.bytes, "SizeInBytes");
@@alternateType(ContainerFileResource.bytes, int64 | null);

@@clientName(ContainerCollectionOrder.asc, "Ascending");
@@clientName(ContainerCollectionOrder.desc, "Descending");

@@clientName(ContainerFileCollectionOrder.asc, "Ascending");
@@clientName(ContainerFileCollectionOrder.desc, "Descending");

@@clientName(ContainerMemoryLimit.`1g`, "Max1GB");
@@clientName(ContainerMemoryLimit.`4g`, "Max4GB");
@@clientName(ContainerMemoryLimit.`16g`, "Max16GB");
@@clientName(ContainerMemoryLimit.`64g`, "Max64GB");

@@dynamicModel(ContainersError, "csharp");
@@dynamicModel(ContainersErrorResponse, "csharp");
@@dynamicModel(ContainerMemoryLimit, "csharp");
@@dynamicModel(ContainerExpiresAfterAnchor, "csharp");
@@dynamicModel(ContainerExpiresAfter, "csharp");
@@dynamicModel(ContainerNetworkPolicyType, "csharp");
@@dynamicModel(ContainerNetworkPolicy, "csharp");
@@dynamicModel(ContainerNetworkPolicyDisabled, "csharp");
@@dynamicModel(ContainerNetworkPolicyAllowlist, "csharp");
@@dynamicModel(ContainerNetworkPolicyDomainSecret, "csharp");
@@dynamicModel(ContainerResource, "csharp");
@@dynamicModel(ContainerCreationOptions, "csharp");
@@dynamicModel(ContainerFileResource, "csharp");
@@dynamicModel(ContainerFileUploadOptions, "csharp");
@@dynamicModel(ContainerDeletionResult, "csharp");
@@dynamicModel(ContainerFileDeletionResult, "csharp");
@@dynamicModel(ContainerCollectionOrder, "csharp");
@@dynamicModel(ContainerCollection, "csharp");
@@dynamicModel(ContainerFileCollectionOrder, "csharp");
@@dynamicModel(ContainerFileCollection, "csharp");