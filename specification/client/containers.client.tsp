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

@@clientName(ContainerFileResource.bytes, "SizeInBytes");
@@alternateType(ContainerFileResource.bytes, int64 | null);

@@clientName(ContainerCollectionOrder.asc, "Ascending");
@@clientName(ContainerCollectionOrder.desc, "Descending");

@@clientName(ContainerFileCollectionOrder.asc, "Ascending");
@@clientName(ContainerFileCollectionOrder.desc, "Descending");

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
@@dynamicModel(CreateContainerOptions);
@@dynamicModel(ContainerFileResource);
@@dynamicModel(CreateContainerFileOptions);
@@dynamicModel(ContainerDeletionResult);
@@dynamicModel(ContainerFileDeletionResult);