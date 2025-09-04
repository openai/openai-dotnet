import "../base/typespec/containers/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@clientName(Containers.retrieveContainer, "GetContainer");
@@clientName(Containers.retrieveContainerFile, "GetContainerFile");
@@clientName(Containers.retrieveContainerFileContent, "GetContainerFileContent");
