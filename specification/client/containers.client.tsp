import "../base/typespec/containers/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@convenientAPI(Containers.createContainer, false);
@@convenientAPI(Containers.retrieveContainer, false);
@@convenientAPI(Containers.deleteContainer, false);
@@convenientAPI(Containers.createContainerFile, false);
@@convenientAPI(Containers.retrieveContainerFile, false);
@@convenientAPI(Containers.deleteContainerFile, false);
@@convenientAPI(Containers.retrieveContainerFileContent, false);

@@clientName(Containers.retrieveContainer, "GetContainer");
@@clientName(Containers.retrieveContainerFile, "GetContainerFile");
@@clientName(Containers.retrieveContainerFileContent, "GetContainerFileContent");
