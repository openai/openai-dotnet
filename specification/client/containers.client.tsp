import "../base/typespec/containers/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;

namespace OpenAI.Containers;

@@clientName(OpenAI.Containers, "ContainersClient", "csharp");
@@clientNamespace(OpenAI.Containers, "OpenAI.Containers", "csharp");
@@dynamicModel(OpenAI.Containers, "csharp");

@@alternateType(ContainerExpiresAfter.minutes, int32);
@@alternateType(ContainerCollectionOptions.limit, int32);
@@alternateType(ContainerFileCollectionOptions.limit, int32);

@@clientName(retrieveContainer, "GetContainer");
@@clientName(createContainerFile, "UploadContainerFile");
@@clientName(retrieveContainerFile, "GetContainerFile");
@@clientName(retrieveContainerFileContent, "DownloadContainerFile");

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
