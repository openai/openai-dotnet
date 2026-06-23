import "../base/typespec/containers/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using OpenAI;
using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

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

@@clientName(ContainerMemoryLimit.`1g`, "Limit1G");
@@clientName(ContainerMemoryLimit.`4g`, "Limit4G");
@@clientName(ContainerMemoryLimit.`16g`, "Limit16G");
@@clientName(ContainerMemoryLimit.`64g`, "Limit64G");

@@dynamicModel(ContainersError);
@@dynamicModel(ContainersErrorResponse);
@@dynamicModel(ContainerMemoryLimit);
@@dynamicModel(ContainerExpiresAfterAnchor);
@@dynamicModel(ContainerExpiresAfter);
@@dynamicModel(ContainerNetworkPolicyType);
@@dynamicModel(ContainerNetworkPolicy);
@@dynamicModel(ContainerNetworkPolicyDisabled);
@@dynamicModel(ContainerNetworkPolicyAllowlist);
@@dynamicModel(ContainerNetworkPolicyDomainSecret);
@@dynamicModel(ContainerResource);
@@dynamicModel(ContainerCreationOptions);
@@dynamicModel(ContainerFileResource);
@@dynamicModel(ContainerFileUploadOptions);
@@dynamicModel(ContainerDeletionResult);
@@dynamicModel(ContainerFileDeletionResult);