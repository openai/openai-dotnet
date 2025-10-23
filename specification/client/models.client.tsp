import "../base/typespec/models/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@clientName(listModels, "GetModels");
@@clientName(retrieveModel, "GetModel");

@@clientLocation(listModels, "Models");
@@clientLocation(deleteModel, "Models");
@@clientLocation(retrieveModel, "Models");